using System.Collections;
using UnityEngine;


/// Controls Father's animation sequence in Scene 1.
/// Sit idle → (wait) → stand up → standing idle.

public class Sence1FatherController : MonoBehaviour
{
    [Header("References")]
    public Animator fatherAnimator;

    [Header("Timing")]
    [Tooltip("Seconds Father sits before standing up")]
    public float readingDuration = 8f;

    // Animator parameter hashes
    private int _hashIsStandingUp;
    private int _hashIsStanding;

    public HandGraspController handGrasp;


    void Awake()
    {
        _hashIsStandingUp = Animator.StringToHash("IsStandUp");
        _hashIsStanding   = Animator.StringToHash("IsStanding");

        if (fatherAnimator == null)
            fatherAnimator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        handGrasp.SetGraspImmediate(true);
        StartCoroutine(ReadThenStand());
    }

    IEnumerator ReadThenStand()
    {
        yield return new WaitForSeconds(readingDuration);
        fatherAnimator.SetBool(_hashIsStandingUp, true);
        fatherAnimator.SetBool(_hashIsStanding, false);
    }

    /// <summary>
    /// Add this as an Animation Event on the LAST frame of the stand-up clip.
    /// Transitions Father into standing idle.
    /// </summary>
    public void OnStandUpComplete()
    {
        fatherAnimator.SetBool(_hashIsStandingUp, false);
        fatherAnimator.SetBool(_hashIsStanding, true);
    }

    [ContextMenu("Force Stand Up Now")]
    public void ForceStandUp()
    {
        StopAllCoroutines();
        fatherAnimator.SetBool(_hashIsStandingUp, true);
        fatherAnimator.SetBool(_hashIsStanding, false);
    }

    [ContextMenu("Reset to Sitting")]
    public void ResetToSitting()
    {
        StopAllCoroutines();
        fatherAnimator.SetBool(_hashIsStandingUp, false);
        fatherAnimator.SetBool(_hashIsStanding, false);
        StartCoroutine(ReadThenStand());
    }

    public void FatherPicksUpPaper()
{
    // Trigger reach animation first
    fatherAnimator.SetTrigger("TriggerReach");

    // After reach animation starts
    StartCoroutine(GraspAfterDelay(0.5f));
}

IEnumerator GraspAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    handGrasp.GraspPaper();
}

public void FatherPutsPaperDown()
{
    handGrasp.ReleasePaper();
}
}
