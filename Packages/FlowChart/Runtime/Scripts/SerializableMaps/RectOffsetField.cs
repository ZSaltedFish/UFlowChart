using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class RectOffsetField : MonoBehaviour, ISerializable
    {
        public RectOffset FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
