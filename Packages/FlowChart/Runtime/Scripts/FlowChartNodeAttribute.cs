using System;

namespace ZKnight.UFlowChart.Runtime
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class FlowChartNodeAttribute : Attribute
    {
        public FlowChartNodeAttribute(string editorPath, FlowChartNodeType type = FlowChartNodeType.Node, bool hidden = false, bool overload = false, bool abenden = false)
        {
            NodeType = type;
            EditorPath = editorPath;
            Hidden = hidden;
            Overload = overload;
            Abenden = abenden;
        }
        public FlowChartNodeType NodeType { get; }
        public string EditorPath { get; }
        public bool Hidden { get; }
        public bool Overload { get; }
        public bool Abenden { get; }
    }
}
