using System;

namespace ZKnight.UFlowChart.Runtime
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
    public class ChatGPTAttribute : Attribute
    {
        public string Prompt { get; private set; }
        public ChatGPTAttribute(string prompt) 
        {
            Prompt = prompt;
        }
    }
}
