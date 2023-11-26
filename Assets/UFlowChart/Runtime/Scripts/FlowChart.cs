using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class FlowChart : MonoBehaviour
    {
        [NonSerialized]
        public List<FlowChartNode> Nodes;
        private Dictionary<Type, List<FlowChartNode>> _type2Root;

        public void Init()
        {
            Nodes = new List<FlowChartNode>(GetComponentsInChildren<FlowChartNode>());
            _type2Root = new Dictionary<Type, List<FlowChartNode>>();
            foreach (FlowChartNode node in Nodes)
            {
                node.ParentChart = this;
                node.InitIO();

                object[] attrs = node.GetType().GetCustomAttributes(typeof(FlowChartNodeAttribute), false);
                if (attrs == null)
                {
                    continue;
                }
                if (!(attrs[0] is FlowChartNodeAttribute attr))
                {
                    continue;
                }
                if (attr.NodeType == FlowChartNodeType.Root)
                {
                    if (!_type2Root.TryGetValue(node.GetType(), out List<FlowChartNode> list))
                    {
                        list = new List<FlowChartNode>();
                        _type2Root.Add(node.GetType(), list);
                    }
                    list.Add(node);
                }
            }
        }

        private Dictionary<string, object> InitParam()
        {
            Dictionary<string, object> paramDatas = new Dictionary<string, object>();
            ISerializable[] serializables = GetComponentsInChildren<ISerializable>();
            foreach (ISerializable serializable in serializables)
            {
                string key = serializable.Key;
                object value = serializable.Value;
                paramDatas.Add(key, value);
            }
            return paramDatas;
        }

        public void Run(Type type, params object[] @params)
        {
            Dictionary<string, object> paramDatas = InitParam();
            if (_type2Root == null)
            {
                Init();
            }
            if (_type2Root.TryGetValue(type, out List<FlowChartNode> nodes))
            {
                foreach (FlowChartNode node in nodes)
                {
                    foreach (var item in node.OutputTargets)
                    {
                        object obj = @params[item.Index];
                        foreach (string key in item.Values)
                        {
                            if (paramDatas.ContainsKey(key))
                            {
                                paramDatas[key] = obj;
                            }
                            else
                            {
                                paramDatas.Add(key, obj);
                            }
                        }
                    }

                    DoRunNode(node, paramDatas);
                }
            }
        }

        public void Run<T>(params object[] @params) where T : FlowChartNode
        {
            try
            {
                Run(typeof(T), @params);
            }
            catch (Exception err)
            {
                Debug.LogError($"错误发生:{name}\n{err}");
            }
        }

        private void DoRunNode(FlowChartNode node, Dictionary<string, object> paramDatas)
        {
            FlowChartNode runNode = node;
            try
            {
                while (runNode != null)
                {
                    runNode.SetInputDatas(paramDatas);
                    FlowChartNode next = runNode.FlowChartContent(paramDatas);
                    if (node != runNode)
                    {
                        runNode.SetOutputDatas(paramDatas);
                    }
                    runNode = next;
                }
            }
            catch (Exception err)
            {
                Debug.LogError($"错误发生:{name}({runNode})\n{err}");
            }
        }
    }
}
