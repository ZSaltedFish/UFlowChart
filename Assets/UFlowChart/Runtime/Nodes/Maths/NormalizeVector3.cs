using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/NormalizeVector", overload: true)]
    public class NormalizeVector3 : FlowChartNode
    {
        [FlowChartInput("Vector")]
        public Vector3 Vector;

        [FlowChartOutput("Normalized value")]
        public Vector3 NValue;

        [FlowChartStream("Next")]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            NValue = Vector.normalized;
            return Next;
        }
    }
}
