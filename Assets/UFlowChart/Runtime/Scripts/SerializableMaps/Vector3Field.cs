using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class Vector3Field : MonoBehaviour, ISerializable
    {
        public Vector3 FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
