using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Power", overload: true)]
    public class PowerVector3 : FlowChartNode
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
            float powX = Mathf.Pow(A.x, B);
            float powY = Mathf.Pow(A.y, B);
            float powZ = Mathf.Pow(A.z, B);
            Result = new Vector3(powX, powY, powZ);
            Result = A * B;
            return Next;
        }
    }
}
