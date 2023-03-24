using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Input/Vector4")]
    public class GetVector4 : FlowChartNode
    {
        [FlowChartInput("Vector")]
        public Vector4 Input;
        [FlowChartOutput("Result")]
        public Vector4 Output;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Output = Input;
            return Next;
        }
    }
}
