using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKnight.UFlowChart.Editor
{
    public class ParamInput
    {
        public int InputID;
        public List<ParamOutput> Sources;
        public object StaticInput;
        public bool IsDynamicInput;
        public Type InputType;
        public NodeParams Parent;
        public string Description;
        public FieldInfo FieldInfo;
        public bool Hidden = false;

        public ParamInput(int id, Type inputType, NodeParams parent, FieldInfo info)
        {
            Sources = new List<ParamOutput>();
            InputID = id;
            InputType = inputType;
            Parent = parent;
            Description = $"Input_{id}";
            FieldInfo = info;
        }

        public void SetInput(ParamOutput output)
        {
            IsDynamicInput = true;
            Sources.Add(output);

            if (!output.OutputTargets.Contains(this))
            {
                output.OutputTargets.Add(this);
            }
        }

        public void DisconnectInput(ParamOutput output)
        {
            IsDynamicInput = false;
            output.OutputTargets.Remove(this);
            StaticInput = FieldType2Function.GetDefaultValue(InputType);
            Sources.Remove(output);
        }

        public void SetStaticInput(object obj)
        {
            StaticInput = obj;
        }

        public void UpdateFrom(ParamInput newInput)
        {
            Description = newInput.Description;
            Hidden = newInput.Hidden;
            InputType = newInput.InputType;
            FieldInfo = newInput.FieldInfo;
            StaticInput = newInput.StaticInput;
        }
    }
}
