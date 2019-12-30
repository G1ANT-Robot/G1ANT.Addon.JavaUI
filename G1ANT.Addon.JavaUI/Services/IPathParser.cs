using System.Collections.Generic;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface IPathParser
    {
        List<PathElement> Parse(string path);
    }
}
