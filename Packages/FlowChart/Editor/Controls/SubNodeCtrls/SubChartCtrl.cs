using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public class SubChartCtrl : FlowChartNodeCtrl
    {
        private GameObject _srcPrefab;
        public GameObject SrcPrefab
        {
            get => _srcPrefab;
            set
            {
                _srcPrefab = value;
                SrcParams.HiddenInputs[0].SetStaticInput(value);
            }
        }

        protected override void InitParamNodeState()
        {
            SrcPrefab = SrcParams.HiddenInputs[0].StaticInput as GameObject;
            base.InitParamNodeState();
        }

        protected override Vector2 SetRect()
        {
            float subWidth = 0, width;
            width = TitleStyle.CalcSize(new GUIContent(SrcPrefab.name)).x;
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
                    subWidth += inputCtrl.FastCalcWidth();
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
                ctrl.ReCalcRect(new Vector2(LENGTH, startHeight));
            }

            for (int i = 0; i < OutputRects.Count; ++i)
            {
                ParamCtrl ctrl = OutputRects[i];
                float width = ctrl.FastCalcWidth();
                float startHeight = ParamIOStart + (i * (16 + LENGTH));
                ctrl.ReCalcRect(new Vector2(size.x - LENGTH - width, startHeight), false);
            }
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

        protected override void DrawTitle()
        {
            RectSize.size = LocalRect.size;
            GUI.DrawTexture(RectSize, Bg);

            _ = EditorGUILayout.GetControlRect(GUILayout.Height(40));
            Rect streamArea = EditorGUILayout.GetControlRect(GUILayout.Height(ParamIOStart - LENGTH / 2 - 40));
            _ = EditorGUILayout.GetControlRect(GUILayout.Height(StateStart - ParamIOStart));
            EditorGUI.DrawRect(TitleRect, new Color(0.2f, 0.2f, 1f, 0.5f));
            EditorGUI.DrawRect(streamArea, new Color(0.3f, 0.3f, 0.3f, 0.5f));
            EditorGUI.LabelField(TitleRect, SrcPrefab.name, TitleStyle);
        }
    }
}
