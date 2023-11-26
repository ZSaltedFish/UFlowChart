using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("", hidden: true)]
    public class SubChart : FlowChartNode
    {
        [FlowChartInput("Inputs")]
        public VariableParam Inputs;
        [FlowChartOutput("Outputs")]
        public VariableParam Outputs;
        [FlowChartInput("", true)]
        public GameObject SubChartPrefab;
        [FlowChartStream]
        public FlowChartNode Next;

        private Dictionary<string, object> _params;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            FlowChart flowChart = SubChartPrefab.GetComponent<FlowChart>();
            flowChart.Init();
            FlowChartNode node = SubChartPrefab.GetComponentInChildren<SubNodeRoot>();

            while (node != null)
            {
                node.SetInputDatas(_params);
                FlowChartNode next = node.FlowChartContent(_params);
                node.SetOutputDatas(_params);
                node = next;
            }
            return Next;
        }

        public override void SetInputDatas(Dictionary<string, object> paramData)
        {
            _params = new Dictionary<string, object>();
            for (int i = 0; i < Inputs.VariableParamType.Length; ++i)
            {
                object value = paramData[GetInputName(i)];
                _params.Add($"Input_{i}", value);
            }
        }

        public override void SetOutputDatas(Dictionary<string, object> paramData)
        {
            for (int i = 0; i < Outputs.VariableParamType.Length; ++i)
            {
                OutputStringValue sValue = OutputTargets[i];
                foreach (string key in sValue.Values)
                {
                    object value = _params[$"Output_{i}"];
                    if (paramData.ContainsKey(key))
                    {
                        paramData[key] = value;
                    }
                    else
                    {
                        paramData.Add(key, value);
                    }
                }
            }
        }
    }
}
