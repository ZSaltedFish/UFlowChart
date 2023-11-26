using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    public class FlowChartManager : MonoBehaviour
    {
        public void InitFlowChart(GameObject item)
        {
            FlowChart fc = item.GetComponent<FlowChart>();
            FlowChartNode[] nodes = fc.GetComponentsInChildren<FlowChartNode>();
            fc.Nodes = new List<FlowChartNode>(nodes);
        }

    }
}
