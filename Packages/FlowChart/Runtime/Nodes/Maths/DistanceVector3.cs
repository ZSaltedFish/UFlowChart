using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Distance", overload: true)]
    public class DistanceVector3 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector3 A;
        [FlowChartInput("B")]
        public Vector3 B;
        [FlowChartOutput("Distance")]
        public float Distance;
        [FlowChartStream]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Distance = Vector3.Distance(A, B);
            return Next;
        }
    }
}
