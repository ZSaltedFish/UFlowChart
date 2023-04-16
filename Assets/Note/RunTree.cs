using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Node
{
    public class RunTree : MonoBehaviour
    {
        public GameObject Tree;

        public void Start()
        {
            var chart = Tree.GetComponent<FlowChart>();
            chart.Run<ObjectCreated>(gameObject);
        }
    }
}
