using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Transform/Position")]
    public class Position : FlowChartNode
    {
        [FlowChartInput("Target")]
        public GameObject Target;
        [FlowChartOutput("Position")]
        public Vector3 Pos;
        [FlowChartStream("Next")]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(System.Collections.Generic.Dictionary<string, object> @param)
        {
            Pos = Target.transform.position;
            return Next;
        }
    }
}
