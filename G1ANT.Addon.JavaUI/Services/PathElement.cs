using System;
using System.Linq;

namespace G1ANT.Addon.JavaUI.Services
{
    public class PathElement
    {
        private const string DefaultFieldName = "Name";

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Role { get; private set; }
        public int? Index { get; private set; }
        public int Id { get; private set; }
        public bool IsWildcard { get; private set; }


        public PathElement(string element)
        {
            if (element.Contains('/'))
                throw new ArgumentException($"'{PathParser.PathSeparator}' is used to separate path elements and cannot be used inside single path element definition");

            var fieldName = DefaultFieldName;
            var value = element;

            if (HasWildcard(element))
                LoadWildcard();
            else if (HasFieldAndValue(element))
                LoadFieldAndValue(element, out fieldName, out value);
            else if (IsId(value))
                LoadId(value);
            else
                Name = StripIndex(value);

            if (HasIndex(value))
            {
                LoadIndex(value);
            }
        }

        private void LoadWildcard()
        {
            IsWildcard = true;
        }

        public override string ToString()
        {
            return $"Name={Name}, Description={Description}, Role={Role}, Index={Index}, Id={Id}, IsWildCard={IsWildcard}";
        }

        private void LoadId(string value)
        {
            Id = int.Parse(value);
        }

        private static bool IsId(string value)
        {
            return int.TryParse(value, out _);
        }

        private static bool HasWildcard(string element)
        {
            return element.FirstOrDefault() == PathParser.Wildcard;
        }

        private void LoadFieldAndValue(string element, out string fieldName, out string value)
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

        private static string StripIndex(string value)
        {
            if (value.Contains('['))
                value = value.Substring(0, value.IndexOf('['));

            if (string.IsNullOrEmpty(value))
                value = null;

            return value;
        }

        private static bool HasFieldAndValue(string element)
        {
            return element.Contains('=');
        }

        private static bool HasIndex(string value)
        {
            return value.Contains('[') && value.Contains(']');
        }

        private void LoadIndex(string value)
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
}