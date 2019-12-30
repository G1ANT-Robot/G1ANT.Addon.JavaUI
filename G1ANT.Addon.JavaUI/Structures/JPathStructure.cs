using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System;

namespace G1ANT.Addon.JavaUI
{
    [Structure(Name = "jpath", Default = "", AutoCreate = false)]
    public class JPathStructure : StructureTyped<string>
    {
        public JPathStructure(string value, string format = "", AbstractScripter scripter = null) :
            base(value, format, scripter)
        {
            Init();
        }

        public JPathStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        protected void Init()
        {
        }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
                return new JPathStructure(Value, Format);
            throw new ArgumentException($"Unknown index '{index}'");
        }

        public override void Set(Structure structure, string index = null)
        {
            if (structure == null || structure.Object == null)
                throw new ArgumentNullException(nameof(structure));
            else if (string.IsNullOrWhiteSpace(index))
                Value = structure.ToString();
            else
                throw new ArgumentException($"Unknown index '{index}'");
        }

        public override string ToString(string format)
        {
            return Value;
        }

        protected override string Parse(string value, string format = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            if (value.StartsWith(SpecialChars.Text) && value.EndsWith(SpecialChars.Text))
                value = value.Substring(1).Substring(0, value.Length - 2);

            new PathParser().Parse(value); // validation

            return value;
        }
    }
}
