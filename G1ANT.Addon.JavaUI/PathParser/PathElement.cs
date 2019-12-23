using System;
using System.Linq;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public class PathElement
    {
        public string Name { get; }
        public string Description { get; }
        public string Role { get; }
        public int? Index { get; }
        public int Id { get; }
        public bool IsWildCard { get; }

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
                throw new ArgumentException($"'{PathParser.PathSeparator}' is used to separate path elements and cannot be used inside single path element definition");

            var fieldName = "Name";
            var value = element;

            if (element.FirstOrDefault() == PathParser.Wildcard)
                IsWildCard = true;
            else if (element.Contains('='))
            {
                var fieldAndValue = element.Split('=');
                if (fieldAndValue.Count() != 2)
                    throw new ArgumentException("Syntax is fieldName=value");

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
                    case "id":
                        Id = int.Parse(valueWithoutIndex);
                        break;
                    default:
                        throw new ArgumentException($"Unknown or empty field name '{valueWithoutIndex}'");
                }
            }
            else
            {
                if (int.TryParse(value, out int id))
                    Id = id;
                else
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
                    throw new ArgumentException("Child index must be defined as [number]");
                }
                if (Index < 0)
                    throw new ArgumentOutOfRangeException("Child index must be a not negative integer");
            }
        }


        public override string ToString()
        {
            return $"Name={Name}, Description={Description}, Role={Role}, Index={Index}, Id={Id}, IsWildCard={IsWildCard}";
        }
    }
}