using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Input/Float")]
    public class GetFloat : FlowChartNode
    {
        [FlowChartInput("Value")]
        public float Input;
        [FlowChartOutput("Result")]
        public float Output;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Output = Input;
            return Next;
        }
    }
}
