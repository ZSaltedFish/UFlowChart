using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class IntField : MonoBehaviour, ISerializable
    {
        public int FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
