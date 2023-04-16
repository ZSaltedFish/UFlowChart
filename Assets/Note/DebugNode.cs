using System.Collections.Generic;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Node
{
    [FlowChartNode("Debug", FlowChartNodeType.Node)]
    [ChatGPT("Debug")]
    public class DebugNode : FlowChartNode
    {
        public enum DebugType
        {
            Message,
            Warnning,
            Error
        }

        [FlowChartInput("Input")]
        public string DebugValue;
        [FlowChartNodeState("Debug Type")]
        public DebugType DType;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            switch (DType)
            {
                case DebugType.Message:
                    Debug.Log(DebugValue); break;
                case DebugType.Warnning:
                    Debug.LogWarning(DebugValue); break;
                case DebugType.Error:
                    Debug.LogError(DebugValue); break;
            }
            return Next;
        }
    }
}
