using System.Collections;
using UnityEngine;


/// Simulates avoidant eye contact — gaze drifts past

public class EyeContactController : MonoBehaviour
{
    [Header("References")]
    public Transform gazeTarget;   // Drag TARGET_GazePoint
    public Transform userCamera;   // Drag Main Camera or CenterEyeAnchor

    [Header("Gaze Settings")]
    public float gazeOffsetMagnitude = 0.35f; // How far gaze misses user (metres)
    public float gazeShiftInterval   = 4.0f;  // Seconds between gaze changes
    public float directGazeChance    = 0.15f; // 15% chance of direct eye contact
    public float confusionLevel      = 0f;    // Increases gaze wandering range

    // ── Private ───────────────────────────────────────
    private Vector3 _offset;
    private Vector3 _targetOffset;
    private float   _timer;

    // Directions gaze wanders to
    private readonly Vector3[] _dirs =
    {
        new Vector3( 0.35f,  0f,     0f), // Left of user
        new Vector3(-0.35f,  0f,     0f), // Right of user
        new Vector3( 0f,    -0.2f,   0f), // Below user
        new Vector3( 0f,     0.25f,  0f), // Above user
        new Vector3( 0.2f,  -0.15f,  0f), // Down-left
    };

    // ── Start ─────────────────────────────────────────
    void Start()
    {
        _timer = gazeShiftInterval * 0.5f;
        SelectTarget();
    }

    // ── Update ────────────────────────────────────────
    void Update()
    {
        if (!gazeTarget || !userCamera) return;

        // Count up to next gaze shift
        _timer += Time.deltaTime;
        if (_timer >= gazeShiftInterval)
        {
            _timer = 0;
            SelectTarget();
        }

        // Smoothly move gaze toward target offset
        _offset = Vector3.MoveTowards(
            _offset,
            _targetOffset,
            1.2f * Time.deltaTime);

        // Apply offset to gaze target position
        gazeTarget.position = userCamera.position + _offset;
    }

    // ── Select New Gaze Target ────────────────────────
    void SelectTarget()
    {
        // Small chance of brief direct eye contact
        if (UnityEngine.Random.value < directGazeChance)
        {
            StartCoroutine(DirectContact());
            return;
        }

        // Pick a random offset direction
        int index = UnityEngine.Random.Range(0, _dirs.Length);

        // Scale offset by confusion level
        float severity = Mathf.Lerp(
            gazeOffsetMagnitude,
            gazeOffsetMagnitude * 2.2f,
            confusionLevel);

        _targetOffset = _dirs[index] * severity;

        // Randomise next shift interval
        gazeShiftInterval = UnityEngine.Random.Range(2.5f, 5.5f);
    }

    // ── Direct Eye Contact ────────────────────────────
    IEnumerator DirectContact()
    {
        // Briefly look directly at user — haunting and rare
        _targetOffset = Vector3.zero;

        yield return new WaitForSeconds(0.8f);

        // Then drift away again
        SelectTarget();
    }

    // ── Test Methods ──────────────────────────────────
    [ContextMenu("Test Direct Gaze")]
    void TestDirectGaze()
    {
        StartCoroutine(DirectContact());
        Debug.Log("Testing direct eye contact — 0.8 seconds");
    }

    [ContextMenu("Test Max Confusion")]
    void TestMaxConfusion()
    {
        confusionLevel = 1f;
        Debug.Log("Confusion level set to maximum — gaze wandering increased");
    }

    [ContextMenu("Reset Confusion")]
    void ResetConfusion()
    {
        confusionLevel = 0f;
        Debug.Log("Confusion level reset to zero");
    }
}