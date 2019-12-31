using G1ANT.Addon.JavaUI;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.UI
{
    [Command(Name = "javaui.getnode",
        Tooltip = "This command performs an action at a desktop Java application UI element specified by JPath structure")]
    public class JavaUIGetNodeCommand : Command
    {
        protected IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to be clicked")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public JavaUIGetNodeCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService(
                new PathParser(),
                new NodeService(
                    new AccessBridgeFactory().GetAccessBridge()
                )
            );
        }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetNode(arguments.Path.Value);
            Scripter.Variables.SetVariableValue(
                arguments.Result.Value,
                new JavaNodeStructure(node, null, Scripter)
            );

        }
    }
}
