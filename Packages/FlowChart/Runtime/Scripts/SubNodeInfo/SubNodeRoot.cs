using System.Collections.Generic;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Sub Node/SubNodeRoot", FlowChartNodeType.Root)]
    public class SubNodeRoot : FlowChartNode
    {
        [FlowChartOutput("Outputs")]
        public VariableParam OutputParam;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            return Next;
        }

        public override void SetInputDatas(Dictionary<string, object> paramData)
        {
        }

        public override void SetOutputDatas(Dictionary<string, object> paramData)
        {
            for (int i = 0; i < OutputParam.VariableParamType.Length; ++i)
            {
                object value = paramData[$"Input_{i}"];
                var target = OutputTargets[i];
                foreach (string str in target.Values)
                {
                    if (paramData.ContainsKey(str))
                    {
                        paramData[str] = value;
                    }
                    else
                    {
                        paramData.Add(str, value);
                    }
                }
            }
        }
    }
}
