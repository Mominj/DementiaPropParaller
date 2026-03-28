using UnityEngine;

public class ShufflingGait : MonoBehaviour
{
    public Animator fatherAnimator;
    public float animatorSpeedMul = 0.65f;
    
    private static readonly int IsWalking = 
        Animator.StringToHash("isWalking");

    [ContextMenu("Start The Shuffle")]
    public void StartShuffle()
    {
        fatherAnimator.SetBool(IsWalking, true);
        fatherAnimator.speed = animatorSpeedMul;
    }
    
    public void StopShuffle()
    {
        fatherAnimator.SetBool(IsWalking, false);
        fatherAnimator.speed = 1f;
    }
}