using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Attach to: CHAR_Father_Patient
/// Controls two-handed paper grasping with smooth blend
/// </summary>
public class HandGraspController : MonoBehaviour
{
    [Header("Rig References")]
    public Rig handGraspingRig;          // drag handGraspingRig
    public TwoBoneIKConstraint rightArmIK; // drag RightArm_IK
    public TwoBoneIKConstraint leftArmIK;  // drag LeftArm_IK

    [Header("Grasp Settings")]
    [Range(0f, 1f)]
    public float graspWeight    = 0f;
    public float graspSpeed     = 2f;   // how fast fingers curl
    public bool  isHoldingPaper = true; // true = holding from start

    [Header("Individual Arm Weights")]
    [Range(0f, 1f)]
    public float rightArmWeight = 1f;
    [Range(0f, 1f)]
    public float leftArmWeight  = 1f;

    // ── Private ───────────────────────────────────────
    private float _currentWeight = 0f;

    // ── Start ─────────────────────────────────────────
    void Start()
    {
        // Snap to holding state immediately — no blend
        if (isHoldingPaper)
            SetGraspImmediate(true);
    }

    // ── Update — runs every frame ─────────────────────
    void Update()
    {
        // Calculate target weight based on holding state
        float targetWeight = isHoldingPaper ? 1f : 0f;

        // Smoothly move current weight toward target
        _currentWeight = Mathf.MoveTowards(
            _currentWeight,
            targetWeight,
            graspSpeed * Time.deltaTime);

        // Apply weight to finger override rig
        if (handGraspingRig != null)
            handGraspingRig.weight = _currentWeight;

        // Apply weight to right arm IK
        if (rightArmIK != null)
            rightArmIK.weight = _currentWeight * rightArmWeight;

        // Apply weight to left arm IK
        if (leftArmIK != null)
            leftArmIK.weight = _currentWeight * leftArmWeight;
    }

    // ── Public Methods ────────────────────────────────

    /// <summary>Father grips the paper</summary>
    public void GraspPaper()
    {
        isHoldingPaper = true;
        Debug.Log("Father grasping paper with both hands");
    }

    /// <summary>Father releases the paper</summary>
    public void ReleasePaper()
    {
        isHoldingPaper = false;
        Debug.Log("Father releasing paper");
    }

    /// <summary>Instantly set weight — no smooth blend</summary>
    public void SetGraspImmediate(bool holding)
    {
        isHoldingPaper = holding;
        _currentWeight = holding ? 1f : 0f;

        if (handGraspingRig != null)
            handGraspingRig.weight = _currentWeight;

        if (rightArmIK != null)
            rightArmIK.weight = _currentWeight;

        if (leftArmIK != null)
            leftArmIK.weight = _currentWeight;
    }

    // ── Test Methods — right-click to test ───────────

    [ContextMenu("Test — Grasp Paper")]
    void TestGrasp()
    {
        GraspPaper();
        Debug.Log("Testing grasp — fingers should curl");
    }

    [ContextMenu("Test — Release Paper")]
    void TestRelease()
    {
        ReleasePaper();
        Debug.Log("Testing release — fingers return to normal");
    }

    [ContextMenu("Test — Toggle Grasp")]
    void TestToggle()
    {
        isHoldingPaper = !isHoldingPaper;
        Debug.Log($"Holding paper: {isHoldingPaper}");
    }

    [ContextMenu("Test — Instant Grasp (no blend)")]
    void TestInstantGrasp()
    {
        SetGraspImmediate(true);
        Debug.Log("Instant grasp applied");
    }

    [ContextMenu("Test — Instant Release (no blend)")]
    void TestInstantRelease()
    {
        SetGraspImmediate(false);
        Debug.Log("Instant release applied");
    }
}