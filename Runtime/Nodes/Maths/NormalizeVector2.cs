using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/NormalizeVector", overload:true)]
    public class NormalizeVector2 : FlowChartNode
    {
        [FlowChartInput("Vector")]
        public Vector2 Vector;

        [FlowChartOutput("Normalized value")]
        public Vector2 NValue;

        [FlowChartStream("Next")]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            NValue = Vector.normalized;
            return Next;
        }
    }
}
