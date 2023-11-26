namespace ZKnight.UFlowChart.Editor
{
    public interface IFlowChartOperator
    {
        void SetSelected();
        void SetUnselected();
        void SetHover();
        void SetUnHover();
        void Delete();
    }
}
