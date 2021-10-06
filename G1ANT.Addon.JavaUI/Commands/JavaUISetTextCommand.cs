using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.settext",
        Tooltip = "This command sets text of a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUISetTextCommand : Command
    {
        private IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Path to a desktop Java application UI element to set the text")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Text to be set at a specified node")]
            public TextStructure Text { get; set; }
        }

        public JavaUISetTextCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            using (var node = pathService.GetByXPath(arguments.Path.Value))
                node.SetTextContents(arguments.Text.Value);
        }
    }
}
