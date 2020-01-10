using G1ANT.Addon.JavaUI;
using G1ANT.Addon.JavaUI.Models;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System.Threading;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.UI
{
    [Command(Name = "javaui.setfocus",
        Tooltip = "This command applies focus to a specified UI element of a desktop Java application window")]
    public class JavaUISetFocusCommand : Command
    {
        private IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to apply the focus")]
            public JavaPathStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "If set to true then brings to front window associated with the node")]
            public BooleanStructure BringToFront { get; set; }
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
