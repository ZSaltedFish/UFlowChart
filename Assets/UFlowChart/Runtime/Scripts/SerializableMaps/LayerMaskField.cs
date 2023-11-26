using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class LayerMaskField : MonoBehaviour, ISerializable
    {
        public LayerMask FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
