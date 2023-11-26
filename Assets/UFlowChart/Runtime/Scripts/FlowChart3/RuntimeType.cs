using System;

namespace ZKnight.UFlowChart.Runtime
{
    [Serializable]
    public class RuntimeType<T>
    {
        public int IntValue;
        public string StringValue;
        public bool BoolValue;
        public float FloatValue;
        public UnityEngine.Object UnityObjValue;

        public bool IsDynamicValue;
        public string RuntimeKey;


    }
}
