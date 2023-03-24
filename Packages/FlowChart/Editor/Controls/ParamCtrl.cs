using System;
using UnityEngine;

namespace ZKnight.UFlowChart.Editor
{
    public class ParamCtrl
    {
        public int Index { get; private set; }
        public ParamCtrlType PType { get; private set; }
        public Rect IconRect { get; private set; }
        public Rect LabelRect { get; private set; }
        public Rect CtrlRect { get; private set; }
        public bool InputMode { get; private set; }
        public Vector2 Offset { get; private set; }
        public Vector2 LinePoint { get; private set; }
        public string Label
        {
            get => _content.text;
            set
            {
                _content = new GUIContent(value);
            }
        }
        public GUIContent LabelContent => _content;

        public Texture2D IconOutTexture;
        public Texture2D IconInTexture;

        public GUIStyle LabelStyle;
        public float Distance = 5;

        private float _height;
        private GUIContent _content;

        public ParamCtrl()
        {

        }

        public ParamCtrl(string label, GUIStyle style, Texture2D @out, Texture2D @in, float height, ParamCtrlType type, int index)
        {
            Index = index;
            PType = type;
            Label = label;
            LabelStyle = new GUIStyle(style);
            IconOutTexture = @out;
            IconInTexture = @in;
            _height = height;

            switch (type)
            {
                case ParamCtrlType.StreamIn:
                    LabelStyle.alignment = TextAnchor.MiddleLeft;
                    break;
                case ParamCtrlType.StreamOut:
                    LabelStyle.alignment = TextAnchor.MiddleRight;
                    break;
                case ParamCtrlType.ParamIn:
                    LabelStyle.alignment = TextAnchor.MiddleLeft;
                    break;
                case ParamCtrlType.ParamOut:
                    LabelStyle.alignment = TextAnchor.MiddleRight;
                    break;
                default:
                    break;
            }
        }

        public void ReCalcRect(Vector2 offset, bool inputMode = true)
        {
            Offset = offset;
            InputMode = inputMode;
            _content = new GUIContent(Label);
            Vector2 iconSize = new Vector2(_height, _height);
            Vector2 labelSize = LabelStyle.CalcSize(_content);
            float height = iconSize.y;
            labelSize.y = height;

            Vector2 iconPos, labelPos;
            if (inputMode)
            {
                iconPos = offset;
                labelPos = new Vector2(iconSize.x + Distance, 0) + offset;
            }
            else
            {
                labelPos = offset;
                iconPos = new Vector2(labelSize.x + Distance, 0) + offset;
            }

            IconRect = new Rect(iconPos, iconSize);
            LabelRect = new Rect(labelPos, labelSize);
            CtrlRect = new Rect(offset, new Vector2(iconSize.x + labelSize.x + Distance, height));

            float halfWidth = CtrlRect.width / 2;
            float offX = inputMode ? -halfWidth : halfWidth;
            LinePoint = CtrlRect.center + new Vector2(offX, 0);
        }

        public void ReCalcRectLockWidth(Vector2 offset, float lockWidth, bool inputMode = true)
        {
            Offset = offset;
            InputMode = inputMode;
            _content = new GUIContent(Label);
            Vector2 iconSize = new Vector2(_height, _height);
            Vector2 labelSize = LabelStyle.CalcSize(_content);
            labelSize.x = lockWidth;
            float height = iconSize.y;
            labelSize.y = height;

            Vector2 iconPos, labelPos;
            if (inputMode)
            {
                iconPos = offset;
                labelPos = new Vector2(iconSize.x + Distance, 0) + offset;
            }
            else
            {
                labelPos = offset;
                iconPos = new Vector2(labelSize.x + Distance, 0) + offset;
            }

            IconRect = new Rect(iconPos, iconSize);
            LabelRect = new Rect(labelPos, labelSize);
            CtrlRect = new Rect(offset, new Vector2(iconSize.x + labelSize.x + Distance, height));

            float halfWidth = CtrlRect.width / 2;
            float offX = inputMode ? -halfWidth : halfWidth;
            LinePoint = CtrlRect.center + new Vector2(offX, 0);
        }

        public float FastCalcWidth()
        {
            return _height + Distance + LabelStyle.CalcSize(_content).x;
        }

        public float LockWidth(float lockWidth)
        {
            return _height + Distance + lockWidth;
        }
    }
}
