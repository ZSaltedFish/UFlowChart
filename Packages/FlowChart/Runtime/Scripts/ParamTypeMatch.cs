using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class ParamTypeMatch
    {
        private struct Type2Type
        {
            public Type SrcType, TargetType;
            public Type2Type(Type src, Type target)
            {
                SrcType = src;
                TargetType = target;
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Type2Type)) return false;
                Type2Type t = (Type2Type)obj;
                return SrcType == t.SrcType && TargetType == t.TargetType;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private static Dictionary<Type2Type, Func<object, object>> _METHOD_DICT = new Dictionary<Type2Type, Func<object, object>>()
        {
            {new Type2Type(typeof(int), typeof(float)), (o) => (float)(int)o },
            {new Type2Type(typeof(int), typeof(double)), o => (double)(int)o },
            {new Type2Type(typeof(float), typeof(int)), o => (int)(float)o },
            {new Type2Type(typeof(float), typeof(double)), o=>(double)(float)o },
            {new Type2Type(typeof(double), typeof(int)), o=>(int)(double)o },
            {new Type2Type(typeof(double), typeof(float)), o => (float)(double)o },
            {new Type2Type(typeof(int), typeof(Vector2)), o => new Vector2((float)o, 0) },
            {new Type2Type(typeof(int), typeof(Vector3)), o => new Vector3((float)o, 0, 0)},
            {new Type2Type(typeof(int), typeof(Vector4)), o => new Vector4((float)o, 0, 0, 0) },
            {new Type2Type(typeof(float), typeof(Vector2)), o => new Vector2((float)o, 0) },
            {new Type2Type(typeof(float), typeof(Vector3)), o => new Vector3((float)o, 0, 0)},
            {new Type2Type(typeof(float), typeof(Vector4)), o => new Vector4((float)o, 0, 0, 0) },
            {new Type2Type(typeof(double), typeof(Vector2)), o => new Vector2((float)o, 0) },
            {new Type2Type(typeof(double), typeof(Vector3)), o => new Vector3((float)o, 0, 0)},
            {new Type2Type(typeof(double), typeof(Vector4)), o => new Vector4((float)o, 0, 0, 0) },
            {new Type2Type(typeof(Vector2), typeof(float)), o => ((Vector2)o).x },
            {new Type2Type(typeof(Vector2), typeof(Vector3)), o => (Vector3)(Vector2)o },
            {new Type2Type(typeof(Vector2), typeof(Vector4)), o => (Vector4)(Vector2)o },
            {new Type2Type(typeof(Vector3), typeof(float)), o => ((Vector3)o).x },
            {new Type2Type(typeof(Vector3), typeof(Vector2)), o => (Vector2)(Vector3)o },
            {new Type2Type(typeof(Vector3), typeof(Vector4)), o => (Vector4)(Vector3)o },
            {new Type2Type(typeof(Vector4), typeof(float)), o => ((Vector4)o).x },
            {new Type2Type(typeof(Vector4), typeof(Vector2)), o => (Vector2)(Vector4)o },
            {new Type2Type(typeof(Vector4), typeof(Vector3)), o => (Vector3)(Vector4)o }
        };

        public static bool TryTransformData(object value, Type targetType, out object result)
        {
            Type srcType = value.GetType();
            if (srcType == targetType)
            {
                result = value;
                return true;
            }

            if (targetType == typeof(string))
            {
                result = value?.ToString();
                return true;
            }

            if (srcType.IsSubclassOf(targetType))
            {
                result = value;
                return true;
            }

            if (_METHOD_DICT.TryGetValue(new Type2Type(srcType, targetType), out var func))
            {
                result = func(value);
                return true;
            }

            if (targetType.IsSubclassOf(typeof(Enum)) && ((targetType == typeof(int) || targetType == typeof(float) || targetType == typeof(double))))
            {
                result = Enum.ToObject(targetType, value);
                return true;
            }

            if ((targetType == typeof(int) || targetType == typeof(float) || targetType == typeof(double)) && srcType.IsSubclassOf(typeof(Enum)))
            {
                result = (int)value;
                return true;
            }

            if (targetType.IsSubclassOf(typeof(Component)) && srcType.IsSubclassOf(typeof(Component)))
            {
                result = (value as Component).GetComponent(targetType);
                return true;
            }

            if (targetType.IsSubclassOf(typeof(Component)) && srcType == typeof(GameObject))
            {
                result = (value as GameObject).GetComponent(targetType);
                return true;
            }

            if (srcType.IsSubclassOf(typeof(Component)) && targetType == typeof(GameObject))
            {
                result = (value as MonoBehaviour).gameObject;
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsMatch(Type srcType, Type targetType)
        {
            if (srcType == targetType)
            {
                return true;
            }

            if (targetType == typeof(string))
            {
                return true;
            }

            if (srcType.IsSubclassOf(targetType))
            {
                return true;
            }

            if (_METHOD_DICT.TryGetValue(new Type2Type(srcType, targetType), out var func))
            {
                return true;
            }

            if (targetType.IsSubclassOf(typeof(Enum)) && ((targetType == typeof(int) || targetType == typeof(float) || targetType == typeof(double))))
            {
                return true;
            }

            if ((targetType == typeof(int) || targetType == typeof(float) || targetType == typeof(double)) && srcType.IsSubclassOf(typeof(Enum)))
            {
                return true;
            }

            if (targetType.IsSubclassOf(typeof(Component)) && srcType.IsSubclassOf(typeof(Component)))
            {
                return true;
            }

            if (targetType.IsSubclassOf(typeof(Component)) && srcType == typeof(GameObject))
            {
                return true;
            }

            if (srcType.IsSubclassOf(typeof(Component)) && targetType == typeof(GameObject))
            {
                return true;
            }
            return false;
        }
    }
}
