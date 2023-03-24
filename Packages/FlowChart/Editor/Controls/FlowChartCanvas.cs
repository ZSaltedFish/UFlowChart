using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;
using static ZKnight.UFlowChart.Editor.FlowChartLinePanel;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartCanvas : EditorControl
    {
        public FlowChartModel Model => (Root as FlowChartDialog).Model;
        public FlowChartLinePanel LinePanel;
        public FlowChartNodePanel NodePanel;
        #region 控件
        public EditorImage NodePanels;
        public EditorImage DetialBackground;
        public EditorImage NodeOutputSrc;
        #endregion

        #region 位置计算参数
        public Vector2 NodeScale = Vector2.one;
        private Vector2 _canvasOffset = Vector2.zero;

        private Rect _selectRect = Rect.zero;
        private bool _selectRectBool = false;
        private Color _selectColor = new Color(0.5f, 0.5f, 1f, 0.5f);
        #endregion

        private Dictionary<int, FlowChartNodeCtrl> _nodeCtrlDict;
        private Dictionary<int, NodeSelectedCtrl> _selectedCtrlDict;
        private Dictionary<FlowChartLineID, LineConfig> _lineConfigDict;

        private int _id = 0;

        public bool IsUsingSelecting => _selectRectBool;

        public List<Action<FlowChartCanvas, float, float>> OnLineDrag = new List<Action<FlowChartCanvas, float, float>>();

        public override void InitFinish()
        {
            Context = NodeFactoryXML.CreateEditorControl<FlowChartContentPanel>();

            OnMouseDown.Add(FlowChartController.OnCanvasMouseDown);
            OnMouseUp.Add(FlowChartController.OnCanvsMouseUp);
            OnMouseDrag.Add(FlowChartController.OnCanvasMouseDrag);
            OnMouseMove.Add(FlowChartController.OnCanvasMouseMove);
            OnKeyDown.Add(FlowChartController.OnCanvasKeyDown);

            OnLineDrag.Add(FlowChartController.OnCanvasLineDragMove);
        }
        public void InitNode(NodeParams[] nodes)
        {
            _lineConfigDict = new Dictionary<FlowChartLineID, LineConfig>();
            _selectedCtrlDict = new Dictionary<int, NodeSelectedCtrl>();
            _nodeCtrlDict = new Dictionary<int, FlowChartNodeCtrl>();
            foreach (var node in nodes)
            {
                //FlowChartNodeCtrl nodeCtrl = NodeFactoryXML.CreateEditorControl<FlowChartNodeCtrl>();
                FlowChartNodeCtrl nodeCtrl = CreateNodeCtrl(node.NodeClass);
                nodeCtrl.SrcParams = node;
                nodeCtrl.SetParent(NodePanel);
                _nodeCtrlDict.Add(node.NodeID, nodeCtrl);

                NodeSelectedCtrl selectCtrl = NodeFactoryXML.CreateEditorControl<NodeSelectedCtrl>();
                selectCtrl.SrcNodeID = nodeCtrl.NodeID;
                selectCtrl.SetParent(NodePanel);
                selectCtrl.Size = nodeCtrl.Size + Vector2.one * 16f;
                selectCtrl.LocalPosition = nodeCtrl.LocalPosition - Vector2.one * 8f;
                _selectedCtrlDict.Add(node.NodeID, selectCtrl);
                _id = Mathf.Max(node.NodeID, _id);
            }
            InitLink();
        }

        public void InitLink()
        {
            List<int> keys = new List<int>(_nodeCtrlDict.Keys);
            foreach (int id in keys)
            {
                FlowChartNodeCtrl ctrl = _nodeCtrlDict[id];
                foreach (var item in ctrl.SrcParams.Streams)
                {
                    NodeParams targetParams = item.Connection;
                    if (targetParams == null) continue;
                    ConnectLine(targetParams.NodeID, 0, id, item.StreamID, LineConnectingType.StreamConnection);
                }

                foreach (var item in ctrl.SrcParams.Outputs)
                {
                    foreach (var outputTarget in item.OutputTargets)
                    {
                        ConnectLine(outputTarget.Parent.NodeID, outputTarget.InputID, item.Parent.NodeID, item.OutputID, LineConnectingType.ParamConnection);
                    }
                }
            }
        }

        public const float MOVE_VALUE = 0.1f;

        protected override void Draw()
        {
            LineConfig tempLine = Model.LineModel.TempLineConfig;
            Size = ParentControl.Size;
            if (tempLine != null)
            {
                Vector2 pos = Event.current.mousePosition - LinePanel.LocalPosition;
                tempLine.ReCalcDPosition(tempLine.StartPosition, pos);

                float xMove = 0, yMove = 0;
                if (pos.x > Size.x * (1 - MOVE_VALUE)) xMove = -1;
                if (pos.x < Size.x * MOVE_VALUE) xMove = 1;
                if (pos.y > Size.y * (1 - MOVE_VALUE)) yMove = -1;
                if (pos.y < Size.y * MOVE_VALUE) yMove = 1;

                if (Mathf.Abs(xMove) > 0.5f || Mathf.Abs(yMove) > 0.5f)
                {
                    foreach (var item in OnLineDrag)
                    {
                        item(this, xMove, yMove);
                    }
                }
            }

            if (IsUsingSelecting)
            {
                EditorGUI.DrawRect(_selectRect, _selectColor);
            }
        }

        private void GetLineConfigData(FlowChartNodeCtrl ctrl, ParamCtrl param, out Color color, out float width)
        {
            if (param.PType == ParamCtrlType.StreamIn || param.PType == ParamCtrlType.StreamOut)
            {
                color = Color.white;
                width = 5;
                return;
            }

            Type type;
            if (param.PType == ParamCtrlType.ParamIn)
            {
                type = ctrl.SrcParams.Inputs[param.Index].InputType;
            }
            else
            {
                type = ctrl.SrcParams.Outputs[param.Index].OutputType;
            }
            color = InputType2Color.GetColor(type);
            width = 3;
        }

        public override void Dispose()
        {
            base.Dispose();
            _lineConfigDict?.Clear();
            _nodeCtrlDict?.Clear();
            _selectedCtrlDict?.Clear();
        }

        public FlowChartLineID GetLineID(LineConfig selectingLine)
        {
            foreach (var item in _lineConfigDict)
            {
                if (item.Value == selectingLine)
                {
                    return item.Key;
                }
            }
            return default;
        }

        private FlowChartLineID GetLineID(int inID, int inIndex, int outID, int outIndex, LineConnectingType type)
        {
            FlowChartLineID fID = new FlowChartLineID()
            {
                LineType = type,
                InputNodeID = inID,
                OutputNodeID = outID,
                InputIndex = inIndex,
                OutputIndex = outIndex
            };
            return fID;
        }

        public FlowChartNodeCtrl GetNodeCtrl(int id)
        {
            return _nodeCtrlDict[id];
        }

        public void Clear()
        {
            if (_nodeCtrlDict == null)
            {
                return;
            }
            foreach (var item in _nodeCtrlDict)
            {
                item.Value.Dispose();
            }
            _nodeCtrlDict.Clear();

            foreach (var item in _selectedCtrlDict)
            {
                item.Value.Dispose();
            }
            _selectedCtrlDict.Clear();

            foreach (var item in _lineConfigDict)
            {
                item.Value.Dispose();
            }
            _lineConfigDict.Clear();
        }

        #region 操作
        /// <summary>
        /// 设置节点是选中
        /// </summary>
        /// <param name="id">ID</param>
        public void SetNodeSelect(int id)
        {
            var item = _selectedCtrlDict[id];
            item.IsSelected = true;
        }

        /// <summary>
        /// 设置节点是选中
        /// </summary>
        /// <param name="nodeCtrl">选中节点</param>
        public void SetNodeSelect(FlowChartNodeCtrl nodeCtrl)
        {
            int id = nodeCtrl.NodeID;
            SetNodeSelect(id);
        }

        /// <summary>
        /// 取消节点选中
        /// </summary>
        /// <param name="id">ID</param>
        public void SetNodeUnselect(int id)
        {
            var item = _selectedCtrlDict[id];
            item.IsSelected = false;
        }

        /// <summary>
        /// 取消节点选中
        /// </summary>
        /// <param name="nodeCtrl">选中节点</param>
        public void SetnodeUnselect(FlowChartNodeCtrl nodeCtrl)
        {
            int id = nodeCtrl.NodeID;
            SetNodeUnselect(id);
        }

        /// <summary>
        /// 鼠标悬停于节点
        /// </summary>
        /// <param name="id">ID</param>
        public void SetNodeHover(int id)
        {
            var item = _selectedCtrlDict[id];
            item.IsOutline = true;
        }

        /// <summary>
        /// 鼠标悬停于节点
        /// </summary>
        /// <param name="nodeCtrl">节点</param>
        public void SetNodeHover(FlowChartNodeCtrl nodeCtrl)
        {
            int id = nodeCtrl.NodeID;
            SetNodeHover(id);
        }

        /// <summary>
        /// 取消节点鼠标悬停
        /// </summary>
        /// <param name="id">ID</param>
        public void SetNodeUnhover(int id)
        {
            var item = _selectedCtrlDict[id];
            item.IsOutline = false;
        }

        /// <summary>
        /// 取消节点鼠标悬停
        /// </summary>
        /// <param name="nodeCtrl">节点</param>
        public void SetNodeUnhover(FlowChartNodeCtrl nodeCtrl)
        {
            int id = nodeCtrl.NodeID;
            SetNodeUnhover(id);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="id">ID</param>
        public void DeleteNode(int id)
        {
            FlowChartNodeCtrl nodeCtrl = _nodeCtrlDict[id];
            DeleteNode(nodeCtrl);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeCtrl">删除节点</param>
        public void DeleteNode(FlowChartNodeCtrl nodeCtrl)
        {
            List<FlowChartLineID> keys = GetLineIDWithNodeID(nodeCtrl.NodeID);
            foreach (FlowChartLineID key in keys)
            {
                DeleteLine(key);
            }
            RemoveSelectCtrl(nodeCtrl.NodeID);
            nodeCtrl.Dispose();
            _nodeCtrlDict.Remove(nodeCtrl.NodeID);
        }

        /// <summary>
        /// 删除连线
        /// </summary>
        /// <param name="inID"></param>
        /// <param name="inIndex"></param>
        /// <param name="outID"></param>
        /// <param name="outIndex"></param>
        /// <param name="type"></param>
        public void DeleteLine(int inID, int inIndex, int outID, int outIndex, LineConnectingType type)
        {
            FlowChartLineID lineID = GetLineID(inID, inIndex, outID, outIndex, type);
            DeleteLine(lineID);
        }

        /// <summary>
        /// 删除连线
        /// </summary>
        /// <param name="lineID">连线ID</param>
        public void DeleteLine(FlowChartLineID lineID)
        {
            LineConfig config = _lineConfigDict[lineID];

            FlowChartNodeCtrl nodeInput = GetNodeCtrl(lineID.InputNodeID);
            FlowChartNodeCtrl nodeOutput = GetNodeCtrl(lineID.OutputNodeID);
            if (lineID.LineType == LineConnectingType.StreamConnection)
            {
                ParamStream outputStream = nodeOutput.SrcParams.Streams[lineID.OutputIndex];
                nodeOutput.DisconnectNode(outputStream);
            }
            else
            {
                ParamInput input = nodeInput.SrcParams.Inputs[lineID.InputIndex];
                ParamOutput output = nodeOutput.SrcParams.Outputs[lineID.OutputIndex];
                nodeOutput.DisconnectInput(input, output);
            }

            config.Dispose();
            _lineConfigDict.Remove(lineID);
        }

        /// <summary>
        /// 尝试获取连线ID
        /// </summary>
        /// <param name="pos">鼠标位置(Canvas坐标)</param>
        /// <param name="lineID">连线ID</param>
        /// <returns>结果</returns>
        public bool TryGetLineID(Vector2 pos, out FlowChartLineID lineID)
        {
            foreach (var item in _lineConfigDict)
            {
                if (item.Value.DistanceFromLine(pos) < item.Value.Width)
                {
                    lineID = item.Key;
                    return true;
                }
            }
            lineID = default;
            return false;
        }

        /// <summary>
        /// 获取Line
        /// </summary>
        /// <param name="lineID">LineID</param>
        /// <returns></returns>
        public LineConfig GetLine(FlowChartLineID lineID)
        {
            return _lineConfigDict[lineID];
        }

        /// <summary>
        /// 设置选择框
        /// </summary>
        /// <param name="p">开始位置</param>
        public void SetSelectRect(Vector2 p)
        {
            _selectRectBool = true;
            _selectRect = new Rect(p - LocalPosition, Vector2.zero);
        }

        /// <summary>
        /// 完成选择框
        /// </summary>
        public void FinishSelectRect()
        {
            _selectRectBool = false;
        }

        /// <summary>
        /// 通过选择框获取包含节点
        /// </summary>
        /// <returns>包含节点数组</returns>
        public List<FlowChartNodeCtrl> GetRectSelectedCtrls()
        {
            List<FlowChartNodeCtrl> list = new List<FlowChartNodeCtrl>();
            Vector2 pos = new Vector2(Mathf.Min(_selectRect.xMin, _selectRect.xMax), Mathf.Min(_selectRect.yMin, _selectRect.yMax));
            Vector2 size = new Vector2(Mathf.Abs(_selectRect.width), Mathf.Abs(_selectRect.height));
            Rect rect = new Rect(pos, size);

            List<int> keys = new List<int>(_nodeCtrlDict.Keys);
            foreach (int key in keys)
            {
                var ctrl = _nodeCtrlDict[key];
                if (rect.Overlaps(ctrl.LocalRect))
                {
                    list.Add(ctrl);
                }
            }
            FinishSelectRect();
            return list;
        }

        /// <summary>
        /// 获取连线引用
        /// </summary>
        /// <returns>Result</returns>
        public Dictionary<FlowChartLineID, LineConfig> GetLineDict()
        {
            return _lineConfigDict;
        }

        /// <summary>
        /// 尝试通过节点获取相关连线
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <returns>相关连线</returns>
        public List<FlowChartLineID> GetLineWidthNode(int id)
        {
            List<FlowChartLineID> keys = new List<FlowChartLineID>(_lineConfigDict.Keys);
            List<FlowChartLineID> result = new List<FlowChartLineID>();
            foreach (var item in keys)
            {
                if (item.InputNodeID == id || item.OutputNodeID == id)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 画临时线
        /// </summary>
        /// <param name="ctrl">起点节点</param>
        /// <param name="param">起点参数</param>
        public void DrawTempLine(FlowChartNodeCtrl ctrl, ParamCtrl param)
        {
            Vector2 start = param.LinePoint + ctrl.LocalPosition;
            Vector2 end = Event.current.mousePosition;
            GetLineConfigData(ctrl, param, out Color color, out float width);
            Model.LineModel.TempLineConfig = LinePanel.DrawLine(start, end, color, color, width);
        }

        /// <summary>
        /// 移除临时连线
        /// </summary>
        public void RemoveTempLine()
        {
            if (Model.LineModel.TempLineConfig == null)
            {
                return;
            }
            Model.LineModel.TempLineUsing = false;
            Model.LineModel.TempLineConfig.Dispose();
            Model.LineModel.TempLineConfig = null;
        }

        /// <summary>
        /// 连接连线
        /// </summary>
        /// <param name="inID"></param>
        /// <param name="inIndex"></param>
        /// <param name="outID"></param>
        /// <param name="outIndex"></param>
        /// <param name="type"></param>
        public void ConnectLine(int inID, int inIndex, int outID, int outIndex, LineConnectingType type)
        {
            FlowChartLineID fID = GetLineID(inID, inIndex, outID, outIndex, type);
            FlowChartNodeCtrl outputNode = _nodeCtrlDict[outID];
            FlowChartNodeCtrl inputNode = _nodeCtrlDict[inID];
            Vector2 start, end;
            Color startColor, endColor;
            float width;
            switch (type)
            {
                case LineConnectingType.StreamConnection:
                    start = outputNode.GetOutputStreamLinePoint(outIndex) + outputNode.LocalPosition;
                    end = inputNode.GetInputStreamLinePoint() + inputNode.LocalPosition;
                    startColor = endColor = Color.white;
                    width = 5;
                    break;
                case LineConnectingType.ParamConnection:
                    ParamCtrl outCtrl = outputNode.GetOutputParamCtrl(outIndex);
                    ParamCtrl inCtrl = inputNode.GetInputParamCtrl(inIndex);
                    start = outCtrl.LinePoint + outputNode.LocalPosition;
                    end = inCtrl.LinePoint + inputNode.LocalPosition;
                    GetLineConfigData(outputNode, outCtrl, out startColor, out _);
                    GetLineConfigData(inputNode, inCtrl, out endColor, out width);
                    break;
                default:
                    throw new Exception("没有这个分类");
            }
            LineConfig config = LinePanel.DrawLine(start, end, startColor, endColor, width);
            _lineConfigDict.Add(fID, config);
        }

        /// <summary>
        /// 创建Node
        /// </summary>
        /// <param name="type">节点类型</param>
        /// <param name="pos">位置</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public NodeParams CreateNode(Type type, Vector2 pos, string path)
        {
            NodeParams nodeParams = new NodeParams(++_id, type, path)
            {
                Position = pos,
                Parent = (Root as FlowChartDialog).Manager
            };
            nodeParams.Parent.AddNodeParams(nodeParams);
            FlowChartNodeCtrl nodeCtrl = CreateNodeCtrl(nodeParams.NodeClass);
            nodeCtrl.SrcParams = nodeParams;
            nodeCtrl.SetParent(NodePanel);
            _nodeCtrlDict.Add(nodeParams.NodeID, nodeCtrl);

            NodeSelectedCtrl selectedCtrl = NodeFactoryXML.CreateEditorControl<NodeSelectedCtrl>();
            selectedCtrl.SrcNodeID = nodeCtrl.NodeID;
            selectedCtrl.SetParent(NodePanel);
            selectedCtrl.Size = nodeCtrl.Size + Vector2.one * 16f;
            selectedCtrl.LocalPosition = nodeCtrl.LocalPosition - Vector2.one * 8f;
            _selectedCtrlDict.Add(nodeParams.NodeID, selectedCtrl);
            return nodeParams;
        }

        public NodeParams CreateSubChart(string path, Vector2 pos)
        {
            GameObject prefab = SubChartDirHelper.GetSubChart()[path];

            Type type = typeof(SubChart);
            NodeParams nodeParams = new NodeParams(++_id, type, path)
            {
                Position = pos,
                Parent = (Root as FlowChartDialog).Manager
            };
            nodeParams.RefreshWidthPrefab(prefab);
            nodeParams.Parent.AddNodeParams(nodeParams);
            SubChartCtrl nodeCtrl = CreateNodeCtrl(type) as SubChartCtrl;
            nodeParams.HiddenInputs[0].SetStaticInput(prefab);
            nodeCtrl.SrcParams = nodeParams;
            nodeCtrl.SetParent(NodePanel);
            _nodeCtrlDict.Add(nodeParams.NodeID, nodeCtrl);

            NodeSelectedCtrl selectedCtrl = NodeFactoryXML.CreateEditorControl<NodeSelectedCtrl>();
            selectedCtrl.SrcNodeID = nodeCtrl.NodeID;
            selectedCtrl.SetParent(NodePanel);
            selectedCtrl.Size = nodeCtrl.Size + Vector2.one * 16f;
            selectedCtrl.LocalPosition = nodeCtrl.LocalPosition - Vector2.one * 8f;
            _selectedCtrlDict.Add(nodeParams.NodeID, selectedCtrl);
            return nodeParams;
        }

        /// <summary>
        /// 改变选中框大小
        /// </summary>
        /// <param name="delta"></param>
        public void SetSelectRectSize(Vector2 mp)
        {
            _selectRect.size = mp - _selectRect.position - LocalPosition;
        }

        /// <summary>
        /// 重新计算节点位置
        /// </summary>
        /// <param name="ctrl"></param>
        public void RecalculateNodePosition(FlowChartNodeCtrl ctrl)
        {
            RecalculateOutline(ctrl);
            List<FlowChartLineID> keys = new List<FlowChartLineID>(_lineConfigDict.Keys);
            foreach (FlowChartLineID key in keys)
            {
                LineConfig line = _lineConfigDict[key];
                switch (key.LineType)
                {
                    case LineConnectingType.StreamConnection:
                        if (ctrl.NodeID == key.InputNodeID) line.ReCalcDPosition(line.StartPosition, ctrl.GetInputStreamLinePoint() + ctrl.LocalPosition);
                        if (ctrl.NodeID == key.OutputNodeID) line.ReCalcDPosition(ctrl.GetOutputStreamLinePoint(key.OutputIndex) + ctrl.LocalPosition, line.EndPosition);
                        break;
                    case LineConnectingType.ParamConnection:
                        if (ctrl.NodeID == key.InputNodeID) line.ReCalcDPosition(line.StartPosition, ctrl.GetInputParamLinePoint(key.InputIndex) + ctrl.LocalPosition);
                        if (ctrl.NodeID == key.OutputNodeID) line.ReCalcDPosition(ctrl.GetOutputParamLinePoint(key.OutputIndex) + ctrl.LocalPosition, line.EndPosition);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 重新计算节点大小
        /// </summary>
        /// <param name="nodeCtrl">选中节点</param>
        public void RecalculateNodeSize(FlowChartNodeCtrl nodeCtrl)
        {
            NodeSelectedCtrl selectCtrl = _selectedCtrlDict[nodeCtrl.NodeID];
            selectCtrl.Size = nodeCtrl.Size + Vector2.one * 16f;
        }
        #endregion

        #region 辅助方法
        private List<FlowChartLineID> GetLineIDWithNodeID(int nodeID)
        {
            List<FlowChartLineID> ids = new List<FlowChartLineID>();
            foreach (FlowChartLineID key in _lineConfigDict.Keys)
            {
                if (nodeID == key.InputNodeID || nodeID == key.OutputNodeID)
                {
                    ids.Add(key);
                }
            }
            return ids;
        }

        private void RemoveSelectCtrl(int nodeID)
        {
            NodeSelectedCtrl ctrl = _selectedCtrlDict[nodeID];
            _selectedCtrlDict.Remove(nodeID);
            ctrl.Dispose();
        }

        private void RecalculateOutline(FlowChartNodeCtrl ctrl)
        {
            if (_selectedCtrlDict.TryGetValue(ctrl.NodeID, out NodeSelectedCtrl selectCtrl))
            {
                selectCtrl.LocalPosition = ctrl.LocalPosition - Vector2.one * 8f;
            }
        }

        private static Dictionary<Type, Type> _NODE_TYPE_DICT = new Dictionary<Type, Type>()
        {
            {typeof(SubNodeRoot), typeof(SubNodeRootCtrl) },
            {typeof(SubNodeOutput), typeof(SubNodeOutputCtrl) },
            {typeof(SubChart) , typeof(SubChartCtrl)}
        };

        private FlowChartNodeCtrl CreateNodeCtrl(Type type)
        {
            if (_NODE_TYPE_DICT.TryGetValue(type, out Type result))
            {
                return NodeFactoryXML.CreateEditorControl(result) as FlowChartNodeCtrl;
            }
            return NodeFactoryXML.CreateEditorControl<FlowChartNodeCtrl>();
        }
        #endregion
    }
}
