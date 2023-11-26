using System.Collections.Generic;
using UnityEngine;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartNodePanel : EditorControl
    {
        private List<FlowChartNodeCtrl> _dragNodes = new List<FlowChartNodeCtrl>();
        private List<Vector2> _offsets = new List<Vector2>();

        public override void InitFinish()
        {
            OnMouseDrag.Add(FlowChartController.OnNodePanelMouseMove);
        }
        public void SetDragNode(FlowChartNodeCtrl dragingNode, Vector2 offset)
        {
            if (dragingNode == null)
            {
                _dragNodes.Clear();
                _offsets.Clear();
            }
            else
            {
                _dragNodes.Add(dragingNode);
                _offsets.Add(offset);
            }
        }
    }
}