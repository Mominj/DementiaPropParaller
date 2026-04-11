using UnityEngine;

// Controls restless hand fidgeting via anxiety level.

public class FidgetLayer : MonoBehaviour
{
    [Header("References")]
    public Animator fatherAnimator;

    [Header("Settings")]
    public float anxietyThreshold = 0.1f;  // Fidget starts above this level
    public int fidgetLayerIndex = 1;        // Base=0, FidgetLayer=1
    [Tooltip("Multiplies the fidget animation playback speed. Lower = slower trembling.")]
    public float fidgetAnimSpeed = 0.5f;

    [Header("Anxiety Level — set by Orchestrator")]
    [Range(0f, 2f)]
    public float anxietyLevel = 0f;

    private static readonly int IsFidgeting =
        Animator.StringToHash("isFidgeting 1");
    
    private bool _fidgeting;

    void OnDisable()
    {
        if (fatherAnimator == null) return;
        _fidgeting = false;
        fatherAnimator.SetBool(IsFidgeting, false);
        fatherAnimator.SetLayerWeight(fidgetLayerIndex, 0f);
    }

    void Update()
    {
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

        // Control trembling speed via layer playback speed
        fatherAnimator.SetFloat("FidgetSpeed", should ? fidgetAnimSpeed : 1f);
    }
}