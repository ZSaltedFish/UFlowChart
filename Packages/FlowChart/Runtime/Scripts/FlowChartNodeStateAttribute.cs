using System;

namespace ZKnight.UFlowChart.Runtime
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FlowChartNodeStateAttribute : Attribute
    {
        private readonly string _desc;
        public string Description => _desc;
        public FlowChartNodeStateAttribute(string desc)
        {
            _desc = desc;
        }
    }
}
