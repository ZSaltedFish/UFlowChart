using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class DoubleField : MonoBehaviour, ISerializable
    {
        public double FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
