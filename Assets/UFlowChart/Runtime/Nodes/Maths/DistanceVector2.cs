using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Distance", overload: true)]
    public class DistanceVector2 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector2 A;
        [FlowChartInput("B")]
        public Vector2 B;
        [FlowChartOutput("Distance")]
        public float Distance;
        [FlowChartStream]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Distance = Vector2.Distance(A, B);
            return Next;
        }
    }
}
