using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.getrectangle",
        Tooltip = "This command gets a bounding box of a desktop Java application UI element specified by WPath structure")]
    public class JavaUIGetRectangleCommand : Command
    {
        private PathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to be located as a bounding box")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored in rectangle structure")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public JavaUIGetRectangleCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetByXPath(arguments.Path.Value);
            Scripter.Variables.SetVariableValue(
                arguments.Result.Value,
                new RectangleStructure(node.Bounds, null, Scripter)
            );
        }
    }
}
