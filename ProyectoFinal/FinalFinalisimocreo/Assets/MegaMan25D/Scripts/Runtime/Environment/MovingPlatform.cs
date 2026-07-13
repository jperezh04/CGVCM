using UnityEngine;

namespace MegaMan25D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class MovingPlatform : MonoBehaviour
    {
        public Vector3 localOffset = new Vector3(4f, 0f, 0f);
        public float travelSeconds = 2.5f;
        public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Rigidbody body;
        private Vector3 startPosition;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            float duration = Mathf.Max(0.2f, travelSeconds);
            float normalized = Mathf.PingPong(Time.time / duration, 1f);
            float curved = movementCurve.Evaluate(normalized);
            body.MovePosition(Vector3.Lerp(startPosition, startPosition + localOffset, curved));
        }
    }
}
