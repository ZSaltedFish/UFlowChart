using System;
using UnityEditor;
using UnityEngine;

namespace ZKnight.UFlowChart.Editor
{
    public static class FieldType2Function
    {
        public static float FIELD_WIDTH = 120;
        public static float FIELD_HEIGHT = 16f;
        private static GUIContent _UNSUPPORTABLE_STRING = new GUIContent("不能输入静态变量");
        public static object FieldContent(Rect rect, Type type, object value)
        {
            if (type == typeof(int))
            {
                if (value == null) value = 0;
                return EditorGUI.IntField(rect, (int)value);
            }

            if (type == typeof(float))
            {
                if (value == null) value = 0f;
                return EditorGUI.FloatField(rect, (float)value);
            }

            if (type == typeof(double))
            {
                if (value == null) value = 0d;
                return EditorGUI.DoubleField(rect, (double)value);
            }

            if (type == typeof(string))
            {
                if (value == null) value = "";
                return EditorGUI.TextField(rect, (string)value);
            }

            if (type == typeof(bool))
            {
                if (value == null) value = false;
                return EditorGUI.Toggle(rect, (bool)value);
            }

            if (type == typeof(Color))
            {
                if (value == null) value = Color.black;
                return EditorGUI.ColorField(rect, (Color)value);
            }

            if (type == typeof(Vector2))
            {
                if (value == null) value = Vector2.zero;
                return EditorGUI.Vector2Field(rect, "", (Vector2)value);
            }

            if (type == typeof(Vector3))
            {
                if (value == null) value = Vector3.zero;
                return EditorGUI.Vector3Field(rect, "", (Vector3)value);
            }

            if (type == typeof(Vector4))
            {
                if (value == null) value = Vector4.zero;
                return EditorGUI.Vector4Field(rect, "", (Vector4)value);
            }

            if (type == typeof(Rect))
            {
                if (value == null) value = Rect.zero;
                return EditorGUI.RectField(rect, (Rect)value);
            }

            if (type == typeof(Vector2Int))
            {
                if (value == null) value = Vector2Int.zero;
                return EditorGUI.Vector2IntField(rect, "", (Vector2Int)value);
            }

            if (type == typeof(Vector3Int))
            {
                if (value == null) value = Vector3Int.zero;
                return EditorGUI.Vector3IntField(rect, "", (Vector3Int)value);
            }

            if (type.IsSubclassOf(typeof(UnityEngine.Object)) || type == typeof(UnityEngine.Object))
            {
                return EditorGUI.ObjectField(rect, value as UnityEngine.Object, type, true);
            }

            if (type.IsEnum)
            {
                if (value == null) value = type.GetEnumValues().GetValue(0);
                return EditorGUI.EnumPopup(rect, value as Enum);
            }

            EditorGUI.LabelField(rect, _UNSUPPORTABLE_STRING);
            return null;
            //throw new TypeAccessException($"{type} is unavailable type.");
        }

        public static object GetDefaultValue(Type type)
        {
            if (type == typeof(int)) return 0;
            if (type == typeof(float)) return 0f;
            if (type == typeof(double)) return 0d;
            if (type == typeof(bool)) return false;
            if (type == typeof(string)) return "";
            return null;
        }
    }
}
