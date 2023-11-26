using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class QuaternionField : MonoBehaviour, ISerializable
    {
        public Quaternion FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
