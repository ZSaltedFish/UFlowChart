using System.Collections.Generic;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Node
{
    [FlowChartNode("Move")]
    [ChatGPT("移动单位")]
    public class Move : FlowChartNode
    {
        [FlowChartInput("需要移动的单位")]
        public MoveToPoint MoveUnit;
        [FlowChartInput("移动距离")]
        public float Distance;
        [FlowChartInput("移动速度")]
        public float Speed;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            var point = MoveUnit.transform.position + MoveUnit.transform.forward * Distance;
            MoveUnit.Move(point, Speed);
            return Next;
        }
    }
}
