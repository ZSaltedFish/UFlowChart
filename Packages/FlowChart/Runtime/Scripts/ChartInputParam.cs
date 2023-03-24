namespace ZKnight.UFlowChart.Runtime
{
    public class ChartInputParam<T> : IChartInput
    {
        public T FieldInput;
        public string DynamicInput;

        public string InputKey => DynamicInput;

        public void SetValue(object value)
        {
            FieldInput = (T)value;
        }

        public T Value => FieldInput;
    }
}
