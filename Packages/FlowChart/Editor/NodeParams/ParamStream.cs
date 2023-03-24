using System;

namespace ZKnight.UFlowChart.Editor
{
    public class ParamStream
    {
        public int StreamID;
        public string Description;
        public string FieldName;
        public NodeParams Connection;
        public NodeParams Parent;

        public ParamStream(int id, NodeParams parent)
        {
            StreamID = id;
            Description = $"Stream_{id}";
            Parent = parent;
        }

        public void CopyFrom(ParamStream src)
        {
            StreamID = src.StreamID;
            Description = src.Description;
            FieldName = src.FieldName;
        }
    }
}
