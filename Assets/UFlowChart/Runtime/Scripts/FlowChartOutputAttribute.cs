using System;

namespace ZKnight.UFlowChart.Runtime
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FlowChartOutputAttribute : Attribute
    {
        private readonly string _desc;
        public FlowChartOutputAttribute(string desc)
        {
            _desc = desc;
        }

        public string Description
        {
            get { return _desc; }
        }

        // This is a named argument
        public int NamedInt { get; set; }
    }
}
