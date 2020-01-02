using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace G1ANT.Addon.JavaUI
{
    [Structure(Name = "javanode", Default = "", AutoCreate = false)]
    public class JavaNodeStructure : StructureTyped<NodeModel>
    {
        public JavaNodeStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        { }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
                return new JavaPathStructure(Value, Format);
            else
                return GetByIndex(index);

            throw new ArgumentException($"Unknown index '{index}'");
        }

        private Structure GetByIndex(string index)
        {
            switch (index.ToLower())
            {
                case "jvmid":
                    return new IntegerStructure(Value.JvmId);
                case "id":
                    return new IntegerStructure(Value.Id);
                case "name":
                    return new TextStructure(Value.Name);
                case "description":
                    return new TextStructure(Value.Description);
                case "role":
                case "type":
                    return new TextStructure(Value.Role);
                case "actions":
                    return new ListStructure(Value.Actions);
                case "states":
                    return new ListStructure(Value.States);
                case "bounds":
                    return new RectangleStructure(Value.Bounds);
                case "childrencount":
                    return new IntegerStructure(Value.ChildrenCount);
                case "height":
                    return new IntegerStructure(Value.Height);
                case "indexinparent":
                    return new IntegerStructure(Value.IndexInParent);
                case "width":
                    return new IntegerStructure(Value.Width);
                case "x":
                    return new IntegerStructure(Value.X);
                case "y":
                    return new IntegerStructure(Value.Y);
                default:
                    throw new ArgumentException($"Unknown index {index}");
            }
        }


        public override void Set(Structure structure, string index = null)
        {
            if (structure == null || structure.Object == null)
                throw new ArgumentNullException(nameof(structure));
            else if (string.IsNullOrWhiteSpace(index))
            {
                if (structure is JavaNodeStructure)
                    Value = ((JavaNodeStructure)structure).Value;
                else
                    throw new ArgumentException("Incompatible structure type.");
            }
            else
                throw new ArgumentException($"Unknown index '{index}'");
        }

        public override string ToString()
        {
            var nodeProperties = Value.GetType().GetProperties()
                .Where(p => p.Name != nameof(NodeModel.Node))
                .Select(p => new { Name = p.Name, Value = p.GetValue(Value) })
                .ToDictionary(v => v.Name.ToLower(), v => v.Value is IEnumerable<string> ? string.Join(", ", v.Value as IEnumerable<string>) : v.Value);

            return new DictionaryStructure(nodeProperties, null, Scripter).ToString();
        }
    }
}

