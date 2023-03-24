using System;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartModel
    {
        public FlowChartSelectModel SelectModel;
        public FlowChartHoverModel HoverModel;
        public TempLineModel LineModel;

        public FlowChartModel()
        {
            SelectModel = new FlowChartSelectModel();
            HoverModel = new FlowChartHoverModel();
            LineModel = new TempLineModel();
        }

        public void ObjectDeleted(IFlowChartOperator obj)
        {
            if (SelectModel.SelectedLine == obj)
            {
                SelectModel.SelectedLine = null;
            }

            if (SelectModel.SelectedNodes.Contains(obj))
            {
                SelectModel.SelectedNodes.Remove(obj);
            }

            if (HoverModel.Hovering == obj)
            {
                HoverModel.Hovering = null;
            }
        }
    }
}
