using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Power", overload: true)]
    public class PowerVector4 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector4 A;
        [FlowChartInput("B")]
        public float B;
        [FlowChartOutput("Result")]
        public Vector4 Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            float powX = Mathf.Pow(A.x, B);
            float powY = Mathf.Pow(A.y, B);
            float powZ = Mathf.Pow(A.z, B);
            float powW = Mathf.Pow(A.w, B);
            Result = new Vector4(powX, powY, powZ, powW);
            return Next;
        }
    }
}
