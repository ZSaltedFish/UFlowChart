using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZKnight.UFlowChart.Editor
{
    public class ParamOutput
    {
        public int OutputID;
        public Type OutputType;
        public NodeParams Parent;
        public List<ParamInput> OutputTargets;
        public string Description;
        public FieldInfo FieldInfo;

        public ParamOutput(int outputID, Type outType, NodeParams parent, FieldInfo info)
        {
            OutputID = outputID;
            OutputType = outType;
            Parent = parent;
            Description = $"Output_{OutputID}";
            OutputTargets = new List<ParamInput>();
            FieldInfo = info;
        }

        public string GetOutputName()
        {
            return $"{Parent.NodeID}_Output_{OutputID}";
        }

        public void UpdateFrom(ParamOutput newOutput)
        {
            Description = newOutput.Description;
            OutputType = newOutput.OutputType;
        }
    }
}
