using CompilerFlagWrapper.Commands;
using CompilerFlagWrapper.Options;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CompilerFlagWrapper
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Compiler Flag Wrapper", "Wraps selected code with a configurable compiler flag.", "1.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(GeneralOptions), "Compiler Flag Wrapper", "General", 0, 0, true)]
    [Guid(PackageGuidString)]
    public sealed class CompilerFlagWrapperPackage : AsyncPackage
    {
        public const string PackageGuidString = "f6ca82f5-53e3-4fa3-9d2a-59aa08d5988c";

        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await WrapSelectionCommand.InitializeAsync(this);
        }
    }
}
