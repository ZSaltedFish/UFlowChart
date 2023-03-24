namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartHoverModel
    {
        public IFlowChartOperator Hovering;

        public void Release()
        {
            Hovering?.SetUnHover();
            Hovering = null;
        }

        public void SetHover(IFlowChartOperator item)
        {
            Hovering = item;
            item.SetHover();
        }
    }
}
