using System.Collections.Generic;

namespace Knight.UFlowChart.ChatGPT
{
    public class ChatCompletion
    {
        public string id;
        public string @object;
        public int created;
        public string model;
        public Usage usage;
        public List<Choice> choices;
    }
}
