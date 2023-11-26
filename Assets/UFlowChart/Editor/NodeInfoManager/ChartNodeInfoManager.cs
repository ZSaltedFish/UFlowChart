using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public class ChartNodeInfoManager
    {
        private static Dictionary<Type, Type> _TYPE_TO_MONO;
        public string Name;
        public List<NodeParams> Nodes;

        public ChartNodeInfoManager()
        {
            Nodes = new List<NodeParams>();
            InitializedDictionary();
        }

        private void InitializedDictionary()
        {
            _TYPE_TO_MONO = new Dictionary<Type, Type>();
            Type[] types = typeof(FlowChart).Assembly.GetTypes();
            foreach (Type type in types)
            {
                Type[] interfaceType = type.GetInterfaces();
                bool isType = false;
                foreach (var item in interfaceType)
                {
                    if (item == typeof(ISerializable))
                    {
                        isType = true;
                        break;
                    }
                }
                if (isType)
                {
                    FieldInfo field = type.GetField("FieldValue");
                    if (!_TYPE_TO_MONO.ContainsKey(field.FieldType))
                    {
                        _TYPE_TO_MONO.Add(field.FieldType, type);
                    }
                }
            }
        }

        #region NodeParams 操作
        public void AddNodeParams(NodeParams node)
        {
            Nodes.Add(node);
        }

        public void RemoveNodeParams(NodeParams node)
        {
            Nodes.Remove(node);
        }
        #endregion
        #region 生成GameObject
        public GameObject Generate()
        {
            Dictionary<int, FlowChartNode> id2ChartNode = new Dictionary<int, FlowChartNode>();
            List<ParamStream> streams = new List<ParamStream>();

            GameObject gObj = new GameObject(Name);
            _ = gObj.AddComponent<FlowChart>();
            gObj.transform.position = Vector3.zero;

            foreach (NodeParams node in Nodes)
            {
                GameObject sub = new GameObject($"{node.NodeClass.Name}_{node.NodeID}");
                sub.transform.SetParent(gObj.transform);
                sub.transform.position = node.Position;
                
                FlowChartNode fNode = sub.AddComponent(node.NodeClass) as FlowChartNode;
                fNode.ChartID = node.NodeID;
                FillInput(fNode, node);
                FillOutput(fNode, node);
                FillVariableParam(fNode, node);
                streams.AddRange(node.Streams);

                id2ChartNode.Add(node.NodeID, fNode);
                FillNodeState(fNode, node);
            }

            FillStream(streams, id2ChartNode);
            return gObj;
        }

        private void FillVariableParam(FlowChartNode fNode, NodeParams node)
        {
            foreach (FieldInfo field in fNode.GetType().GetFields())
            {
                if (field.FieldType != typeof(VariableParam))
                {
                    continue;
                }

                VariableParam param = new VariableParam();
                FlowChartInputAttribute inAttr = field.GetCustomAttribute<FlowChartInputAttribute>();
                FlowChartOutputAttribute outAttr = field.GetCustomAttribute<FlowChartOutputAttribute>();

                if (inAttr != null)
                {
                    List<string> names = new List<string>(), types = new List<string>();
                    foreach (ParamInput input in node.Inputs)
                    {
                        names.Add(input.Description);
                        types.Add(input.InputType.FullName);
                    }
                    param.Names = names.ToArray();
                    param.VariableParamType = types.ToArray();
                }

                if (outAttr != null)
                {
                    List<string> names = new List<string>(), types = new List<string>();
                    foreach (ParamOutput output in node.Outputs)
                    {
                        names.Add(output.Description);
                        types.Add(output.OutputType.FullName);
                    }
                    param.Names = names.ToArray();
                    param.VariableParamType = types.ToArray();
                }

                field.SetValue(fNode, param);
            }
        }

        private void FillOutput(FlowChartNode fNode, NodeParams node)
        {
            List<OutputStringValue> values = new List<OutputStringValue>();
            for (int i = 0; i < node.Outputs.Count; ++i)
            {
                OutputStringValue value = new OutputStringValue()
                {
                    Index = i
                };
                values.Add(value);
                List<string> keys = new List<string>();
                ParamOutput output = node.Outputs[i];
                foreach (var item in output.OutputTargets)
                {
                    string name = item.Parent.GetInputName(item.InputID);
                    keys.Add(name);
                }
                value.Values = keys.ToArray();
            }
            fNode.OutputTargets = values.ToArray();
        }

        private void FillNodeState(FlowChartNode fNode, NodeParams node)
        {
            Type fType = fNode.GetType();
            foreach (ParamNodeState pNodeState in node.NodeStates)
            {
                FieldInfo field = fType.GetField(pNodeState.FieldName);
                field.SetValue(fNode, pNodeState.EnumValue);
            }
        }

        private void FillStream(List<ParamStream> streams, Dictionary<int, FlowChartNode> dict)
        {
            foreach (ParamStream ps in streams)
            {
                if (ps.Connection != null)
                {
                    int sNodeID = ps.Parent.NodeID;
                    int targetNodeID = ps.Connection.NodeID;

                    FlowChartNode sNode = dict[sNodeID];
                    FlowChartNode tNode = dict[targetNodeID];
                    FieldInfo info = sNode.GetType().GetField(ps.FieldName);
                    info.SetValue(sNode, tNode);
                }
            }
        }

        private void FillInput(FlowChartNode fNode, NodeParams node)
        {
            foreach (ParamInput input in node.Inputs)
            {
                if (input.FieldInfo == null)
                {
                    continue;
                }
                if (!input.IsDynamicInput)
                {
                    input.FieldInfo.SetValue(fNode, input.StaticInput);
                }
            }

            foreach (ParamInput hiddenInput in node.HiddenInputs)
            {
                hiddenInput.FieldInfo.SetValue(fNode, hiddenInput.StaticInput);
            }
        }
        #endregion

        #region 读取GameObject
        public void ReadFromGameObject(GameObject gObj)
        {
            Nodes = new List<NodeParams>();
            Name = gObj.name;
            FlowChartNode[] nodes = gObj.GetComponentsInChildren<FlowChartNode>();
            Dictionary<int, NodeParams> id2Node = new Dictionary<int, NodeParams>();

            foreach (FlowChartNode node in nodes)
            {
                FlowChartNodeAttribute attr = node.GetType().GetCustomAttribute<FlowChartNodeAttribute>();
                NodeParams nodeParams = new NodeParams(node.ChartID, node.GetType(), attr.EditorPath)
                {
                    Position = node.transform.position,
                    Parent = this
                };
                nodeParams.InitVariableParam(node);
                Nodes.Add(nodeParams);
                id2Node.Add(nodeParams.NodeID, nodeParams);
            }

            foreach (FlowChartNode node in nodes)
            {
                NodeParams nodeParams = id2Node[node.ChartID];
                foreach (ParamInput input in nodeParams.Inputs)
                {
                    if (input.FieldInfo != null)
                    {
                        nodeParams.SetInputObject(input.InputID, input.FieldInfo.GetValue(node));
                    }
                }

                foreach (ParamInput hiddenInput in nodeParams.HiddenInputs)
                {
                    hiddenInput.SetStaticInput(hiddenInput.FieldInfo.GetValue(node));
                }

                foreach (OutputStringValue target in node.OutputTargets)
                {
                    int index = target.Index;
                    foreach (string key in target.Values)
                    {
                        AnalyseInputString(key, out int inputNodeID, out int inputIndex);
                        NodeParams inputNodeParams = id2Node[inputNodeID];
                        inputNodeParams.ConnectInput(nodeParams, index, inputIndex);
                    }
                }

                foreach (ParamStream stream in nodeParams.Streams)
                {
                    FieldInfo info = node.GetType().GetField(stream.FieldName);
                    FlowChartNode connectValue = info.GetValue(node) as FlowChartNode;
                    if (connectValue != null)
                    {
                        nodeParams.ConnectNode(stream.StreamID, id2Node[connectValue.ChartID]);
                    }
                }

                foreach (ParamNodeState nodeState in nodeParams.NodeStates)
                {
                    FieldInfo field = node.GetType().GetField(nodeState.FieldName);
                    int value = (int)field.GetValue(node);
                    nodeState.EnumValue = value;
                }
            }
        }

        private void AnalyseInputString(string str, out int nodeID, out int inputIndex)
        {
            string[] datas = str.Split('_');
            nodeID = int.Parse(datas[0]);
            inputIndex = int.Parse(datas[2]);
        }
        #endregion
    }
}
