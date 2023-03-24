using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public class NodeParams
    {
        public int NodeID;
        public Type NodeClass;
        public FlowChartNodeType NodeType;
        public List<ParamInput> Inputs;
        public List<ParamInput> HiddenInputs;
        public List<ParamOutput> Outputs;
        public List<ParamStream> Streams;
        public List<ParamNodeState> NodeStates;
        public Vector2 Position;

        public List<ParamStream> SourceStreams { get; private set; }
        public string Path { get; }
        public ChartNodeInfoManager Parent { get; set; }

        public NodeParams (int nodeID, Type @class, string path)
        {
            NodeID = nodeID;
            NodeClass = @class;
            SourceStreams = new List<ParamStream>();
            Path = path;
            FlowChartNodeAttribute attr = NodeClass.GetCustomAttribute<FlowChartNodeAttribute>();
            if (attr != null)
            {
                Path = attr.EditorPath;
            }
            InitializeParams();
        }

        #region 初始化
        private void InitializeParams()
        {
            Inputs = new List<ParamInput>();
            Outputs = new List<ParamOutput>();
            Streams = new List<ParamStream>();
            HiddenInputs = new List<ParamInput>();
            NodeStates = new List<ParamNodeState>();
            FieldInfo[] infos = NodeClass.GetFields(BindingFlags.Public | BindingFlags.Instance);
            FlowChartNodeAttribute nodeAttr = NodeClass.GetCustomAttribute<FlowChartNodeAttribute>();
            if (nodeAttr == null)
            {
                throw new ArgumentException($"{NodeClass.FullName} 没有 {typeof(FlowChartNodeAttribute)}标签");
            }
            NodeType = nodeAttr.NodeType;
            int inputIndex = 0, outputIndex = 0, streamIndex = 0, hiddenInputIndex = 0;
            foreach (FieldInfo info in infos)
            {
                if (info.FieldType != typeof(VariableParam))
                {
                    FlowChartInputAttribute inAttr = info.GetCustomAttribute<FlowChartInputAttribute>();
                    if (inAttr != null)
                    {
                        if (!inAttr.Hidden)
                        {
                            Inputs.Add(CreateInput(info, inputIndex++, inAttr));
                        }
                        else
                        {
                            var input = CreateInput(info, hiddenInputIndex++, inAttr);
                            input.Hidden = true;
                            HiddenInputs.Add(input);
                        }
                    }

                    FlowChartOutputAttribute outAttr = info.GetCustomAttribute<FlowChartOutputAttribute>();
                    if (outAttr != null)
                    {
                        Outputs.Add(CreateOutput(info, outputIndex++, outAttr));
                    }

                    FlowChartStreamAttribute streamAttr = info.GetCustomAttribute<FlowChartStreamAttribute>();
                    if (streamAttr != null)
                    {
                        ParamStream stream = CreateStream(streamIndex++, streamAttr);
                        stream.FieldName = info.Name;
                        Streams.Add(stream);
                    }

                    FlowChartNodeStateAttribute stateAttr = info.GetCustomAttribute<FlowChartNodeStateAttribute>();
                    if (stateAttr != null)
                    {
                        if (!info.FieldType.IsEnum)
                        {
                            Debug.LogError($"{stateAttr.Description} 只支持枚举类型");
                            continue;
                        }

                        ParamNodeState state = CreateNodeState(info, stateAttr);
                        NodeStates.Add(state);
                    }
                }
            }
        }

        private ParamNodeState CreateNodeState(FieldInfo info, FlowChartNodeStateAttribute stateAttr)
        {
            ParamNodeState state = new ParamNodeState()
            {
                Description = stateAttr.Description,
                FieldName = info.Name,
                NType = info.FieldType
            };
            return state;
        }

        private ParamStream CreateStream(int v, FlowChartStreamAttribute streamAttr)
        {
            ParamStream stream = new ParamStream(v, this);
            if (!string.IsNullOrEmpty(streamAttr.Description))
            {
                stream.Description = streamAttr.Description;
            }
            return stream;
        }

        private ParamOutput CreateOutput(FieldInfo info, int v, FlowChartOutputAttribute outAttr)
        {
            ParamOutput output = new ParamOutput(v, info.FieldType, this);
            if (!string.IsNullOrEmpty(outAttr.Description))
            {
                output.Description = outAttr.Description;
            }
            return output;
        }

        private ParamInput CreateInput(FieldInfo info, int inputIndex, FlowChartInputAttribute attr)
        {
            ParamInput input = new ParamInput(inputIndex, info.FieldType, this, info);
            if (!string.IsNullOrEmpty(attr.Description))
            {
                input.Description = attr.Description;
            }
            return input;
        }

        private void InitVariableParam(VariableParam vParam, FieldInfo info, bool toInput, bool toOutput)
        {
            for (int i = 0; i < vParam.VariableParamType.Length; ++i)
            {
                Type type = AssemblyHelper.GetType(vParam.VariableParamType[i]);
                string name = vParam.Names[i];
                if (toInput)
                {
                    AddInput(i, type, name);
                }

                if (toOutput)
                {
                    AddOutput(i, type, name);
                }
            }
        }
        #endregion

        #region 操作
        /// <summary>
        /// 连接输入
        /// </summary>
        /// <param name="outNode">输出端节点</param>
        /// <param name="outIndex">输出节点索引</param>
        /// <param name="inIndex">输入节点索引</param>
        public void ConnectInput(NodeParams outNode, int outIndex, int inIndex)
        {
            ParamOutput output = outNode.Outputs[outIndex];
            ParamInput input = Inputs[inIndex];
            input.SetInput(output);
        }

        /// <summary>
        /// 断开输入连接
        /// </summary>
        /// <param name="inIndex">输入索引</param>
        /// <param name="output">输出连线</param>
        public void DisconnectInput(int inIndex, ParamOutput output)
        {
            ParamInput input = Inputs[inIndex];
            input.DisconnectInput(output);
        }

        /// <summary>
        /// 设置静态输入变量
        /// </summary>
        /// <param name="inIndex">输入索引</param>
        /// <param name="obj">变量值</param>
        public void SetInputObject(int inIndex, object obj)
        {
            Inputs[inIndex].SetStaticInput(obj);
        }

        /// <summary>
        /// 设置流程连接节点
        /// </summary>
        /// <param name="streamIndex">输出流程索引</param>
        /// <param name="next">下一个节点</param>
        public void ConnectNode(int streamIndex, NodeParams next)
        {
            ParamStream ps = Streams[streamIndex];
            if (ps.Connection != null)
            {
                ps.Connection.SourceStreams.Remove(ps);
            }

            ps.Connection = next;
            next.SourceStreams.Add(ps);
        }

        /// <summary>
        /// 断开流程连接
        /// </summary>
        /// <param name="streamIndex">流程索引</param>
        public void DisconnectNode(int streamIndex)
        {
            ParamStream ps = Streams[streamIndex];
            ps.Connection.SourceStreams.Remove(ps);
            ps.Connection = null;
        }

        /// <summary>
        /// 添加Input
        /// </summary>
        /// <param name="id">Index</param>
        /// <param name="type">类型</param>
        public void AddInput(int id, Type type, string name)
        {
            ParamInput input = new ParamInput(id, type, this, null)
            {
                Description = name
            };
            Inputs.Add(input);
        }

        /// <summary>
        /// 添加Output
        /// </summary>
        /// <param name="id">Index</param>
        /// <param name="type">类型</param>
        public void AddOutput(int id, Type type, string name)
        {
            ParamOutput output = new ParamOutput(id, type, this)
            {
                Description = name
            };
            Outputs.Add(output);
        }

        /// <summary>
        /// 删除Output
        /// </summary>
        /// <param name="id"></param>
        public void RemoveOutput(int id)
        {
            Outputs.RemoveAt(id);
            foreach (var item in Outputs)
            {
                if (item.OutputID > id)
                {
                    --item.OutputID;
                }
            }
        }

        /// <summary>
        /// 删除input
        /// </summary>
        /// <param name="id">ID</param>
        public void RemoveInput(int id)
        {
            Inputs.RemoveAt(id);
            foreach (var item in Inputs)
            {
                if (item.InputID > id)
                {
                    --item.InputID;
                }
            }
        }

        /// <summary>
        /// 初始化可变参数
        /// </summary>
        /// <param name="node"></param>
        public void InitVariableParam(FlowChartNode node)
        {
            FieldInfo[] infos = NodeClass.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in infos)
            {
                if (info.FieldType != typeof(VariableParam))
                {
                    continue;
                }

                VariableParam vParam = info.GetValue(node) as VariableParam;
                FlowChartInputAttribute inAttr = info.GetCustomAttribute<FlowChartInputAttribute>();
                FlowChartOutputAttribute outAttr = info.GetCustomAttribute<FlowChartOutputAttribute>();
                InitVariableParam(vParam, info, inAttr != null, outAttr != null);
            }
        }

        /// <summary>
        /// 通过子图创建
        /// </summary>
        /// <param name="prefab"></param>
        public void RefreshWidthPrefab(GameObject prefab)
        {
            SubNodeRoot root = prefab.GetComponentInChildren<SubNodeRoot>();
            SubNodeOutput output = prefab.GetComponentInChildren<SubNodeOutput>();

            FieldInfo rootInfo = root.GetType().GetField("OutputParam");
            FieldInfo outputInfo = output.GetType().GetField("InputParam");
            InitVariableParam(root.OutputParam, rootInfo, true, false);
            InitVariableParam(output.InputParam, outputInfo, false, true);
        }

        public string GetInputName(int index)
        {
            return $"{NodeID}_Input_{index}";
        }

        public string GetOutputName(int index)
        {
            return $"{NodeID}_Output_{index}";
        }

        /// <summary>
        /// 从新的参数中复制
        /// </summary>
        /// <param name="newParams"></param>
        public void UpdateFrom(NodeParams newParams)
        {
            NodeClass = newParams.NodeClass;
            for (int i = 0; i < Inputs.Count; ++i)
            {
                ParamInput input = Inputs[i];
                ParamInput newInput = newParams.Inputs[i];
                input.UpdateFrom(newInput);
            }

            for (int i = 0; i < Outputs.Count; ++i)
            {
                ParamOutput output = Outputs[i];
                ParamOutput newOutput = newParams.Outputs[i];
                output.UpdateFrom(newOutput);
            }
        }
        #endregion
    }
}
