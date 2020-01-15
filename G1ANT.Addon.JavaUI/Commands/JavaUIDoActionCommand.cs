using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.doaction",
        Tooltip = "This command performs an action at a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUIDoActionCommand : Command
    {
        protected IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "XPath to a desktop Java application UI element for which an acion is requested")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Name of an action (node must allow to execute this action, see Actions property, otherwise - for a click - use javaui.mouseclick)")]
            public TextStructure Action { get; set; }
        }

        public JavaUIDoActionCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetByXPath(arguments.Path.Value);
            node.DoAction(arguments.Action.Value);
        }
    }
}
