using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Logic/Compare")]
    public class Compare : FlowChartNode
    {
        public enum CompareFlag
        {
            Equal,
            NotEqual,
            Greater,
            Less,
            EGreater,
            ELess
        }

        [FlowChartInput("Value A")]
        public float ValueA;
        [FlowChartInput("Value B")]
        public float ValueB;
        [FlowChartOutput("Result")]
        public bool Result;
        [FlowChartStream("Next")]
        public FlowChartNode Next;
        [FlowChartNodeState("CompareFlag")]
        public CompareFlag Flag;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            switch (Flag)
            {
                case CompareFlag.Equal:
                    Result = ValueA == ValueB;
                    break;
                case CompareFlag.NotEqual:
                    Result = ValueA != ValueB;
                    break;
                case CompareFlag.Greater:
                    Result = ValueA > ValueB;
                    break;
                case CompareFlag.Less:
                    Result = ValueA < ValueB;
                    break;
                case CompareFlag.EGreater:
                    Result = ValueA >= ValueB;
                    break;
                case CompareFlag.ELess:
                    Result = ValueA <= ValueB;
                    break;
                default:
                    Result = false;
                    break;
            }
            return Next;
        }
    }
}
