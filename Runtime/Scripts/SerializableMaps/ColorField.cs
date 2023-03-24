using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class ColorField : MonoBehaviour, ISerializable
    {
        public Color FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
