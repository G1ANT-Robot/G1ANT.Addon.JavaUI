using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.doaction",
        Tooltip = "This command performs an action at a desktop Java application UI element specified by JPath structure")]
    public class JavaUIDoActionCommand : Command
    {
        protected IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to be clicked")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Desktop Java application UI element to be clicked")]
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
