using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public static class ChatGPTCreator
    {
        public static IEnumerable<Type> GetAllChatGPTTypeNode()
        {
            var list = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(FlowChartNode)))
                    {
                        if (type.GetCustomAttributes(typeof(ChatGPTAttribute), false).Length > 0)
                        {
                            list.Add(type);
                        }
                    }
                }
            }
            return list;
        }

        public static string GetArrayJson()
        {
            var types = GetAllChatGPTTypeNode();
            var list = new List<FlowChartGPTType>();
            foreach (var type in types)
            {
                var gptAttrs = type.GetCustomAttributes(typeof(ChatGPTAttribute), false);
                var gptType = new FlowChartGPTType
                {
                    FullName = type.FullName,
                    Description = (gptAttrs[0] as ChatGPTAttribute).Prompt
                };
                var inputs = new List<FlowChartGPTField>();
                var outputs = new List<FlowChartGPTField>();
                var streams = new List<FlowChartGPTField>();


                var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var attrs = field.GetCustomAttributes(false);
                    foreach (var attr in attrs)
                    {
                        if (attr is FlowChartInputAttribute)
                        {
                            if (TryGetGPTField(field, out var desc))
                            {
                                inputs.Add(new FlowChartGPTField(field.Name, desc));
                            }
                            else
                            {
                                inputs.Add(new FlowChartGPTField(field.Name, (attr as FlowChartInputAttribute).Description));
                            }
                        }
                        else if (attr is FlowChartOutputAttribute)
                        {
                            if (TryGetGPTField(field, out var desc))
                            {
                                outputs.Add(new FlowChartGPTField(field.Name, desc));
                            }
                            else
                            {
                                outputs.Add(new FlowChartGPTField(field.Name, (attr as FlowChartOutputAttribute).Description));
                            }
                        }
                        else if (attr is FlowChartStreamAttribute)
                        {
                            if (TryGetGPTField(field, out var desc))
                            {
                                streams.Add(new FlowChartGPTField(field.Name, desc));
                            }
                            else
                            {
                                streams.Add(new FlowChartGPTField(field.Name, (attr as FlowChartStreamAttribute).Description));
                            }
                        }
                    }
                }

                gptType.Inputs = inputs.ToArray();
                gptType.Outputs = outputs.ToArray();
                gptType.Links = streams.ToArray();

                list.Add(gptType);
            }

            return JsonConvert.SerializeObject(list);
        }

        private static bool TryGetGPTField(FieldInfo field, out string desc)
        {
            var chatGPT = field.GetCustomAttribute<ChatGPTAttribute>();
            desc = string.Empty;
            if (chatGPT == null)
            {
                return false;
            }

            desc = chatGPT.Prompt;
            return true;
        }

        public static string StartWorld()
        {
            var path = "Packages/com.ZKnight.UFlowChart/Resources/ChatGPT/GPTDirect.txt";
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            return textAsset.text;
        }

        public static string GetGPTHead()
        {
            return $"{StartWorld()}\n{GetArrayJson()}";
        }
    }
}
