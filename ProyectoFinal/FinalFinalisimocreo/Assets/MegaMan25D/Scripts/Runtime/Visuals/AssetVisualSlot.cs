using UnityEngine;

namespace MegaMan25D
{
    public sealed class AssetVisualSlot : MonoBehaviour
    {
        public GameObject assetPrefab;
        public Transform visualAnchor;
        public GameObject placeholderRoot;
        public bool hidePlaceholderWhenAssetIsApplied = true;
        public Vector3 visualPositionOffset;
        public Vector3 visualRotationOffset;
        public Vector3 visualScale = Vector3.one;

        [SerializeField, HideInInspector]
        private GameObject appliedVisual;

        public GameObject AppliedVisual => appliedVisual;

        private void Awake()
        {
            if (assetPrefab == null || appliedVisual != null)
            {
                return;
            }

            Transform parent = visualAnchor != null ? visualAnchor : transform;
            appliedVisual = Instantiate(assetPrefab, parent);
            appliedVisual.name = "__RuntimeVisual";
            ApplyOffsets();

            if (hidePlaceholderWhenAssetIsApplied && placeholderRoot != null)
            {
                placeholderRoot.SetActive(false);
            }
        }

        public void RegisterAppliedVisual(GameObject visual)
        {
            appliedVisual = visual;
            ApplyOffsets();
        }

        public void ClearAppliedVisualReference()
        {
            appliedVisual = null;
        }

        public void ApplyOffsets()
        {
            if (appliedVisual == null)
            {
                return;
            }

            Transform visualTransform = appliedVisual.transform;
            visualTransform.localPosition = visualPositionOffset;
            visualTransform.localRotation = Quaternion.Euler(visualRotationOffset);
            visualTransform.localScale = visualScale;
        }
    }
}
