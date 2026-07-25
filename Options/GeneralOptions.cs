using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace CompilerFlagWrapper.Options
{
    public enum DirectiveStyle
    {
        IfDef,
        IfDefined,
        IfExpression
    }

    public sealed class GeneralOptions : DialogPage
    {
        [Category("Compiler flag")]
        [DisplayName("Flag")]
        [Description("The preprocessor symbol used around the selected lines.")]
        [DefaultValue("FEATURE_FLAG")]
        public string Flag { get; set; } = "FEATURE_FLAG";

        [Category("Formatting")]
        [DisplayName("Directive style")]
        [Description(
            "Choose between '#ifdef FLAG', '#if defined(FLAG)', and '#if FLAG'.")]
        [DefaultValue(DirectiveStyle.IfDef)]
        public DirectiveStyle DirectiveStyle { get; set; }
            = DirectiveStyle.IfDef;

        [Category("Formatting")]
        [DisplayName("Add flag to #endif comment")]
        [Description("Generate '#endif // FLAG' instead of '#endif'.")]
        [DefaultValue(true)]
        public bool AddEndifComment { get; set; } = true;
    }
}