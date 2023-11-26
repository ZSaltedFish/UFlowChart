using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class ObjectField : MonoBehaviour, ISerializable
    {
        public Object FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
