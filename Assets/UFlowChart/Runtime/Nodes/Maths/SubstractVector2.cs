using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Substract", overload: true)]
    public class SubstractVector2 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector2 A;
        [FlowChartInput("B")]
        public Vector2 B;
        [FlowChartOutput("Result")]
        public Vector2 Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Result = A - B;
            return Next;
        }
    }
}
