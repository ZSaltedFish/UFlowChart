using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;
using static ZKnight.UFlowChart.Editor.FlowChartLinePanel;

namespace ZKnight.UFlowChart.Editor
{
    public static class FlowChartController
    {
        #region 选中
        public static void OnNodeMouseDown(EditorControl arg1, EditorEvent arg2)
        {
            if (arg2.Event.button != 0)
            {
                return;
            }
            FlowChartNodeCtrl nodeCtrl = arg1 as FlowChartNodeCtrl;

            Vector2 local = arg2.Event.mousePosition - nodeCtrl.Rect.position;
            if (nodeCtrl.TryGetParamCtrlWithPosition(local, out ParamCtrl param) && !param.InputMode)
            {
                nodeCtrl.Model.SelectModel.ReleaseSelected();
                nodeCtrl.ParentCanvas.DrawTempLine(nodeCtrl, param);
                nodeCtrl.Model.LineModel.OutParamCtrl = param;
                nodeCtrl.Model.LineModel.TempLineUsing = true;
                nodeCtrl.Model.LineModel.OutNode = nodeCtrl;
            }
            else
            {
                FlowChartSelectModel model = nodeCtrl.Model.SelectModel;
                if (!model.SelectedNodes.Contains(nodeCtrl))
                {
                    nodeCtrl.Model.SelectModel.ReleaseSelected();
                    nodeCtrl.Model.SelectModel.SetSelectedNode(nodeCtrl);
                }
            }
            arg2.Use();
        }

        public static void OnCanvasMouseDown(EditorControl arg1, EditorEvent arg2)
        {
            if (arg2.Event.button != 0)
            {
                return;
            }
            FlowChartCanvas canvas = arg1 as FlowChartCanvas;
            FlowChartSelectModel selectModel = canvas.Model.SelectModel;
            selectModel.ReleaseSelected();

            if (canvas.TryGetLineID(arg2.Event.mousePosition - canvas.LocalPosition, out FlowChartLineID id))
            {
                LineConfig line = canvas.GetLine(id);
                selectModel.SetSelectedLine(line);
            }
            else
            {
                canvas.SetSelectRect(arg2.Event.mousePosition);
            }
            arg2.Use();
        }

        public static void OnCanvsMouseUp(EditorControl arg1, EditorEvent arg2)
        {
            FlowChartCanvas canvas = arg1 as FlowChartCanvas;
            if (canvas.IsUsingSelecting)
            {
                FlowChartSelectModel selectMode = canvas.Model.SelectModel;
                selectMode.ReleaseSelected();
                foreach (var item in canvas.GetRectSelectedCtrls())
                {
                    selectMode.SetSelectedNode(item);
                }
                arg2.Use();
            }

            if (canvas.Model.LineModel.TempLineUsing)
            {
                canvas.RemoveTempLine();
                arg2.Use();
            }
        }
        #endregion

        #region 拖拽
        public static void OnNodePanelMouseMove(EditorControl arg1, EditorEvent arg2)
        {
            if (arg2.Event.button != 0)
            {
                return;
            }

            FlowChartSelectModel model = (arg1.Root as FlowChartDialog).Model.SelectModel;
            if (model.SelectedNodes.Count == 0)
            {
                return;
            }

            foreach (FlowChartNodeCtrl item in model.SelectedNodes)
            {
                item.LocalPosition += arg2.Event.delta;
            }
            arg2.Use();
        }

        public static void OnCanvasMouseDrag(EditorControl arg1, EditorEvent arg2)
        {
            if (arg2.Event.button == 0)     //鼠标左键
            {
                FlowChartCanvas canvas = arg1 as FlowChartCanvas;
                if (canvas.IsUsingSelecting)
                {
                    canvas.SetSelectRectSize(arg2.Event.mousePosition);
                }
            }

            if (arg2.Event.button == 2)     // 鼠标中键
            {
                FlowChartCanvas canvas = arg1 as FlowChartCanvas;
                canvas.Model.SelectModel.ReleaseSelected();
                Vector2 delta = arg2.Event.delta;
                foreach (EditorControl ctrl in canvas.NodePanel.FirstChildrenList)
                {
                    if (ctrl is FlowChartNodeCtrl)
                    {
                        ctrl.LocalPosition += delta;
                    }
                }
            }
        }

        public static void OnNodeMouseUp(EditorControl arg1, EditorEvent arg2)
        {
            FlowChartNodeCtrl nodeCtrl = arg1 as FlowChartNodeCtrl;
            if (!nodeCtrl.ParentCanvas.IsUsingSelecting)
            {
                arg2.Use();
            }
            if (arg2.Event.button != 0)
            {
                return;
            }
            TempLineModel lineModel = nodeCtrl.Model.LineModel;
            Vector2 local = arg2.Event.mousePosition - arg1.Rect.position;
            if (lineModel.TempLineUsing && nodeCtrl.TryGetParamCtrlWithPosition(local, out ParamCtrl param))
            {
                ParamCtrl outCtrl = lineModel.OutParamCtrl;
                FlowChartNodeCtrl outNode = lineModel.OutNode;
                if (param.PType == outCtrl.PType - 1 && lineModel.OutNode != nodeCtrl)
                {
                    if (outCtrl.PType == ParamCtrlType.StreamOut)
                    {
                        ParamStream outputStream = outNode.SrcParams.Streams[outCtrl.Index];
                        NodeParams inputNode = outputStream.Connection;
                        if (inputNode != null)
                        {
                            nodeCtrl.ParentCanvas.DeleteLine(inputNode.NodeID, 0, outNode.NodeID, outCtrl.Index, LineConnectingType.StreamConnection);
                        }
                        nodeCtrl.ParentCanvas.ConnectLine(nodeCtrl.NodeID, 0, outNode.NodeID, outCtrl.Index, LineConnectingType.StreamConnection);
                        outNode.SetStreamConnect(outCtrl, nodeCtrl);
                    }

                    if (outCtrl.PType == ParamCtrlType.ParamOut)
                    {
                        ParamOutput paramOutput = outNode.SrcParams.Outputs[outCtrl.Index];
                        ParamInput paramInput = nodeCtrl.SrcParams.Inputs[param.Index];
                        if (!paramOutput.OutputTargets.Contains(paramInput))
                        {
                            if (ParamTypeMatch.IsMatch(paramOutput.OutputType, paramInput.InputType))
                            {
                                nodeCtrl.ParentCanvas.ConnectLine(nodeCtrl.NodeID, paramInput.InputID, outNode.NodeID, paramOutput.OutputID, LineConnectingType.ParamConnection);
                                outNode.SetInputConnect(nodeCtrl.SrcParams, paramOutput, paramInput);
                            }

                            if (TryOverloadNode(nodeCtrl, out NodeParams newParams))
                            {
                                OverloadNode(nodeCtrl, newParams);
                            }
                        }
                    }
                }
            }
            nodeCtrl.ParentCanvas.RemoveTempLine();
        }

        public static void OnCanvasLineDragMove(FlowChartCanvas canvas, float x, float y)
        {
            Vector2 delta = new Vector2(x, y) * 10f;
            foreach (EditorControl ctrl in canvas.NodePanel.FirstChildrenList)
            {
                if (ctrl is FlowChartNodeCtrl)
                {
                    ctrl.LocalPosition += delta;
                }
            }
            canvas.Model.LineModel.TempLineConfig.Offset(delta);
        }
        #endregion

        #region 鼠标悬停
        public static void OnMouseIn(EditorControl arg1, EditorEvent arg2)
        {
            FlowChartNodeCtrl nodeCtrl = arg1 as FlowChartNodeCtrl;
            nodeCtrl.Model.HoverModel.Release();
            nodeCtrl.Model.HoverModel.SetHover(nodeCtrl);
        }

        public static void OnMouseOut(EditorControl arg1, EditorEvent arg2)
        {
            FlowChartNodeCtrl nodeCtrl = arg1 as FlowChartNodeCtrl;
            if (nodeCtrl.ParentControl == null)
            {
                return;
            }
            if (nodeCtrl == nodeCtrl.Model.HoverModel.Hovering)
            {
                nodeCtrl.Model.HoverModel.Release();
            }
        }

        public static void OnCanvasMouseMove(EditorControl arg1, EditorEvent arg2)
        {
            FlowChartCanvas canvas = arg1 as FlowChartCanvas;
            FlowChartHoverModel hoverModel = canvas.Model.HoverModel;
            
            if (canvas.TryGetLineID(arg2.Event.mousePosition - canvas.LocalPosition, out FlowChartLineID id))
            {
                LineConfig line = canvas.GetLine(id);
                if (hoverModel.Hovering != line)
                {
                    hoverModel.Release();
                    hoverModel.SetHover(line);
                }
            }
            else
            {
                if (hoverModel.Hovering != null && hoverModel.Hovering is LineConfig)
                {
                    hoverModel.Release();
                }
            }
        }
        #endregion

        #region 位置改变
        public static void OnNodePositionChange(EditorControl obj)
        {
            FlowChartNodeCtrl nodeCtrl = obj as FlowChartNodeCtrl;
            if (nodeCtrl.ParentControl != null)
            {
                nodeCtrl.ParentCanvas.RecalculateNodePosition(nodeCtrl);
                nodeCtrl.SrcParams.Position = nodeCtrl.LocalPosition;
            }
        }
        #endregion

        #region 按键控制
        public static void OnCanvasKeyDown(EditorControl arg1, EditorEvent arg2)
        {
            FlowChartCanvas canvas = arg1 as FlowChartCanvas;
            switch (arg2.Event.keyCode)
            {
                case KeyCode.Delete:
                    FlowChartSelectModel selectModel = canvas.Model.SelectModel;
                    if (selectModel.SelectedLine != null)
                    {
                        selectModel.SelectedLine.Delete();
                        canvas.Model.ObjectDeleted(selectModel.SelectedLine);
                    }

                    List<IFlowChartOperator> list = new List<IFlowChartOperator>(selectModel.SelectedNodes);
                    foreach (var item in list)
                    {
                        item.Delete();
                        canvas.Model.ObjectDeleted(item);
                    }
                    arg2.Use();
                    break;
                case KeyCode.S:
                    if (arg2.Event.control)
                    {
                        (arg1.Root as FlowChartDialog).Save();
                    }
                    break;
                default:
                    break;
            }
        }

        public static void CreateNewNode(FlowChartDialog root, Type arg1, Vector2 arg2, string path)
        {
            root.FCanvas.CreateNode(arg1, arg2, path);
        }

        public static void CreateSubNode(FlowChartDialog root, string sub, Vector2 vector2)
        {
            root.FCanvas.CreateSubChart(sub, vector2);
        }
        #endregion

        #region 打开界面
        [MenuItem("Assets/Flow Chart")]
        public static void Init()
        {
            FlowChartDialog dialog = ScriptableObject.CreateInstance<FlowChartDialog>();
            if (Selection.gameObjects.Length > 0)
            {
                GameObject go = Selection.gameObjects[0];
                dialog.OpenPrefab(go);
            }
            else
            {
                dialog.CreateNew();
            }

            dialog.Show();
        }

        public static void Open(GameObject go)
        {
            FlowChartDialog dialog = ScriptableObject.CreateInstance<FlowChartDialog>();
            dialog.OpenPrefab(go);
            dialog.Show();
        }
        #endregion

        #region 子图参数
        public static void OnSubRootAddParam(SubNodeRootCtrl ctrl, Type type, int index)
        {
            NodeParams nodeParams = ctrl.SrcParams;
            nodeParams.AddOutput(index, type, $"Output_{index}");
            ctrl.UpdateOutputParams();
        }

        public static void OnSubRootRemoveParam(SubNodeRootCtrl ctrl, int index)
        {
            FlowChartCanvas canvas = ctrl.ParentCanvas;
            List<FlowChartLineID> reconnects = new List<FlowChartLineID>();
            foreach (var item in canvas.GetLineWidthNode(ctrl.NodeID))
            {
                if (item.LineType == LineConnectingType.StreamConnection)
                {
                    continue;
                }
                if (item.OutputIndex == index)
                {
                    canvas.GetLine(item).Delete();
                }

                if (item.OutputIndex > index)
                {
                    reconnects.Add(item);
                    canvas.GetLine(item).Delete();
                }
            }

            NodeParams nodeParams = ctrl.SrcParams;
            nodeParams.RemoveOutput(index);
            ctrl.UpdateOutputParams();

            foreach (var item in reconnects)
            {
                int newIndex = item.OutputIndex - 1;
                int inputIndex = item.InputIndex;
                FlowChartNodeCtrl inNode = canvas.GetNodeCtrl(item.InputNodeID);
                canvas.ConnectLine(item.InputNodeID, inputIndex, ctrl.NodeID, newIndex, LineConnectingType.ParamConnection);
                ctrl.SetInputConnect(inNode.SrcParams, ctrl.SrcParams.Outputs[newIndex], inNode.SrcParams.Inputs[inputIndex]);
            }
        }

        public static void OnSubOutputAddParam(SubNodeOutputCtrl arg1, Type arg2, int arg3)
        {
            NodeParams nodeParams = arg1.SrcParams;
            nodeParams.AddInput(arg3, arg2, $"Output_{arg3}");
            arg1.UpdateInputParams();
        }

        public static void OnSubOutputRemoveParam(SubNodeOutputCtrl arg1, int arg2)
        {
            FlowChartCanvas canvas = arg1.ParentCanvas;
            List<FlowChartLineID> reconnects = new List<FlowChartLineID>();
            foreach (var item in canvas.GetLineWidthNode(arg1.NodeID))
            {
                if (item.LineType == LineConnectingType.StreamConnection)
                {
                    continue;
                }

                if (item.InputIndex == arg2)
                {
                    canvas.GetLine(item).Delete();
                }

                if (item.InputIndex > arg2)
                {
                    reconnects.Add(item);
                    canvas.GetLine(item).Delete();
                }
            }

            NodeParams nodeParams = arg1.SrcParams;
            nodeParams.RemoveInput(arg2);
            arg1.UpdateInputParams();

            foreach (var item in reconnects)
            {
                int newInputIndex = item.InputIndex - 1;
                int outputIndex = item.OutputIndex;
                FlowChartNodeCtrl outNode = canvas.GetNodeCtrl(item.OutputNodeID);
                canvas.ConnectLine(arg1.NodeID, newInputIndex, outNode.NodeID, outputIndex, LineConnectingType.ParamConnection);
                outNode.SetInputConnect(arg1.SrcParams, outNode.SrcParams.Outputs[outputIndex], arg1.SrcParams.Inputs[newInputIndex]);
            }
        }

        public static void OnSubNodeSizeChange(EditorControl arg1, Rect arg2)
        {
            FlowChartNodeCtrl nodeCtrl = arg1 as FlowChartNodeCtrl;
            if (nodeCtrl.ParentControl != null)
            {
                nodeCtrl.ParentCanvas.RecalculateNodeSize(nodeCtrl);
            }
        }
        #endregion

        #region 节点重载
        public static void OverloadNode(FlowChartNodeCtrl nodeCtrl, NodeParams newParams)
        {
            FlowChartCanvas canvas = nodeCtrl.ParentCanvas;
            NodeParams srcParams = nodeCtrl.SrcParams;

            srcParams.UpdateFrom(newParams);
            nodeCtrl.SrcParams = nodeCtrl.SrcParams;

            List<FlowChartLineID> lineIDs = canvas.GetLineWidthNode(nodeCtrl.NodeID);
            foreach (FlowChartLineID id in lineIDs)
            {
                if (id.InputNodeID == nodeCtrl.NodeID && id.LineType == LineConnectingType.ParamConnection)
                {
                    ParamInput input = nodeCtrl.SrcParams.Inputs[id.InputIndex];
                    LineConfig config = canvas.GetLine(id);
                    config.EndColor = InputType2Color.GetColor(input.InputType);
                }
            }
        }

        public static bool TryOverloadNode(FlowChartNodeCtrl nodeCtrl, out NodeParams newParams)
        {
            newParams = null;
            List<ParamInput> inputs = nodeCtrl.SrcParams.Inputs;
            int count = 0;
            List<Type> types = new List<Type>();

            int index = -1;
            Type pType = null;
            for (int i = 0; i < inputs.Count; ++i)
            {
                ParamInput input = inputs[i];
                if (input.IsDynamicInput)
                {
                    index = i;
                    pType = input.Sources[0].OutputType;
                    types.Add(input.Sources[0].OutputType);
                    ++count;
                }
                else
                {
                    types.Add(input.InputType);
                }
            }
            if (count != 1) return false;

            return NodeOverloadManager.INSTANCE.TryOverloadNode(nodeCtrl.SrcParams.Path, out newParams, pType, index, types.Count);
        }
        #endregion
    }
}
