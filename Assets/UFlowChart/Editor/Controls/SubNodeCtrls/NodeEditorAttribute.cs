using System;

namespace ZKnight.UFlowChart.Editor
{
    public sealed class NodeEditorAttribute : Attribute
    {
        public Type TargetType;

        public NodeEditorAttribute(Type type)
        {
            TargetType = type;
        }
    }
}
