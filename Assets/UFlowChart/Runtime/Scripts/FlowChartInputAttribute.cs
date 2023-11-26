using System;
using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class FlowChartInputAttribute : Attribute
    {
        private readonly string _desc;
        private readonly bool _hidden;
        public FlowChartInputAttribute(string desc, bool hidden = false)
        {
            _desc = desc;
            _hidden = hidden;
        }

        public string Description
        {
            get { return _desc; }
        }

        public bool Hidden => _hidden;
    }
}
