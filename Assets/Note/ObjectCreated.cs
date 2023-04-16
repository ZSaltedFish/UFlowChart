using System.Collections.Generic;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Node
{
    [FlowChartNode("Root/当单位被创建", FlowChartNodeType.Root)]
    [ChatGPT("当单位被创建")]
    public class ObjectCreated : FlowChartNode
    {
        [FlowChartOutput("被创建的单位")]
        public GameObject CreatedUnit;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            return Next;
        }
    }
}
