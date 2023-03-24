using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class Matrix4x4Field : MonoBehaviour, ISerializable
    {
        public Matrix4x4 FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
