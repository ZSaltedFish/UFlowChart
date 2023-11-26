using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class Vector4Field : MonoBehaviour, ISerializable
    {
        public Vector4 FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
