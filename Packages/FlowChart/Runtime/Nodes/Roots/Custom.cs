using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Root/Custom", FlowChartNodeType.Root)]
    public class Custom : FlowChartNode
    {
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            return Next;
        }
    }
}
