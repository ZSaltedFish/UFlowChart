using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public static class CreateNodeByJson
    {
        public static IEnumerable<NodeParams> GetNodeByJson(FlowChartDialog dialog, string json)
        {
            var nodeParams = new Dictionary<NodeParams, JObject>();
            var name2Params = new Dictionary<string, NodeParams>();
            var xPos = 0f;
            var root = JsonConvert.DeserializeObject<JObject>(json);
            foreach (var pair in root)
            {
                var jObject = pair.Value;
                var fullName = jObject["NodeClass"].ToString();
                if (TryGetType(fullName, out var nodeType))
                {
                    var attr = nodeType.GetCustomAttribute<FlowChartNodeAttribute>();
                    var path = attr.EditorPath;
                    var pos = new Vector2(xPos, 0);
                    xPos += 500f;
                    var nodeParam = dialog.FCanvas.CreateNode(nodeType, pos, path);

                    nodeParams.Add(nodeParam, jObject as JObject);
                    name2Params.Add(jObject["NodeName"].ToString(), nodeParam);
                    //nodeParams[jObject["NodeName"].ToString()] = nodeParam;
                }
            }

            foreach (var key in nodeParams.Keys)
            {
                var jObject = nodeParams[key];

                if (jObject["Links"]["Next"] != null && !string.IsNullOrEmpty(jObject["Links"]["Next"].ToString()))
                {
                    var targetParam = name2Params[jObject["Links"]["Next"].ToString()];
                    var streamID = 0;
                    var nextID = targetParam.NodeID;
                    dialog.FCanvas.ConnectLine(nextID, 0, key.NodeID, streamID, LineConnectingType.StreamConnection);
                    key.ConnectNode(streamID, targetParam);
                }

                if ((jObject["Inputs"] as JArray).Count > 0)
                {
                    foreach (var input in jObject["Inputs"])
                    {
                        var index = key.Inputs.FindIndex(p => p.FieldInfo.Name == input["FieldName"].ToString());
                        var inputParam = key.Inputs[index];
                        if (input["Value"] != null)
                        {
                            if (index != -1)
                            {
                                var v = input["Value"];
                                var valueString = v.Value<string>();
                                object value = valueString;
                                var inputType = inputParam.FieldInfo.FieldType;
                                if(inputType == typeof(int))
                                {
                                    value = int.Parse(valueString);
                                }
                                else if(inputType == typeof(float))
                                {
                                    value = float.Parse(valueString);
                                }
                                key.SetInputObject(index, value);
                            }
                        }
                        else if (input["Source"] != null)
                        {
                            var sps = input["Source"].ToString().Split(' ');
                            var outputNode = name2Params[sps[0]];
                            var outputIndex = outputNode.GetOutputIndexByName(sps[1]);
                            dialog.FCanvas.ConnectLine(key.NodeID, index, outputNode.NodeID, outputIndex, LineConnectingType.ParamConnection);
                            key.ConnectInput(outputNode, outputIndex, index);
                        }
                    }
                }
            }

            return nodeParams.Keys;
        }

        private static Dictionary<string, Type> _fullName2Type;
        public static bool TryGetType(string fullName, out Type type)
        {
            if (_fullName2Type == null)
            {
                _fullName2Type = new Dictionary<string, Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var assType in assembly.GetTypes())
                    {
                        if (assType.IsSubclassOf(typeof(FlowChartNode)))
                        {
                            _fullName2Type.Add(assType.FullName, assType);
                        }
                    }
                }
            }

            return _fullName2Type.TryGetValue(fullName, out type);
        }

        public static dynamic JsonObject()
        {
            dynamic objs = JsonConvert.DeserializeObject($"{GetTextJson()}");
            return objs[0]["NodeName"];
        }

        public static string GetTextJson()
        {
            var path = "Packages/com.zknight.uflowchart/Resources/ChatGPT/info.json";
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            return asset.text;
        }
    }
}
