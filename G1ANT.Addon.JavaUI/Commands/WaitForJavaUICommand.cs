using G1ANT.Addon.JavaUI;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace G1ANT.Addon.UI
{
    [Command(Name = "waitfor.javaui",
        Tooltip = "This command waits for a UI element of a desktop Java application specified by JavaPath structure")]
    public class WaitForJavaUICommand : Command
    {
        private IPathService pathService;

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop Java application UI element to be awaited")]
            public JavaPathStructure Path { get; set; }

            [Argument(DefaultVariable = "timeoutui", Tooltip = "Specifies time in milliseconds for G1ANT.Robot to wait for the command to be executed. Default is 10s")]
            public override TimeSpanStructure Timeout { get; set; } = new TimeSpanStructure(10000);

        }

        public WaitForJavaUICommand(AbstractScripter scripter) : base(scripter)
        {
            pathService = new PathService();
        }

        public void Execute(Arguments arguments)
        {
            var timeout = (int)arguments.Timeout.Value.TotalMilliseconds;
            var start = Environment.TickCount;

            while (Math.Abs(Environment.TickCount - start) < timeout && !Scripter.Stopped)
            {
                try
                {
                    var nodes = pathService.GetByXPath(arguments.Path.Value);
                    if (nodes != null)
                        return;
                }
                catch { }

                Thread.Sleep(250);
                Application.DoEvents();
            }

            var errorMessage = $"Node described as \"{arguments.Path.Value}\" has not been found.";
            Scripter.Log.Log(AbstractLogger.Level.Debug, errorMessage);
            throw new TimeoutException(errorMessage);
        }
    }
}
