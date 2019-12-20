using System.Collections.Generic;
using System.Drawing;

namespace G1ANT.Addon.JavaUI
{
    public class NodeModel
    {
        public string JvmId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>type</summary>
        public string Role { get; set; }
        public List<string> Actions { get; set; }
        public List<string> States { get; set; }
        public Rectangle Bounds { get; set; }
    }
}

