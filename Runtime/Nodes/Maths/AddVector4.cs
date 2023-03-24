using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Add", overload: true)]
    public class AddVector4 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector4 A;
        [FlowChartInput("B")]
        public Vector4 B;
        [FlowChartOutput("Result")]
        public Vector4 Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Result = A + B;
            return Next;
        }
    }
}
