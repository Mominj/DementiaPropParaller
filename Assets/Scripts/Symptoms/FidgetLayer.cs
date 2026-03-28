using UnityEngine;

/// <summary>
/// Attach to: CHAR_Father_Patient
/// Controls restless hand fidgeting via anxiety level.
/// </summary>
public class FidgetLayer : MonoBehaviour
{
    [Header("References")]
    public Animator fatherAnimator;

    [Header("Settings")]
    public float anxietyThreshold = 0.4f;  // Fidget starts above this level
    public int fidgetLayerIndex = 1;        // Base=0, FidgetLayer=1

    [Header("Anxiety Level — set by Orchestrator")]
    [Range(0f, 2f)]
    public float anxietyLevel = 0f;

    // ── Private ───────────────────────────────────────
    private static readonly int IsFidgeting = 
        Animator.StringToHash("isFidgeting");
    
    private bool _fidgeting;

    // ── Update ────────────────────────────────────────
    void Update()
    {
        // Check if Father should be fidgeting
        bool should = anxietyLevel >= anxietyThreshold;

        // Only change parameter when state changes
        if (should != _fidgeting)
        {
            _fidgeting = should;
            fatherAnimator.SetBool(IsFidgeting, should);
        }

        // Calculate target layer weight
        float targetWeight = should
            ? Mathf.Lerp(
                0.5f, 
                1f, 
                (anxietyLevel - anxietyThreshold) / 1.6f)
            : 0f;

        // Smoothly adjust layer weight
        float currentWeight = 
            fatherAnimator.GetLayerWeight(fidgetLayerIndex);
        
        fatherAnimator.SetLayerWeight(
            fidgetLayerIndex,
            Mathf.MoveTowards(
                currentWeight, 
                targetWeight, 
                Time.deltaTime * 0.8f));
    }
}