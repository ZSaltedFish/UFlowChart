using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class Color32Field : MonoBehaviour, ISerializable
    {
        public Color32 FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
