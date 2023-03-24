using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Input/Vector3")]
    public class GetVector3 : FlowChartNode
    {
        [FlowChartInput("Vector")]
        public Vector3 Input;
        [FlowChartOutput("Result")]
        public Vector3 Output;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Output = Input;
            return Next;
        }
    }
}
