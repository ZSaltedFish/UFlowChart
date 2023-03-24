using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    [NodeEditor(typeof(SubNodeRoot))]
    public class SubNodeRootCtrl : FlowChartNodeCtrl
    {
        private const float _CONST_WIDTH = 100;
        public List<Action<SubNodeRootCtrl, Type, int>> AddNewParam = new List<Action<SubNodeRootCtrl, Type, int>>();
        public List<Action<SubNodeRootCtrl, int>> RemoveParam = new List<Action<SubNodeRootCtrl, int>>();
        private SubNodeContentPanel _popupMenu;
        private float _oldParamHeight;

        public override void InitFinish()
        {
            base.InitFinish();
            _popupMenu = NodeFactoryXML.CreateEditorControl<SubNodeContentPanel>();

            _popupMenu.OnTypeSelected.Add(OnTypeSelected);


            AddNewParam.Add(FlowChartController.OnSubRootAddParam);
            RemoveParam.Add(FlowChartController.OnSubRootRemoveParam);
            OnSizeChange.Add(FlowChartController.OnSubNodeSizeChange);
            ParamIOStyle = new GUIStyle(EditorStyles.textField)
            {
                fontSize = 14
            };
        }

        private void OnTypeSelected(Type type)
        {
            int index = SrcParams.Outputs.Count;
            foreach (var item in AddNewParam)
            {
                try
                {
                    item(this, type, index);
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

            if (ctrl.PType == ParamCtrlType.ParamOut)
            {
                ctrl.Label = EditorGUI.TextField(ctrl.LabelRect, ctrl.Label, ctrl.LabelStyle);
                SrcParams.Outputs[ctrl.Index].Description = ctrl.Label;
            }
            else
            {
                EditorGUI.LabelField(ctrl.LabelRect, ctrl.LabelContent, ctrl.LabelStyle);
            }

            if (ctrl.PType == ParamCtrlType.ParamOut)
            {
                Vector2 pos = ctrl.LabelRect.position - Vector2.right * 21;
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
                    subWidth += inputCtrl.LockWidth(_CONST_WIDTH) + LENGTH * 2 + INPUT_FIELD_LENGTH;
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

        public void UpdateOutputParams()
        {
            int count = SrcParams.Outputs.Count;
            Vector2 size = Size;
            size.y -= _oldParamHeight;
            _oldParamHeight = count * 21;
            size.y += _oldParamHeight;

            float subWidth = 0;
            StateStart = ParamIOStart + (count * (16 + LENGTH));
            OutputRects.Clear();
            for (int i = 0; i < count; ++i)
            {
                ParamOutput output = SrcParams.Outputs[i];
                ParamCtrl outputCtrl = new ParamCtrl(output.Description, ParamIOStyle, ParamIOOut, ParamIOIn, 16, ParamCtrlType.ParamOut, i);
                subWidth += outputCtrl.LockWidth(_CONST_WIDTH);
                OutputRects.Add(outputCtrl);
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

            InputParamRect = new List<Rect>();
            for (int i = 0; i < InputRects.Count; ++i)
            {
                ParamCtrl ctrl = InputRects[i];
                float startHeight = ParamIOStart + (i * (16 + LENGTH));
                ctrl.ReCalcRectLockWidth(new Vector2(LENGTH, startHeight), _CONST_WIDTH);

                Rect fieldRect = new Rect(new Vector2(LENGTH * 2 + ctrl.CtrlRect.width, startHeight), new Vector2(INPUT_FIELD_LENGTH, 16));
                InputParamRect.Add(fieldRect);
            }

            for (int i = 0; i < OutputRects.Count; ++i)
            {
                ParamCtrl ctrl = OutputRects[i];
                float width = ctrl.LockWidth(_CONST_WIDTH);
                float startHeight = ParamIOStart + (i * (16 + LENGTH));
                ctrl.ReCalcRectLockWidth(new Vector2(size.x - LENGTH - width, startHeight), _CONST_WIDTH, false);
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
    }
}
