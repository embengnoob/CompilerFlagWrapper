using CompilerFlagWrapper.Options;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace CompilerFlagWrapper.Commands
{
    internal sealed class WrapSelectionCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet =
            new Guid("a815b71d-cfe0-43c8-a773-2bc88396b404");

        private readonly AsyncPackage package;

        private WrapSelectionCommand(
            AsyncPackage package,
            OleMenuCommandService commandService)
        {
            this.package = package
                ?? throw new ArgumentNullException(nameof(package));

            commandService.AddCommand(
                new MenuCommand(
                    Execute,
                    new CommandID(CommandSet, CommandId)));
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory
                .SwitchToMainThreadAsync(package.DisposalToken);

            var commandService =
                await package.GetServiceAsync(typeof(IMenuCommandService))
                    as OleMenuCommandService;

            if (commandService == null)
            {
                throw new InvalidOperationException(
                    "Visual Studio command service is unavailable.");
            }

            _ = new WrapSelectionCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = package
                .GetServiceAsync(typeof(DTE))
                .GetAwaiter()
                .GetResult() as DTE;

            var selection =
                dte?.ActiveDocument?.Selection as TextSelection;

            if (selection == null || selection.IsEmpty)
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    "Select one or more lines of code first.",
                    "Compiler Flag Wrapper",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON
                        .OLEMSGICON_INFO,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON
                        .OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON
                        .OLEMSGDEFBUTTON_FIRST);

                return;
            }

            var options =
                (GeneralOptions)package.GetDialogPage(
                    typeof(GeneralOptions));

            string flag =
                (options.Flag ?? string.Empty).Trim();

            if (flag.Length == 0)
            {
                VsShellUtilities.ShowMessageBox(
                    package,
                    "Set a compiler flag under Tools > Options > " +
                    "Compiler Flag Wrapper > General.",
                    "Compiler Flag Wrapper",
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON
                        .OLEMSGICON_WARNING,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON
                        .OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON
                        .OLEMSGDEFBUTTON_FIRST);

                return;
            }

            int startLine = selection.TopPoint.Line;
            int endLine = selection.BottomPoint.Line;

            // A selection ending at column 1 belongs to the previous line.
            if (selection.BottomPoint.LineCharOffset == 1
                && endLine > startLine)
            {
                endLine--;
            }

            EditPoint start =
                selection.Parent.CreateEditPoint();

            start.MoveToLineAndOffset(startLine, 1);

            EditPoint end =
                selection.Parent.CreateEditPoint();

            end.MoveToLineAndOffset(endLine, 1);
            end.EndOfLine();

            string newline = Environment.NewLine;
            string opening;

            switch (options.DirectiveStyle)
            {
                case DirectiveStyle.IfDefined:
                    opening =
                        $"#if defined({flag}){newline}";
                    break;

                case DirectiveStyle.IfExpression:
                    opening =
                        $"#if {flag}{newline}";
                    break;

                case DirectiveStyle.IfDef:
                default:
                    opening =
                        $"#ifdef {flag}{newline}";
                    break;
            }

            string closing = options.AddEndifComment
                ? $"{newline}#endif // {flag}"
                : $"{newline}#endif";

            // Insert closing text first so the start position does not shift.
            end.Insert(closing);
            start.Insert(opening);
        }
    }
}