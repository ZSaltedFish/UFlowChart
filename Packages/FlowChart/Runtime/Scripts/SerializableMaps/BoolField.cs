using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class BoolField : MonoBehaviour, ISerializable
    {
        public bool FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
