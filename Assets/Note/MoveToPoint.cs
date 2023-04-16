using UnityEngine;

namespace ZKnight.UFlowChart.Node
{
    public class MoveToPoint : MonoBehaviour
    {
        public float MinDistance = 0.1f;
        public Vector3 Point;
        public float Speed;
        private bool _moving = false;

        public void Update()
        {
            if (!_moving)
            {
                return;
            }

            var position = transform.position;
            var distance = Vector3.Distance(Point, position);
            float deltaMoving = Mathf.Min(Speed * Time.deltaTime, distance);
            transform.position = position + (Point - position).normalized * deltaMoving;

            if (distance < MinDistance)
            {
                _moving = false;
            }
        }

        public void Move(Vector3 point, float speed)
        {
            _moving = true;
            Point = point;
            Speed = speed;
        }
    }
}
