using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    [Header("Start Delay")]
    [Tooltip("Wait this many seconds before starting to follow bone")]
    public float startDelay = 013f; // small delay for animation to initialise

    [Header("Editor Tools")]
    public bool showHandle = true;
    public bool editMode   = false;

    


#if UNITY_EDITOR
    private bool _prevEditMode = false;

    void OnValidate()
    {
        if (_prevEditMode && !editMode)
        {
            if (boneToFollow != null)
                CaptureCurrentOffset();
            else
                Debug.LogWarning("FollowBone: boneToFollow is null");
        }
        _prevEditMode = editMode;
    }
#endif

    // ── Private ───────────────────────────────────────
    private bool _ready = false;

    // ── Start ─────────────────────────────────────────
    void Start()
    {
        // Wait for animation to initialise before following
        StartCoroutine(DelayedStart());
    }

System.Collections.IEnumerator InitAfterFrame()
{
    // Completely hide list on first frame
    if (TryGetComponent<MeshRenderer>(out var mr))
        mr.enabled = false;

    // Wait 3 frames to be safe
    yield return new WaitForEndOfFrame();
    yield return new WaitForEndOfFrame();
    yield return new WaitForEndOfFrame();

    // Show list — animator has updated all bones by now
    if (TryGetComponent<MeshRenderer>(out var mr2))
        mr2.enabled = true;

    _ready = true;
    Debug.Log("[FollowBone] Ready ✅");
}
    System.Collections.IEnumerator DelayedStart()
    {
        // Wait for animator to play first frame
        yield return new WaitForSeconds(startDelay);
        _ready = true;
        Debug.Log("[FollowBone] Started following bone ✅");
    }

    // ── LateUpdate ────────────────────────────────────
    void LateUpdate()
    {
        if (boneToFollow == null) return;
        if (editMode) return;
        if (!_ready) return; // wait until delay is done

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
            Debug.LogError("FollowBone: boneToFollow is NULL!");
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
        Debug.Log($"[FollowBone] Position Offset: {positionOffset}");
        Debug.Log($"[FollowBone] Rotation Offset: {rotationOffset}");
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
        Debug.Log("[FollowBone] Edit mode ON");
    }

    [ContextMenu("Disable Edit Mode + Capture Offset")]
    public void DisableEditMode()
    {
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

// ── CUSTOM EDITOR ─────────────────────────────────────────────
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

        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.PositionHandle(
            followBone.transform.position,
            followBone.transform.rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(followBone.transform, "Move Shopping List");
            followBone.transform.position = newPos;
        }

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