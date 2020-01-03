using G1ANT.Addon.JavaUI;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.UI
{
    [Command(Name = "javaui.settext",
        Tooltip = "This command inserts text into a specified UI element of a desktop Java application window")]
    public class JavaUISetTextCommand : Command
    {
        private IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to set the text")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Text to be inserted into a specified UI element")]
            public TextStructure Text { get; set; }
        }

        public JavaUISetTextCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetByXPath(arguments.Path.Value);
            node.SetTextContents(arguments.Text.Value);
        }
    }
}
