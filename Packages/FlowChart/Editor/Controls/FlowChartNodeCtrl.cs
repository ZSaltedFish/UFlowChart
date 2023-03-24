using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartNodeCtrl : EditorControl, IFlowChartOperator
    {
        public FlowChartModel Model => RootDialog.Model;
        public FlowChartCanvas ParentCanvas => ParentControl.ParentControl as FlowChartCanvas;
        public FlowChartDialog RootDialog => Root as FlowChartDialog;

        public static float INPUT_FIELD_LENGTH = FieldType2Function.FIELD_WIDTH;
        public const float LENGTH = 10;
        public NodeParams SrcParams
        {
            get
            {
                return _nodeParams;
            }

            set
            {
                _nodeParams = value;
                UpdateStyle();
            }
        }

        public int NodeID { get; set; }

        public Color TitleColor = ROOT_TITLE_COLOR;
        public Color RootTitleColor;
        public Color SubTitleColor;

        #region 参数贴图
        public Texture2D StreamBg;
        public Texture2D StreamTog;

        public Texture2D ParamIOOut;
        public Texture2D ParamIOIn;
        #endregion

        protected Texture2D Bg;
        protected Rect RectSize;

        protected GUIStyle TitleStyle;
        protected GUIStyle StreamStyle;
        protected GUIStyle ParamIOStyle;
        private NodeParams _nodeParams;

        private readonly static Color ROOT_TITLE_COLOR = new Color(0.8f, 0.2f, 0.2f);
        private readonly static Color SUB_TITLE_COLOR = new Color(0.2f, 0.8f, 0.2f);

        private Dictionary<Type, string[]> _typeSelection;

        protected Rect TitleRect;
        protected List<Rect> InputParamRect;
        protected ParamCtrl IStreamRect;
        protected List<ParamCtrl> OutStreamRects;
        protected List<ParamCtrl> InputRects;
        protected List<ParamCtrl> OutputRects;
        protected float ParamIOStart;
        protected float StateStart;

        public override string XMLNodePath => "Packages/com.zknight.uflowchart/Editor/ControlXmls/FlowChartNodeCtrl.xml";

        public override void InitFinish()
        {
            base.InitFinish();
            Bg = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.zknight.uflowchart/Resources/NodeBg.png");
            RectSize = new Rect(Vector2.zero, LocalRect.size);
            TitleStyle = new GUIStyle()
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            StreamStyle = new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            ParamIOStyle = new GUIStyle()
            {
                fontSize = 14,
            };
            OutStreamRects = new List<ParamCtrl>();
            InputRects = new List<ParamCtrl>();
            OutputRects = new List<ParamCtrl>();

            #region 初始化事件
            OnMouseDown.Add(FlowChartController.OnNodeMouseDown);
            OnMouseUp.Add(FlowChartController.OnNodeMouseUp);
            OnMouseIn.Add(FlowChartController.OnMouseIn);
            OnMouseLeft.Add(FlowChartController.OnMouseOut);
            OnPositionChange.Add(FlowChartController.OnNodePositionChange);
            #endregion
        }

        #region 重画
        protected override void Draw()
        {
            try
            {
                DrawTitle();
                DrawIStream();
                DrawOutStreamStream();
                DrawInputParams();
                DrawOutputParams();
                DrawState();
            }
            catch (NullReferenceException err)
            {
                Debug.LogError($"Node Draw Faild:{NodeID}\n{err}");
            }
        }

        protected virtual void DrawTitle()
        {
            RectSize.size = LocalRect.size;
            GUI.DrawTexture(RectSize, Bg);

            _ = EditorGUILayout.GetControlRect(GUILayout.Height(40));
            Rect streamArea = EditorGUILayout.GetControlRect(GUILayout.Height(ParamIOStart - LENGTH / 2 - 40));
            _ = EditorGUILayout.GetControlRect(GUILayout.Height(StateStart - ParamIOStart));
            EditorGUI.DrawRect(TitleRect, TitleColor);
            EditorGUI.DrawRect(streamArea, new Color(0.3f, 0.3f, 0.3f, 0.5f));
            EditorGUI.LabelField(TitleRect, SrcParams.NodeClass.Name, TitleStyle);
        }

        protected virtual void DrawIStream()
        {
            if (IStreamRect != null)
            {
                DrawParamCtrl(IStreamRect, SrcParams.SourceStreams.Count > 0);
            }
        }

        protected virtual void DrawOutStreamStream()
        {
            for (int i = 0; i < OutStreamRects.Count; ++i)
            {
                ParamCtrl oStream = OutStreamRects[i];
                var outData = SrcParams.Streams[i];
                DrawParamCtrl(oStream, outData.Connection != null);
            }
        }

        protected virtual void DrawInputParams()
        {
            for (int i = 0; i < InputParamRect.Count; ++i)
            {
                ParamCtrl inputCtrl = InputRects[i];
                Rect inputFieldRect = InputParamRect[i];
                var inputData = SrcParams.Inputs[i];
                bool tog = inputData.Sources.Count > 0;
                DrawParamCtrl(inputCtrl, tog);
                if (!tog)
                {
                    inputData.SetStaticInput(FieldType2Function.FieldContent(inputFieldRect, inputData.InputType, inputData.StaticInput));
                }
            }
        }

        protected virtual void DrawOutputParams()
        {
            for (int i = 0; i < OutputRects.Count; ++i)
            {
                ParamCtrl outputCtrl = OutputRects[i];
                var outputData = SrcParams.Outputs[i];
                DrawParamCtrl(outputCtrl, outputData.OutputTargets.Count > 0);
            }
        }

        protected virtual void DrawState()
        {
            foreach (ParamNodeState nodeState in SrcParams.NodeStates)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                nodeState.EnumValue = EditorGUILayout.Popup(nodeState.EnumValue, _typeSelection[nodeState.NType], GUILayout.Width(120), GUILayout.Height(30));
                EditorGUILayout.EndHorizontal();
            }
        }

        protected virtual void DrawParamCtrl(ParamCtrl ctrl, bool tog)
        {
            Color color;
            if (ctrl.PType == ParamCtrlType.StreamIn || ctrl.PType == ParamCtrlType.StreamOut)
            {
                color = Color.white;
            }
            else
            {
                Type type;
                if (ctrl.PType == ParamCtrlType.ParamIn)
                {
                    type = SrcParams.Inputs[ctrl.Index].InputType;
                }
                else
                {
                    type = SrcParams.Outputs[ctrl.Index].OutputType;
                }
                color = InputType2Color.GetColor(type);
            }

            GUI.DrawTexture(ctrl.IconRect, ctrl.IconOutTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            if (tog)
            {
                GUI.DrawTexture(ctrl.IconRect, ctrl.IconInTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            }

            EditorGUI.LabelField(ctrl.LabelRect, ctrl.LabelContent, ctrl.LabelStyle);
        }
        #endregion

        #region 初始化参数
        private void UpdateStyle()
        {
            switch (SrcParams.NodeType)
            {
                case FlowChartNodeType.Root:
                    TitleColor = ROOT_TITLE_COLOR;
                    break;
                case FlowChartNodeType.Node:
                    TitleColor = SUB_TITLE_COLOR;
                    break;
                default:
                    break;
            }
            TitleColor.a = 0.5f;
            InitParamNodeState();
            LocalPosition = SrcParams.Position;
        }

        protected virtual void InitParamNodeState()
        {
            NodeID = SrcParams.NodeID;
            _typeSelection = new Dictionary<Type, string[]>();
            foreach (ParamNodeState nodeState in SrcParams.NodeStates)
            {
                if (!_typeSelection.ContainsKey(nodeState.NType))
                {
                    string[] list = Enum.GetNames(nodeState.NType);
                    _typeSelection.Add(nodeState.NType, list);
                }
            }

            Vector2 size = SetRect();
            Size = size;
            CalculateRect(size);
        }

        protected virtual Vector2 SetRect()
        {
            OutStreamRects.Clear();
            InputRects.Clear();
            OutputRects.Clear();

            float subWidth = 0, width;
            width = TitleStyle.CalcSize(new GUIContent(SrcParams.NodeClass.Name)).x;
            if (SrcParams.NodeType != FlowChartNodeType.Root)
            {
                IStreamRect = new ParamCtrl("", StreamStyle, StreamBg, StreamTog, 32, ParamCtrlType.StreamIn, 0);
                subWidth += IStreamRect.FastCalcWidth() + LENGTH;
            }
            if (SrcParams.Streams.Count > 0)
            {
                ParamStream ps = SrcParams.Streams[0];
                ParamCtrl outCtrl = new ParamCtrl(ps.Description, StreamStyle, StreamBg, StreamTog, 32, ParamCtrlType.StreamOut, 0);
                OutStreamRects.Add(outCtrl);
                subWidth += outCtrl.FastCalcWidth();
            }
            width = Mathf.Max(width, subWidth);

            for (int i = 1; i < SrcParams.Streams.Count; ++i)
            {
                ParamStream ps = SrcParams.Streams[i];
                ParamCtrl outCtrl = new ParamCtrl(ps.Description, StreamStyle, StreamBg, StreamTog, 32, ParamCtrlType.StreamOut, i);
                OutStreamRects.Add(outCtrl);
                subWidth = outCtrl.FastCalcWidth();
            }
            width = Mathf.Max(width, subWidth);

            int ioMax = Mathf.Max(1, SrcParams.Streams.Count);
            int max = Mathf.Max(SrcParams.Inputs.Count, SrcParams.Outputs.Count);
            for (int i = 0; i < max; ++i)
            {
                subWidth = 0;
                if (i < SrcParams.Inputs.Count)
                {
                    var input = SrcParams.Inputs[i];
                    ParamCtrl inputCtrl = new ParamCtrl(input.Description, ParamIOStyle, ParamIOOut, ParamIOIn, 16, ParamCtrlType.ParamIn, i);
                    subWidth += inputCtrl.FastCalcWidth() + LENGTH * 2 + INPUT_FIELD_LENGTH;
                    InputRects.Add(inputCtrl);
                }
                if (i < SrcParams.Outputs.Count)
                {
                    ParamOutput output = SrcParams.Outputs[i]; 
                    ParamCtrl outputCtrl = new ParamCtrl(output.Description, ParamIOStyle, ParamIOOut, ParamIOIn, 16, ParamCtrlType.ParamOut, i);
                    subWidth += outputCtrl.FastCalcWidth();
                    OutputRects.Add(outputCtrl);
                }
                width = Mathf.Max(width, subWidth);
            }
            width += LENGTH * 2;
            TitleRect = new Rect(Vector2.zero, new Vector2(width, 40));

            float ioStreamHeight = ioMax * 32;
            float ioParamHeight = max * 16;
            float stateHeight = SrcParams.NodeStates.Count * 30;
            float blank = (ioMax + 1 + max + SrcParams.NodeStates.Count) * LENGTH;
            float height = 40 + ioStreamHeight + ioParamHeight + stateHeight + blank;
            ParamIOStart = 40 + ioMax * 32 + (1 + ioMax) * LENGTH;
            StateStart = ParamIOStart + (max * (16 + LENGTH));

            return new Vector2(width, height);
        }

        protected virtual void CalculateRect(Vector2 size)
        {
            if (IStreamRect != null)
            {
                IStreamRect.ReCalcRect(new Vector2(LENGTH, 40 + LENGTH));
            }

            for (int i = 0; i < OutStreamRects.Count; ++i)
            {
                ParamCtrl ctrl = OutStreamRects[i];
                float width = ctrl.FastCalcWidth();
                ctrl.ReCalcRect(new Vector2(size.x - LENGTH - width, 40 + LENGTH + i * 32), false);
            }

            InputParamRect = new List<Rect>();
            for (int i = 0; i < InputRects.Count; ++i)
            {
                ParamCtrl ctrl = InputRects[i];
                float startHeight = ParamIOStart + (i * (16 + LENGTH));
                ctrl.ReCalcRect(new Vector2(LENGTH, startHeight));

                Rect fieldRect = new Rect(new Vector2(LENGTH * 2 + ctrl.CtrlRect.width, startHeight), new Vector2(INPUT_FIELD_LENGTH, 16));
                InputParamRect.Add(fieldRect);
            }

            for (int i = 0; i < OutputRects.Count; ++i)
            {
                ParamCtrl ctrl = OutputRects[i];
                float width = ctrl.FastCalcWidth();
                float startHeight = ParamIOStart + (i * (16 + LENGTH));
                ctrl.ReCalcRect(new Vector2(size.x - LENGTH - width, startHeight), false);
            }
        }
        #endregion

        #region 获取Rectangle
        public Vector2 GetInputStreamLinePoint()
        {
            return IStreamRect.LinePoint;
        }

        public Vector2 GetOutputStreamLinePoint(int index)
        {
            try
            {
                return OutStreamRects[index].LinePoint;
            }
            catch (ArgumentOutOfRangeException err)
            {
                Debug.LogError($"{index}越界->{OutStreamRects.Count}");
                throw err;
            }
        }

        public Vector2 GetInputParamLinePoint(int index)
        {
            return InputRects[index].LinePoint;
        }

        public Vector2 GetOutputParamLinePoint(int index)
        {
            return OutputRects[index].LinePoint;
        }
        #endregion

        #region 获取ParamCtrl
        /// <summary>
        /// 尝试获取选中的参数控件
        /// </summary>
        /// <param name="pos">当前控件坐标系</param>
        /// <param name="ctrl">输出参数控件</param>
        /// <returns>是否选中</returns>
        public bool TryGetParamCtrlWithPosition(Vector2 pos, out ParamCtrl ctrl)
        {
            if (IStreamRect != null && IStreamRect.CtrlRect.Contains(pos))
            {
                ctrl = IStreamRect;
                return true;
            }

            foreach (ParamCtrl paramCtrl in OutStreamRects)
            {
                if (paramCtrl.CtrlRect.Contains(pos))
                {
                    ctrl = paramCtrl;
                    return true;
                }
            }
            foreach (ParamCtrl paramCtrl in InputRects)
            {
                if (paramCtrl.CtrlRect.Contains(pos))
                {
                    ctrl = paramCtrl;
                    return true;
                }
            }
            foreach (ParamCtrl paramCtrl in OutputRects)
            {
                if (paramCtrl.CtrlRect.Contains(pos))
                {
                    ctrl = paramCtrl;
                    return true;
                }
            }

            ctrl = null;
            return false;
        }

        public ParamCtrl GetOutputParamCtrl(int index)
        {
            return OutputRects[index];
        }

        public ParamCtrl GetInputParamCtrl(int index)
        {
            return InputRects[index];
        }
        #endregion

        #region 设置连接参数
        public void SetStreamConnect(ParamCtrl outPS, FlowChartNodeCtrl ctrl)
        {
            SrcParams.ConnectNode(OutStreamRects.IndexOf(outPS), ctrl.SrcParams);
        }

        public void SetInputConnect(NodeParams inNodeParams, ParamOutput paramOutput, ParamInput paramInput)
        {
            inNodeParams.ConnectInput(SrcParams, paramOutput.OutputID, paramInput.InputID);
        }

        public void DisconnectNode(ParamStream outputStream)
        {
            SrcParams.DisconnectNode(outputStream.StreamID);
        }

        public void DisconnectInput(ParamInput inputTarget, ParamOutput output)
        {
            inputTarget.Parent.DisconnectInput(inputTarget.InputID, output);
        }
        #endregion

        public override string ToString()
        {
            return $"{SrcParams.NodeClass.Name}_{NodeID}";
        }

        public override void Dispose()
        {
            SrcParams.Parent.RemoveNodeParams(SrcParams);
            base.Dispose();
        }

        #region IFlowChartOperator
        public void SetSelected()
        {
            ParentCanvas.SetNodeSelect(this);
        }

        public void SetUnselected()
        {
            ParentCanvas.SetnodeUnselect(this);
        }

        public void SetHover()
        {
            ParentCanvas.SetNodeHover(this);
        }

        public void SetUnHover()
        {
            ParentCanvas.SetNodeUnhover(this);
        }

        public void Delete()
        {
            ParentCanvas.DeleteNode(this);
        }
        #endregion
    }
}
