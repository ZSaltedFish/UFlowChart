using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartLinePanel : EditorControl, IDisposable
    {
        public class LineConfig : IFlowChartOperator
        {
            public FlowChartLineID Key;
            public uint ID { get; private set; }
            public Vector3 StartPosition => _start;
            public Vector3 EndPosition => _end;
            public Vector3[] Points => _positions;
            public int LineCount = 10;
            public bool IsSelecting => _isSelecting;
            public bool IsHovering => _isHover;
            public Color SelectedColor => _selectingColor;
            public float Width = 5;
            public Color StartColor = Color.white, EndColor = Color.white;

            private Vector3[] _positions;
            private Vector3 _start;
            private Vector3 _end;
            private readonly FlowChartLinePanel _parent;

            private bool _isSelecting = false;
            private Color _selectingColor = new Color(1f, 0.45f, 0.24f);
            private bool _isHover;

            public FlowChartCanvas ParentCanvas => _parent.Parent as FlowChartCanvas;

            public LineConfig(uint id, FlowChartLinePanel parent)
            {
                ID = id;
                _parent = parent;
            }

            public void ReCalcDPosition(Vector3 start, Vector3 end)
            {
                _start = start;
                _end = end;
                Vector3 center = (_start + _end) / 2;
                List<Vector3> points = new List<Vector3>();
                points.AddRange(AddPoints(_start, center, true));
                points.AddRange(AddPoints(center, _end, false));
                _positions = points.ToArray();
            }

            private IEnumerable<Vector3> AddPoints(Vector3 s, Vector3 e, bool v)
            {
                List<Vector3> points = new List<Vector3>();
                Vector3 p = v ? new Vector3(e.x, s.y) : new Vector3(s.x, e.y);
                int start = v ? 0 : 1;
                for (int i = start; i <= LineCount; ++i)
                {
                    float t = Mathf.Clamp01(i / (float)LineCount);
                    float oT = 1 - t;
                    Vector3 point = oT * oT * s + 2 * oT * t * p + t * t * e;
                    points.Add(point);
                }
                return points;
            }

            public void Dispose()
            {
                _parent.Remove(this);
            }

            public float DistanceFromLine(Vector2 point)
            {
                float distance = float.MaxValue;
                for (int i = 1; i < _positions.Length; ++i)
                {
                    Vector3 pStart, pEnd;
                    pStart = _positions[i - 1];
                    pEnd = _positions[i];
                    float tempDist = HandleUtility.DistancePointLine(point, pStart, pEnd);
                    distance = Mathf.Min(distance, tempDist);
                }
                return distance;
            }

            public void Offset(Vector3 offset)
            {
                _start += offset;
                _end += offset;
                for (int i = 0; i < _positions.Length; ++i)
                {
                    _positions[i] += offset;
                }
            }

            public void SetSelecting(bool state)
            {
                _isSelecting = state;
            }

            public void SetSelected()
            {
                _isSelecting = true;
            }

            public void SetUnselected()
            {
                _isSelecting = false;
            }

            public void SetHover()
            {
                _isHover = true;
            }

            public void SetUnHover()
            {
                _isHover = false;
            }

            public void Delete()
            {
                var id = ParentCanvas.GetLineID(this);
                ParentCanvas.DeleteLine(id);
            }
        };

        private Dictionary<uint, LineConfig> _id2Array;
        private uint _ids = 0;

        public override void InitFinish()
        {
            base.InitFinish();
            _id2Array = new Dictionary<uint, LineConfig>();
        }

        protected override void Draw()
        {
            Color handleColor = Handles.color;
            List<uint> ids = new List<uint>(_id2Array.Keys);
            for (int index = 0; index < ids.Count; ++index)
            {
                uint key = ids[index];
                LineConfig config = _id2Array[key];
                for (int i = 0; i < config.Points.Length - 1; ++i)
                {
                    float width = config.IsHovering ? config.Width * 2 : config.Width;
                    Handles.color = GetColor(config, i / ((float)config.Points.Length - 1));
                    Handles.DrawAAPolyLine(width, config.Points[i], config.Points[i + 1]);
                }
            }
            Handles.color = handleColor;
        }

        private Color GetColor(LineConfig config, float lerp)
        {
            if (config.IsSelecting) return config.SelectedColor;
            Color color = Color.Lerp(config.StartColor, config.EndColor, lerp);
            return color;
        }

        public LineConfig DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor, float width = 5, int tick = 10)
        {
            uint id = _ids;
            ++_ids;
            LineConfig config = new LineConfig(id, this)
            {
                StartColor = startColor,
                EndColor = endColor,
                Width = width,
                LineCount = tick
            };
            config.ReCalcDPosition(start, end);
            _id2Array.Add(id, config);
            return config;
        }

        public LineConfig DrawLine(Vector3 start, Vector3 end)
        {
            return DrawLine(start, end, Color.white, Color.white);
        }

        private void Remove(LineConfig lineConfig)
        {
            _id2Array.Remove(lineConfig.ID);
        }
    }
}
