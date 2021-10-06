using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.click",
        Tooltip = "This command clicks a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUIClickCommand : JavaUIDoActionCommand
    {
        new public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "XPath to a desktop application UI element to be clicked (node must allow to execute 'click' action, see Actions property, otherwise use javaui.mouseclick)")]
            public JavaPathStructure Path { get; set; }
        }

        public JavaUIClickCommand(AbstractScripter scripter) : base(scripter)
        { }

        public void Execute(Arguments arguments)
        {
            using (var node = pathService.GetByXPath(arguments.Path.Value))
                node.DoAction("click");
        }
    }
}
