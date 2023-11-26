using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Multiply", overload: true)]
    public class MultiplyFloat : FlowChartNode
    {
        [FlowChartInput("A")]
        public float A;
        [FlowChartInput("B")]
        public float B;
        [FlowChartOutput("Result")]
        public float Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Result = A * B;
            return Next;
        }
    }
}
