using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Power", overload: true)]
    public class PowerFloat : FlowChartNode
    {
        [FlowChartInput("A")]
        public float A;
        [FlowChartInput("B")]
        public float B;
        [FlowChartOutput("Result")]
        public float Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Result = Mathf.Pow(A, B);
            return Next;
        }
    }
}
