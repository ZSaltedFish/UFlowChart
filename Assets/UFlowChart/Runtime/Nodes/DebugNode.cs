using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Debug/DebugNode")]
    public class DebugNode : FlowChartNode
    {
        public enum DebugType
        {
            Log,
            Warning,
            Error
        }

        [FlowChartInput("Content")]
        public string Content;
        [FlowChartStream]
        public FlowChartNode Next;

        public DebugType Type;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            switch (Type)
            {
                case DebugType.Log:
                    UnityEngine.Debug.Log(Content);
                    break;
                case DebugType.Warning:
                    UnityEngine.Debug.LogWarning(Content);
                    break;
                case DebugType.Error:
                    UnityEngine.Debug.LogError(Content);
                    break;
                default:
                    break;
            }

            return Next;
        }
    }
}
