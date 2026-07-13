using MegaMan25D;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetVisualSlot))]
public sealed class AssetVisualSlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AssetVisualSlot slot = (AssetVisualSlot)target;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Asigna un prefab visual y pulsa Apply. La física y los scripts permanecen en la raíz.",
            MessageType.Info
        );

        using (new EditorGUI.DisabledScope(slot.assetPrefab == null))
        {
            if (GUILayout.Button("Apply / Refresh Asset In Scene"))
            {
                ApplyVisual(slot);
            }
        }

        if (GUILayout.Button("Remove Applied Asset And Show Placeholder"))
        {
            RemoveVisual(slot);
        }

        if (GUILayout.Button("Reapply Position / Rotation / Scale"))
        {
            Undo.RecordObject(slot, "Reapply Visual Offsets");
            slot.ApplyOffsets();
            EditorUtility.SetDirty(slot);
        }
    }

    private static void ApplyVisual(AssetVisualSlot slot)
    {
        RemoveVisual(slot);

        Transform parent = slot.visualAnchor != null ? slot.visualAnchor : slot.transform;
        GameObject instance = PrefabUtility.InstantiatePrefab(slot.assetPrefab, parent) as GameObject;

        if (instance == null)
        {
            instance = Instantiate(slot.assetPrefab, parent);
        }

        Undo.RegisterCreatedObjectUndo(instance, "Apply Visual Asset");
        instance.name = "__AppliedVisual";

        Undo.RecordObject(slot, "Register Applied Visual");
        slot.RegisterAppliedVisual(instance);

        if (slot.hidePlaceholderWhenAssetIsApplied && slot.placeholderRoot != null)
        {
            Undo.RecordObject(slot.placeholderRoot, "Hide Placeholder");
            slot.placeholderRoot.SetActive(false);
        }

        EditorUtility.SetDirty(slot);
    }

    private static void RemoveVisual(AssetVisualSlot slot)
    {
        GameObject current = slot.AppliedVisual;

        if (current == null)
        {
            Transform parent = slot.visualAnchor != null ? slot.visualAnchor : slot.transform;
            Transform found = parent.Find("__AppliedVisual");
            if (found != null)
            {
                current = found.gameObject;
            }
        }

        if (current != null)
        {
            Undo.DestroyObjectImmediate(current);
        }

        Undo.RecordObject(slot, "Clear Applied Visual");
        slot.ClearAppliedVisualReference();

        if (slot.placeholderRoot != null)
        {
            Undo.RecordObject(slot.placeholderRoot, "Show Placeholder");
            slot.placeholderRoot.SetActive(true);
        }

        EditorUtility.SetDirty(slot);
    }
}
