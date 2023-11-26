using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/NormalizeVector", overload: true)]
    public class NormalizeVector4 : FlowChartNode
    {
        [FlowChartInput("Vector")]
        public Vector4 Vector;

        [FlowChartOutput("Normalized value")]
        public Vector4 NValue;

        [FlowChartStream("Next")]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            NValue = Vector.normalized;
            return Next;
        }
    }
}
