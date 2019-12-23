using System.Collections.Generic;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public interface IPathParser
    {
        List<PathElement> Parse(string path);
    }
}
