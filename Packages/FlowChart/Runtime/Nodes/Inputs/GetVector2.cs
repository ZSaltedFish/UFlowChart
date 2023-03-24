using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Input/Vector2")]
    public class GetVector2 : FlowChartNode
    {
        [FlowChartInput("Vector")]
        public Vector2 Input;
        [FlowChartOutput("Result")]
        public Vector2 Output;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Output = Input;
            return Next;
        }
    }
}
