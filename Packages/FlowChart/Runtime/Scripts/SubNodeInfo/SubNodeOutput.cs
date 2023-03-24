using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Sub Node/SubNodeOutput")]
    public class SubNodeOutput : FlowChartNode
    {
        [FlowChartInput("Inputs")]
        public VariableParam InputParam;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            return null;
        }

        public override void SetInputDatas(Dictionary<string, object> paramData)
        {
            
        }

        public override void SetOutputDatas(Dictionary<string, object> paramData)
        {
            for (int i = 0; i < InputParam.VariableParamType.Length; ++i)
            {
                string key = GetInputName(i);
                object value = paramData[key];

                paramData.Add($"Output_{i}", value);
            }
        }
    }
}
