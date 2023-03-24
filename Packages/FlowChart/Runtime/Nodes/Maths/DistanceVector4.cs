using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Distance", overload: true)]
    public class DistanceVector4 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector4 A;
        [FlowChartInput("B")]
        public Vector4 B;
        [FlowChartOutput("Distance")]
        public float Distance;
        [FlowChartStream]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Distance = Vector4.Distance(A, B);
            return Next;
        }
    }
}
