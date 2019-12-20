using System;
using System.Linq;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public class PathElement
    {
        public string Name { get; }
        public string Description { get; }
        public string Role { get; }
        public int Index { get; }

        private static string StripIndex(string value)
        {
            if (value.Contains('['))
                value = value.Substring(0, value.IndexOf('['));

            if (string.IsNullOrEmpty(value))
                value = null;

            return value;
        }


        public PathElement(string element)
        {
            if (element.Contains('/'))
                throw new Exception("Slash is used to separate elements and cannot be used inside single path element definition");

            var fieldName = "Name";
            var value = element;


            if (element.Contains('='))
            {
                var fieldAndValue = element.Split('=');
                if (fieldAndValue.Count() != 2)
                    throw new Exception("Syntax is fieldName=value");

                fieldName = fieldAndValue[0];
                value = fieldAndValue[1];
                var valueWithoutIndex = StripIndex(value);

                switch (fieldName.ToLower())
                {
                    case "name":
                        Name = valueWithoutIndex;
                        break;
                    case "description":
                    case "desc":
                        Description = valueWithoutIndex;
                        break;
                    case "role":
                    case "type":
                        Role = valueWithoutIndex;
                        break;
                    default:
                        throw new Exception($"Unknown or empty field name '{valueWithoutIndex}'");
                }
            }
            else
            {
                Name = StripIndex(value);
            }

            if (value.Contains('[') && value.Contains(']'))
            {
                try
                {
                    var startIndex = value.IndexOf('[');
                    var endIndex = value.IndexOf(']');
                    var index = value.Substring(startIndex + 1, endIndex - startIndex - 1);
                    Index = int.Parse(index);
                }
                catch
                {
                    throw new Exception("Child index must be defined as [number]");
                }
                if (Index <= 0)
                    throw new Exception("Child index must be positive integer");
            }
        }

    }
}