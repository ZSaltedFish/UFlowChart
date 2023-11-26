namespace ZKnight.UFlowChart.Runtime
{
    public interface IChartInput
    {
        string InputKey { get; }
        void SetValue(object value);
    }
}
