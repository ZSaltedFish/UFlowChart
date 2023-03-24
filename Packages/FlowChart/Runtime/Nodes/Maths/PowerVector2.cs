using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Power", overload: true)]
    public class PowerVector2 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector2 A;
        [FlowChartInput("B")]
        public float B;
        [FlowChartOutput("Result")]
        public Vector2 Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            float powX = Mathf.Pow(A.x, B);
            float powY = Mathf.Pow(A.y, B);
            Result = new Vector2(powX, powY);
            return Next;
        }
    }
}
