using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Attach to: PROP_ShoppingList
/// Follows a bone with offset — supports Scene view rotation handle
/// </summary>
public class FollowBone : MonoBehaviour
{
    [Header("Target Bone")]
    public Transform boneToFollow;

    [Header("Offset From Bone")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Follow Settings")]
    public bool followPosition = true;
    public bool followRotation = true;

    [Header("Editor Tools")]
    public bool showHandle = true;   // show rotate handle in Scene
    public bool editMode   = false;  // toggle to adjust position live

#if UNITY_EDITOR
    private bool _prevEditMode = false;

    // Fires synchronously when any Inspector field changes — before LateUpdate.
    // This catches the case where the user unchecks editMode directly in the Inspector
    // instead of using the context menu.
    void OnValidate()
    {
        if (_prevEditMode && !editMode)
        {
            // editMode was just turned OFF — capture offset immediately
            // before LateUpdate can overwrite transform.position
            if (boneToFollow != null)
                CaptureCurrentOffset();
            else
                Debug.LogWarning("FollowBone: editMode turned off but boneToFollow is null — assign it first.");
        }
        _prevEditMode = editMode;
    }
#endif

    // ── LateUpdate ────────────────────────────────────
    void LateUpdate()
    {
        if (boneToFollow == null) return;

        // Skip following when in edit mode
        if (editMode) return;

        if (followPosition)
        {
            transform.position = boneToFollow.position
                + boneToFollow.TransformDirection(positionOffset);
        }

        if (followRotation)
        {
            transform.rotation = boneToFollow.rotation
                * Quaternion.Euler(rotationOffset);
        }
    }

    // ── Capture Current Offset ────────────────────────
    [ContextMenu("Capture Current Offset")]
    public void CaptureCurrentOffset()
    {
        if (boneToFollow == null)
        {
            Debug.LogError("FollowBone: boneToFollow is NULL — drag the hand bone into the field!");
            return;
        }

#if UNITY_EDITOR
        Undo.RecordObject(this, "Capture Bone Offset");
#endif

        positionOffset = boneToFollow.InverseTransformDirection(
            transform.position - boneToFollow.position);

        rotationOffset = (Quaternion.Inverse(boneToFollow.rotation)
            * transform.rotation).eulerAngles;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif

        Debug.Log($"[FollowBone] === CAPTURED OFFSET ===");
        Debug.Log($"[FollowBone] Position Offset: {positionOffset}");
        Debug.Log($"[FollowBone] Rotation Offset: {rotationOffset}");
        Debug.Log($"[FollowBone] ====================");
    }

    // ── Toggle Edit Mode ──────────────────────────────
    [ContextMenu("Enable Edit Mode (use handles)")]
    public void EnableEditMode()
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Enable Edit Mode");
        _prevEditMode = true;
#endif
        editMode = true;
        Debug.Log("[FollowBone] Edit mode ON — use W/E handles to position the prop");
    }

    [ContextMenu("Disable Edit Mode + Capture Offset")]
    public void DisableEditMode()
    {
        // Capture BEFORE setting editMode = false so LateUpdate cannot race us
        CaptureCurrentOffset();

#if UNITY_EDITOR
        Undo.RecordObject(this, "Disable Edit Mode");
        _prevEditMode = false;
#endif
        editMode = false;
        Debug.Log("[FollowBone] Edit mode OFF — offset saved");
    }

    [ContextMenu("Reset to Bone Position")]
    public void ResetToBone()
    {
        if (boneToFollow == null) return;
#if UNITY_EDITOR
        Undo.RecordObject(this, "Reset Bone Offset");
#endif
        positionOffset = Vector3.zero;
        rotationOffset = Vector3.zero;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        Debug.Log("[FollowBone] Offset reset to zero");
    }
}

// ── CUSTOM EDITOR — adds move/rotate handles in Scene view ──────
#if UNITY_EDITOR
[CustomEditor(typeof(FollowBone))]
public class FollowBoneEditor : Editor
{
    void OnSceneGUI()
    {
        FollowBone followBone = (FollowBone)target;

        if (!followBone.showHandle) return;
        if (!followBone.editMode)   return;
        if (followBone.boneToFollow == null) return;

        // Draw position handle
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.PositionHandle(
            followBone.transform.position,
            followBone.transform.rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(followBone.transform, "Move Shopping List");
            followBone.transform.position = newPos;
        }

        // Draw rotation handle
        EditorGUI.BeginChangeCheck();
        Quaternion newRot = Handles.RotationHandle(
            followBone.transform.rotation,
            followBone.transform.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(followBone.transform, "Rotate Shopping List");
            followBone.transform.rotation = newRot;
        }

        Handles.Label(
            followBone.transform.position + Vector3.up * 0.2f,
            "Shopping List\n[Edit Mode ON]");
    }
}
#endif
