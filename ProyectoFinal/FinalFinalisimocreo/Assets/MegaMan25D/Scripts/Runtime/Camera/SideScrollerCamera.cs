using UnityEngine;

namespace MegaMan25D
{
    public sealed class SideScrollerCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(2.5f, 1.2f, -10f);
        public float smoothTime = 0.16f;
        public bool clampVertical;
        public Vector2 verticalLimits = new Vector2(-1f, 5f);
        public bool useHorizontalLimits;
        public Vector2 horizontalLimits = new Vector2(-1000f, 1000f);

        private Vector3 velocity;
        private bool temporaryHorizontalLock;
        private Vector2 savedHorizontalLimits;
        private bool savedUseHorizontalLimits;

        public void SetTemporaryHorizontalBounds(float minimumX, float maximumX)
        {
            if (!temporaryHorizontalLock)
            {
                savedUseHorizontalLimits = useHorizontalLimits;
                savedHorizontalLimits = horizontalLimits;
            }

            temporaryHorizontalLock = true;
            useHorizontalLimits = true;
            horizontalLimits = new Vector2(minimumX, maximumX);
        }

        public void ClearTemporaryHorizontalBounds()
        {
            if (!temporaryHorizontalLock)
            {
                return;
            }

            temporaryHorizontalLock = false;
            useHorizontalLimits = savedUseHorizontalLimits;
            horizontalLimits = savedHorizontalLimits;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desired = target.position + offset;
            desired.z = offset.z;

            if (clampVertical)
            {
                desired.y = Mathf.Clamp(desired.y, verticalLimits.x, verticalLimits.y);
            }

            if (useHorizontalLimits)
            {
                desired.x = Mathf.Clamp(desired.x, horizontalLimits.x, horizontalLimits.y);
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desired,
                ref velocity,
                smoothTime
            );
        }
    }
}
