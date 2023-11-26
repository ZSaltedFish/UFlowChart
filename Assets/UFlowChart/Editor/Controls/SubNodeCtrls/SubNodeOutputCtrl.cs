using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    [NodeEditor(typeof(SubNodeOutput))]
    public class SubNodeOutputCtrl : FlowChartNodeCtrl
    {
        private const float _CONST_WIDTH = 100f;
        public List<Action<SubNodeOutputCtrl, Type, int>> AddNewParam = new List<Action<SubNodeOutputCtrl, Type, int>>();
        public List<Action<SubNodeOutputCtrl, int>> RemoveParam = new List<Action<SubNodeOutputCtrl, int>>();
        private SubNodeContentPanel _popupMenu;
        private float _oldParamHeight;

        public override void InitFinish()
        {
            base.InitFinish();
            _popupMenu = NodeFactoryXML.CreateEditorControl<SubNodeContentPanel>();
            _popupMenu.OnTypeSelected.Add(OnTypeSelected);

            AddNewParam.Add(FlowChartController.OnSubOutputAddParam);
            RemoveParam.Add(FlowChartController.OnSubOutputRemoveParam);
            OnSizeChange.Add(FlowChartController.OnSubNodeSizeChange);

            ParamIOStyle = new GUIStyle(EditorStyles.textField)
            {
                fontSize = 14
            };
        }

        private void OnTypeSelected(Type obj)
        {
            int index = SrcParams.Inputs.Count;
            foreach (var item in AddNewParam)
            {
                try
                {
                    item(this, obj, index);
                }
                catch (Exception err)
                {
                    Debug.LogError(err);
                }
            }
        }
        protected override void DrawParamCtrl(ParamCtrl ctrl, bool tog)
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

            if (ctrl.PType == ParamCtrlType.ParamIn)
            {
                ctrl.Label = EditorGUI.TextField(ctrl.LabelRect, ctrl.Label, ctrl.LabelStyle);
                SrcParams.Inputs[ctrl.Index].Description = ctrl.Label;
            }
            else
            {
                EditorGUI.LabelField(ctrl.LabelRect, ctrl.LabelContent, ctrl.LabelStyle);
            }

            if (ctrl.PType == ParamCtrlType.ParamIn)
            {
                Vector2 pos = ctrl.LabelRect.position + Vector2.right * (16 + ctrl.LabelRect.size.x);
                Vector2 size = Vector2.one * 16;
                Rect buttonRect = new Rect(pos, size);
                if (GUI.Button(buttonRect, "X"))
                {
                    foreach (var item in RemoveParam)
                    {
                        try
                        {
                            item(this, ctrl.Index);
                        }
                        catch (Exception err)
                        {
                            Debug.LogError(err);
                        }
                    }
                }
            }
        }

        protected override Vector2 SetRect()
        {
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
                    subWidth += inputCtrl.LockWidth(_CONST_WIDTH) + LENGTH;
                    InputRects.Add(inputCtrl);
                }
                if (i < SrcParams.Outputs.Count)
                {
                    ParamOutput output = SrcParams.Outputs[i];
                    ParamCtrl outputCtrl = new ParamCtrl(output.Description, ParamIOStyle, ParamIOOut, ParamIOIn, 16, ParamCtrlType.ParamOut, i);
                    subWidth += outputCtrl.LockWidth(_CONST_WIDTH);
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
            _oldParamHeight = ioParamHeight;
            return new Vector2(width, height + 45);
        }

        public void UpdateInputParams()
        {
            int count = SrcParams.Inputs.Count;
            Vector2 size = Size;
            size.y -= _oldParamHeight;
            _oldParamHeight = count * 21;
            size.y += _oldParamHeight;

            float subWidth = 0;
            StateStart = ParamIOStart + (count * (16 + LENGTH));
            InputRects.Clear();
            for (int i = 0; i < count; ++i)
            {
                var input = SrcParams.Inputs[i];
                ParamCtrl inputCtrl = new ParamCtrl(input.Description, ParamIOStyle, ParamIOOut, ParamIOIn, 16, ParamCtrlType.ParamIn, i);
                subWidth += inputCtrl.FastCalcWidth() + LENGTH;
                InputRects.Add(inputCtrl);
            }
            Size = size;
            CalculateRect(size);
        }

        protected override void CalculateRect(Vector2 size)
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

            for (int i = 0; i < InputRects.Count; ++i)
            {
                ParamCtrl ctrl = InputRects[i];
                float startHeight = ParamIOStart + (i * (16 + LENGTH));
                ctrl.ReCalcRectLockWidth(new Vector2(LENGTH, startHeight), _CONST_WIDTH);
            }
        }

        protected override void Draw()
        {
            base.Draw();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
            {
                EditorControlDragEventManager.Instance.PopupMenu(this, _popupMenu, Event.current.mousePosition + LocalPosition);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }

        protected override void DrawInputParams()
        {
            for (int i = 0; i < InputRects.Count; ++i)
            {
                ParamCtrl inputCtrl = InputRects[i];
                var inputData = SrcParams.Inputs[i];
                bool tog = inputData.Sources.Count > 0;
                DrawParamCtrl(inputCtrl, tog);
            }
        }
    }
}
