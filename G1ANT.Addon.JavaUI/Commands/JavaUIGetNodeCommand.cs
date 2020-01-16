using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using Newtonsoft.Json;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.getnode",
        Tooltip = "This command allows to get node details from a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUIGetNodeCommand : Command
    {
        protected IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "XPath to a desktop Java application UI element for which detals are requested")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public JavaUIGetNodeCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetByXPath(arguments.Path.Value);
            var json = JsonConvert.SerializeObject(node);

            Scripter.Variables.SetVariableValue(
                arguments.Result.Value,
                new JsonStructure(json)
            );

        }
    }
}
