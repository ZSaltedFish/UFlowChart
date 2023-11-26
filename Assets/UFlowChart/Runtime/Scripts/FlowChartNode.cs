using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public abstract class FlowChartNode : MonoBehaviour
    {
        public int ChartID;
        public FlowChart ParentChart;
        public OutputStringValue[] OutputTargets;

        private List<FieldInfo> _inputs;
        private List<FieldInfo> _outputs;
        public bool IsRoot = false;
        /// <summary>
        /// Content
        /// </summary>
        /// <param name="params">Param values</param>
        /// <returns>Next FlowChart</returns>
        public abstract FlowChartNode FlowChartContent(Dictionary<string, object> @params);

        public void InitIO()
        {
            _inputs = new List<FieldInfo>();
            _outputs = new List<FieldInfo>();
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in fields)
            {
                FlowChartInputAttribute inAttr = info.GetCustomAttribute<FlowChartInputAttribute>();
                if (inAttr != null)
                {
                    _inputs.Add(info);
                }

                FlowChartOutputAttribute outAttr = info.GetCustomAttribute<FlowChartOutputAttribute>();
                if (outAttr != null)
                {
                    _outputs.Add(info);
                }
            }
        }

        public virtual void SetInputDatas(Dictionary<string, object> paramData)
        {
            for (int index = 0; index < _inputs.Count; ++index)
            {
                FieldInfo item = _inputs[index];
                string inputKey = GetInputName(index);
                if (paramData.TryGetValue(inputKey, out object value))
                {
                    try
                    {
                        if (ParamTypeMatch.TryTransformData(value, item.FieldType, out object result))
                        {
                            item.SetValue(this, result);
                        }
                        else
                        {
                            Debug.LogError($"{ParentChart.name}({name})没有{value.GetType()} 到{item.FieldType}的适配方法，{item.Name}的值将会保持为NULL");
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.LogError($"Set Input:{ChartID} -> {index} -> {item.FieldType} -> {ParentChart.gameObject.name}\n{err}");
                        throw err;
                    }
                }
            }
        }

        public virtual void SetOutputDatas(Dictionary<string, object> paramData)
        {
            for (int i = 0; i < _outputs.Count; ++i)
            {
                FieldInfo info = _outputs[i];
                object value = info.GetValue(this);
                OutputStringValue sValue = OutputTargets[i];
                foreach (string str in sValue.Values)
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

        public string GetInputName(int index)
        {
            return $"{ChartID}_Input_{index}";
        }

        public string GetOutputName(int index)
        {
            return $"{ChartID}_Output_{index}";
        }

        public override string ToString()
        {
            return $"{name}";
        }
    }
}