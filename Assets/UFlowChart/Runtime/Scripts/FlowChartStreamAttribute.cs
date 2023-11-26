using System;

namespace ZKnight.UFlowChart.Runtime
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FlowChartStreamAttribute : Attribute
    {
        private string _desc;
        public string Description => _desc;

        public FlowChartStreamAttribute(string description = "Next")
        {
            _desc = description;
        }
    }
}
