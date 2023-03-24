namespace ZKnight.UFlowChart.Editor
{
    public struct FlowChartLineID
    {
        public int OutputNodeID, InputNodeID;
        public LineConnectingType LineType;
        public int OutputIndex, InputIndex;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(FlowChartLineID)) return false;
            FlowChartLineID target = (FlowChartLineID)obj;
            return OutputNodeID == target.OutputNodeID && InputNodeID == target.InputNodeID && LineType == target.LineType && OutputIndex == target.OutputIndex && InputIndex == target.InputIndex;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
