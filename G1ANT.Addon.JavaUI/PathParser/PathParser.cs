using System;
using System.Collections.Generic;
using System.Linq;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public partial class PathParser
    {
        public List<PathElement> Parse(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var elementsPath = path
                .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(pe => new PathElement(pe))
                .ToList();
            return elementsPath;
        }

    }
}
