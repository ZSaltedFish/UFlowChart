using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class FloatField : MonoBehaviour, ISerializable
    {
        public float FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
