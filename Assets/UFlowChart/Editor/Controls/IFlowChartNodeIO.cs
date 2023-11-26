namespace ZKnight.UFlowChart.Editor
{
    public interface IFlowChartNodeIO
    {
        IFlowChartNodeIO Input { get; set; }
        IFlowChartNodeIO Output { get; set; }
    }
}
