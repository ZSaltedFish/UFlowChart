using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public static class AssemblyHelper
    {
        private static Type[] _TYPES;
        private static Dictionary<string, Type> _ACCEPATABLE_TYPE;
        public static Type GetType(string name)
        {
            Type type = typeof(int).Assembly.GetType(name);
            if (type == null)
            {
                type = typeof(FlowChart).Assembly.GetType(name);
            }

            if (type == null)
            {
                type = typeof(GameObject).Assembly.GetType(name);
            }

            if (type == null)
            {
                type = typeof(Animator).Assembly.GetType(name);
            }

            if (type == null)
            {
                type = typeof(RawImage).Assembly.GetType(name);
            }

            if (type == null)
            {
                type = typeof(ParticleSystem).Assembly.GetType(name);
            }
            return type;
        }

        public static Type[] GetAllType()
        {
            if (_TYPES == null)
            {
                List<Type> types = new List<Type>(typeof(int).Assembly.GetTypes());
                types.AddRange(typeof(FlowChart).Assembly.GetTypes());
                types.AddRange(typeof(GameObject).Assembly.GetTypes());
                types.AddRange(typeof(Animator).Assembly.GetTypes());
                types.AddRange(typeof(RawImage).Assembly.GetTypes());
                types.AddRange(typeof(ParticleSystem).Assembly.GetTypes());
                _TYPES = types.ToArray();
            }
            return _TYPES;
        }

        public static Dictionary<string, Type> AcceptableTypes
        {
            get
            {
                if (_ACCEPATABLE_TYPE == null)
                {
                    _ACCEPATABLE_TYPE = new Dictionary<string, Type>()
                    {
                        {"bool", typeof(bool)},
                        {"int", typeof(int) },
                        {"float", typeof(float) },
                        {"double", typeof(double) },
                        {"Vector2", typeof(Vector2) },
                        {"Vector3", typeof(Vector3) },
                        {"Vector4", typeof(Vector4) },
                        {"Color", typeof(Color) },
                        {"Color32", typeof(Color32) },
                        {"Gradient", typeof(Gradient) },
                        {"GUIStyle", typeof(GUIStyle) },
                        {"LayerMask", typeof(LayerMask) },
                        {"Matrix4x4", typeof(Matrix4x4) },
                        {"Quaternion", typeof(Quaternion) },
                        {"Rect", typeof(Rect) },
                        {"RectOffset", typeof(RectOffset) },
                        {"string", typeof(string) },
                        {"GameObject", typeof(GameObject) }
                    };
                    Type[] types = GetAllType();
                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(typeof(UnityEngine.Object)) && !type.IsSubclassOf(typeof(FlowChartNode)) && type != typeof(GameObject))
                        {
                            if (!_ACCEPATABLE_TYPE.ContainsKey(type.FullName))
                            {
                                _ACCEPATABLE_TYPE.Add(type.FullName, type);
                            }
                        }
                    }
                }

                return _ACCEPATABLE_TYPE;
            }
        }
    }
}
