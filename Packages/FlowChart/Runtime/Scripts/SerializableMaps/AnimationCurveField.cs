﻿using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class AnimationCurveField : MonoBehaviour, ISerializable
    {
        public AnimationCurve FieldValue;
        public string FieldKey;
        public string Key => FieldKey;
        public object Value => FieldValue;
    }
}
