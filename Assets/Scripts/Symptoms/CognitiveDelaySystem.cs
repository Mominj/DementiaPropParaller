using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to: SYS_CognitiveDelay (empty GameObject)
/// Route ALL Father reactions through RequestAction()
/// instead of calling Animator directly.
/// </summary>
public class CognitiveDelaySystem : MonoBehaviour
{
    [Header("Delay Configuration")]
    public float minDelaySeconds = 3.0f;
    public float maxDelaySeconds = 9.0f;
    public float cognitiveLoad   = 0f;  // 0=mild, 1=moderate, 2=severe

    [Header("References")]
    public Animator fatherAnimator;

    // ── Private ───────────────────────────────────────
    private Coroutine _active;
    private static readonly int IsConfused =
        Animator.StringToHash("isConfused");

    // ── Public Methods ────────────────────────────────

    /// <summary>
    /// Call this instead of calling Animator directly.
    /// Father will wait [delay] seconds then execute the action.
    /// </summary>
    
    [ContextMenu("Start Request Action")]
    public void RequestAction(Action action, 
                              bool highPriority = false)
    {
        // High priority interrupts current processing
        if (highPriority && _active != null)
        {
            StopCoroutine(_active);
            _active = null;
        }

        // Ignore new input if already processing
        if (_active != null) return;

        _active = StartCoroutine(Delay(action));
    } 
  
    // ── Coroutine ─────────────────────────────────────
     IEnumerator Delay(Action action)
    {
        // Calculate delay based on disease severity
        float wait = Mathf.Lerp(
            minDelaySeconds,
            maxDelaySeconds,
            cognitiveLoad / 2f);

        // Add slight randomness each time
        wait += UnityEngine.Random.Range(-0.5f, 0.5f);
        wait  = Mathf.Max(0.5f, wait);

        // Show confused blank look during processing
        if (fatherAnimator)
            fatherAnimator.SetBool(IsConfused, true);

        // ← THE COGNITIVE LAG HAPPENS HERE
        yield return new WaitForSeconds(wait);

        // Execute the delayed response
        action?.Invoke();

        // Clear confused look after responding
        if (fatherAnimator)
            fatherAnimator.SetBool(IsConfused, false);

        _active = null;
    } 

 

    // ── Helper ────────────────────────────────────────
    public bool IsProcessing => _active != null;

    [ContextMenu("Test Normal Delay")]
void TestNormalDelay()
{
    RequestAction(() => 
    {
        Debug.Log("Father responded after normal delay!");
    });
}

[ContextMenu("Test High Priority")]
void TestHighPriority()
{
    RequestAction(() => 
    {
        Debug.Log("Father responded — HIGH PRIORITY!");
    }, highPriority: true);
}

[ContextMenu("Test Confused Animation")]
void TestConfusedAnim()
{
    RequestAction(() => 
    {
        Debug.Log("Confusion cleared!");
        fatherAnimator.SetBool("isConfused", false);
    });
}
}
