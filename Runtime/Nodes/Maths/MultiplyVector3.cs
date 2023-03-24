using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Multiply", overload: true)]
    public class MultiplyVector3 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector3 A;
        [FlowChartInput("B")]
        public float B;
        [FlowChartOutput("Result")]
        public Vector3 Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Result = A * B;
            return Next;
        }
    }
}
