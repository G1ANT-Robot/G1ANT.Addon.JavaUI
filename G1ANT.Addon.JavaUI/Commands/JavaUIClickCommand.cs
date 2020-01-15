using G1ANT.Addon.JavaUI;
using G1ANT.Language;

namespace G1ANT.Addon.JavaUI
{
    [Command(Name = "javaui.click",
        Tooltip = "This command clicks a desktop Java application UI element specified by WPath structure")]
    public class JavaUIClickCommand : JavaUIDoActionCommand
    {
        new public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public JavaPathStructure Path { get; set; }
        }

        public JavaUIClickCommand(AbstractScripter scripter) : base(scripter)
        { }

        public void Execute(Arguments arguments)
        {
            var node = pathService.GetByXPath(arguments.Path.Value);
            node.DoAction("click");
        }
    }
}
