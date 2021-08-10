using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System.Drawing;
using System.Threading;

namespace G1ANT.Addon.JavaUI.Commands
{
    [Command(Name = "javaui.mouseclick",
        Tooltip = "This command simulates a mouse click at a desktop Java application UI element specified by JavaPath structure")]
    public class JavaUIMouseClickCommand : Command
    {
        private readonly PathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Path to a desktop application UI element to be clicked (done by sending mouse left button down/up at element middle)")]
            public JavaPathStructure Path { get; set; }
        }

        public JavaUIMouseClickCommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            using (var node = pathService.GetByXPath(arguments.Path.Value))
            {
                node.BringToFront();

                var currentMousePosition = MouseWin32.GetPhysicalCursorPosition();
                var nodeMiddlePosition = new Point(node.X + node.Width / 2, node.Y + node.Height / 2);

                var mouseArgs = MouseStr.ToMouseEventsArgs(nodeMiddlePosition.X, nodeMiddlePosition.Y, currentMousePosition.X, currentMousePosition.Y, "left");

                foreach (var arg in mouseArgs)
                {
                    MouseWin32.MouseEvent(arg.dwFlags, arg.dx, arg.dy, arg.dwData);
                    Thread.Sleep(10);
                }
            }
        }
    }
}