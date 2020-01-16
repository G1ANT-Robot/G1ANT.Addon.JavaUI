using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.setfocus",
        Tooltip = "This command applies focus to a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUISetFocusCommand : Command
    {
        private IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Path to a desktop Java application UI element to apply the focus")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = false, Tooltip = "Bring to front window associated with the node (false by default)")]
            public BooleanStructure BringToFront { get; set; } = new BooleanStructure(false);
        }

        public JavaUISetFocusCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetByXPath(arguments.Path.Value);
            if (arguments.BringToFront.Value)
            {
                node.BringToFront();
            }

            node.RequestFocus();
        }
    }
}
