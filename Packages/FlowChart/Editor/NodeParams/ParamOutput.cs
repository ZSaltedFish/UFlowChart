using System;
using System.Collections.Generic;

namespace ZKnight.UFlowChart.Editor
{
    public class ParamOutput
    {
        public int OutputID;
        public Type OutputType;
        public NodeParams Parent;
        public List<ParamInput> OutputTargets;
        public string Description;

        public ParamOutput(int outputID, Type outType, NodeParams parent)
        {
            OutputID = outputID;
            OutputType = outType;
            Parent = parent;
            Description = $"Output_{OutputID}";
            OutputTargets = new List<ParamInput>();
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
