using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Editor
{
    public static class InputType2Color
    {
        private static Dictionary<Type, Color> _TYPE_TO_COLOR = new Dictionary<Type, Color>()
        {
            {typeof(int), new Color(0.2f, 0.3f, 1f, 1) },
            {typeof(float), new Color(0.5f, 0.5f, 1f, 1) },
            {typeof(double), new Color(0.5f, 0.65f, 1f, 1) },
            {typeof(Vector2), new Color(1, 1, 0, 1) },
            {typeof(Vector3), new Color(0, 1, 1, 1) },
            {typeof(Vector4), new Color(1, 0, 1, 1) },
            {typeof(MonoBehaviour), new Color(0.3f, 1f, 0.3f, 1)},
            {typeof(ScriptableObject), new Color(0.4f, 1f, 0.4f, 1) },
            {typeof(UnityEngine.Object), new Color(0.2f, 1f, 0.2f, 1) },
            {typeof(string), new Color(0.1f, 0.1f, 1f) },
            {typeof(Enum), new Color(0.1f, 1f, 0.3f) },
            {typeof(bool), new Color(1, 0.2f, 0.2f) }
        };

        public static Color GetColor(Type type)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour))) return _TYPE_TO_COLOR[typeof(MonoBehaviour)];
            if (type.IsSubclassOf(typeof(ScriptableObject))) return _TYPE_TO_COLOR[typeof(ScriptableObject)];
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) return _TYPE_TO_COLOR[typeof(UnityEngine.Object)];
            if (type.IsSubclassOf(typeof(Enum))) return _TYPE_TO_COLOR[typeof(Enum)];

            if (_TYPE_TO_COLOR.TryGetValue(type, out Color color))
            {
                return color;
            }
            else
            {
                return Color.white;
            }
        }
    }
}
