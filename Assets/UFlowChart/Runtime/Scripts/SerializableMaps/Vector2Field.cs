using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class Vector2Field : MonoBehaviour, ISerializable
    {
        public Vector2 FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
