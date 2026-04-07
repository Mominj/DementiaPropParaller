using UnityEngine;

/// Tilts Father's head downward in LateUpdate (after Animator) to simulate
/// looking at the shopping list on his lap.
/// Attach to CHAR_Father_Patient — head bone is auto-resolved from the humanoid rig.
public class HeadTiltDown : MonoBehaviour
{
    [Tooltip("Auto-filled from humanoid rig; drag manually to override")]
    public Transform headBone;

    [Tooltip("Degrees to pitch the head forward/down while active")]
    [Range(0f, 45f)]
    public float tiltDegrees = 25f;

    [Tooltip("Blend speed when enabling/disabling the tilt")]
    public float blendSpeed = 3f;

    [Tooltip("Enable or disable the tilt at runtime")]
    public bool tiltEnabled = true;

    private float _weight = 0f;

    void Awake()
    {
        if (headBone != null) return;

        Animator anim = GetComponent<Animator>();
        if (anim != null && anim.isHuman)
            headBone = anim.GetBoneTransform(HumanBodyBones.Head);
    }

    void LateUpdate()
    {
        if (headBone == null) return;

        float target = tiltEnabled ? 1f : 0f;
        _weight = Mathf.MoveTowards(_weight, target, blendSpeed * Time.deltaTime);
        if (_weight <= 0f) return;

        // Positive X on a Mixamo/Avaturn humanoid pitches head forward (down)
        headBone.localRotation *= Quaternion.Euler(tiltDegrees * _weight, 0f, 0f);
    }

    [ContextMenu("Enable Tilt")]
    public void EnableTilt()  => tiltEnabled = true;

    [ContextMenu("Disable Tilt")]
    public void DisableTilt() => tiltEnabled = false;
}
