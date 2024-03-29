using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.gettext",
        Tooltip = "This command allows to get text from a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUIGetTextCommand : Command
    {
        private IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to get text from")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public JavaUIGetTextCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            using (var node = pathService.GetByXPath(arguments.Path.Value))
            {
                var text = node.GetTextContents();

                Scripter.Variables.SetVariableValue(
                    arguments.Result.Value,
                    new TextStructure(text, null, Scripter)
                );
            }
        }
    }
}
