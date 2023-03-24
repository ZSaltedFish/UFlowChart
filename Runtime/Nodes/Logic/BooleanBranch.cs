using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Logic/BooleanBranch")]
    public class BooleanBranch : FlowChartNode
    {
        [FlowChartInput("Value")]
        public bool Input;
        [FlowChartStream("True")]
        public FlowChartNode TrueNext;
        [FlowChartStream("False")]
        public FlowChartNode FalseNext;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            return Input ? TrueNext : FalseNext;
        }
    }
}
