using System;
using System.Collections.Generic;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public class NodeOverloadManager
    {
        public const int NOT_MATCH = 1000;
        private static NodeOverloadManager _ins;

        public static NodeOverloadManager INSTANCE
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new NodeOverloadManager();
                }
                return _ins;
            }
        }

        private Dictionary<string, List<Type>> _overloadTypes;
        private Dictionary<string, List<NodeParams>> _buffer;
        private NodeOverloadManager()
        {
            _buffer = new Dictionary<string, List<NodeParams>>();
            _overloadTypes = new Dictionary<string, List<Type>>();
        }

        public void SetOverrideNode(string path, Type type)
        {
            if (!_overloadTypes.TryGetValue(path, out List<Type> types))
            {
                types = new List<Type>();
                _overloadTypes.Add(path, types);
            }
            types.Add(type);
        }

        public List<Type> GetTypes(string path)
        {
            return _overloadTypes[path];
        }

        public bool TryOverloadNode(string path, out NodeParams nodeParams, Type paramType, int typeIndex, int count)
        {
            nodeParams = null;
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (_overloadTypes.TryGetValue(path, out List<Type> types))
            {
                int min = int.MaxValue;
                int index = -1;
                List<NodeParams> list = GetFromBuffer(path);
                for (int i = 0; i < list.Count; ++i)
                {
                    NodeParams node = list[i];
                    if (node.Inputs.Count != count)
                    {
                        continue;
                    }

                    ParamInput input = node.Inputs[typeIndex];
                    Type inputType = input.InputType;
                    Type outputType = paramType;
                    int value = MatchLevel(outputType, inputType);

                    if (min > value)
                    {
                        min = value;
                        index = i;
                    }
                }

                if (index == -1 || min > NOT_MATCH)
                {
                    return false;
                }
                else
                {
                    nodeParams = list[index];
                    return true;
                }
            }
            return false;
        }

        private int MatchLevel(Type typeA, Type typeB)
        {
            if (typeA == typeB || typeA.IsSubclassOf(typeB))
            {
                return 0;
            }

            if (!ParamTypeMatch.IsMatch(typeA, typeB))
            {
                return NOT_MATCH;
            }

            if (typeB != typeof(string))
            {
                return 1;
            }

            return 2;
        }

        private List<NodeParams> GetFromBuffer(string path)
        {
            if (!_buffer.TryGetValue(path, out List<NodeParams> buffer))
            {
                buffer = new List<NodeParams>();
                _buffer.Add(path, buffer);

                List<Type> types = _overloadTypes[path];
                foreach (Type type in types)
                {
                    buffer.Add(new NodeParams(0, type, path));
                }
            }

            return buffer;
        }
    }
}
