using UnityEngine;

/// <summary>
/// Manages the opening sequence: Father starts sitting on sofa holding a
/// shopping list, then stands up after a configurable delay.
///
/// Setup:
///   1. Attach this component to CHAR_Father_Patient.
///   2. Assign shoppingListProp — parented to Father's right hand at Start.
///   3. Set standUpDelay (seconds before Father rises from the sofa).
///      Set to 0 to disable auto stand-up (call TriggerStandUp() manually).
/// </summary>
public class SittingStartSequence : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The PROP_ShoppingList GameObject")]
    public GameObject shoppingListProp;

    [Tooltip("Father's right-hand bone (Armature/.../RightHand)")]
    public Transform rightHandBone;

    [Tooltip("The Father's Animator (auto-found on same GameObject if null)")]
    public Animator fatherAnimator;

    [Header("Shopping List Hold Offset")]
    [Tooltip("Local position offset relative to the right hand bone")]
    public Vector3 holdPositionOffset = new Vector3(0f, 0.05f, 0.05f);

    [Tooltip("Local rotation offset relative to the right hand bone")]
    public Vector3 holdRotationOffset = new Vector3(-30f, 0f, 0f);

    [Header("Timing")]
    [Tooltip("Seconds to sit before standing up. 0 = never stand automatically.")]
    public float standUpDelay = 5f;

    void Awake()
    {
        if (fatherAnimator == null)
            fatherAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        // Ensure Father starts in Sitting Idle
        if (fatherAnimator != null)
            fatherAnimator.SetBool("isSitting", true);

        AttachShoppingList();

        if (standUpDelay > 0f)
            Invoke(nameof(TriggerStandUp), standUpDelay);
    }

    /// <summary>Call this (or use ContextMenu) when you want Father to stand up.</summary>
    [ContextMenu("Trigger Stand Up Now")]
    public void TriggerStandUp()
    {
        if (fatherAnimator == null) return;
        fatherAnimator.SetBool("isSitting", false);
    }

    void AttachShoppingList()
    {
        if (shoppingListProp == null || rightHandBone == null)
        {
            Debug.LogWarning("[SittingStartSequence] shoppingListProp or rightHandBone not assigned.", this);
            return;
        }

        shoppingListProp.transform.SetParent(rightHandBone, false);
        shoppingListProp.transform.localPosition = holdPositionOffset;
        shoppingListProp.transform.localRotation = Quaternion.Euler(holdRotationOffset);
    }

    [ContextMenu("Re-Attach Shopping List to Hand")]
    void EditorAttach() => AttachShoppingList();
}
