using G1ANT.Language;
using System;

namespace G1ANT.Addon.JavaUI
{
    [Structure(Name = "nodestructure", Default = "", AutoCreate = false)]
    public class NodeStructure : StructureTyped<NodeModel>
    {
        public NodeStructure(string value, string format = "", AbstractScripter scripter = null) :
            base(value, format, scripter)
        {
            Init();
        }

        public NodeStructure(object value, string format = null, AbstractScripter scripter = null)
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
            {
                if (structure is NodeStructure)
                    Value = ((NodeStructure)structure).Value;
                else
                    throw new ArgumentException("Incompatible structure type.");
            }
            else
                throw new ArgumentException($"Unknown index '{index}'");
        }

        public override string ToString(string format)
        {
            return Value.Name;
        }
    }
}
