using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to: SYS_SymptomOrchestrator
/// Controls all 5 symptoms from two variables:
/// diseaseProgression and anxietyLevel
/// </summary>
public class DementiaSymptomOrchestrator : MonoBehaviour
{
    [Header("All 5 Symptom Scripts")]
    public ShufflingGait        shufflingGait;
    public FidgetLayer          fidgetLayer;
    public CognitiveDelaySystem cogDelay;
    public EyeContactController eyeContact;
    public FlatAffectController flatAffect;

    [Header("Disease State — adjust per scene")]
    [Range(0, 1)]
    public float diseaseProgression = 0.6f; // 0=early, 1=severe

    [Range(0, 2)]
    public float anxietyLevel    = 0f;  // rises during scene
    public float anxietyRiseTime = 45f; // seconds to peak
    public float anxietyPeakValue= 1.5f;

    // ── Start ─────────────────────────────────────────
    void Start()
    {
        ApplyDisease();
        StartCoroutine(RiseAnxiety());
    }

    // ── Apply Disease Progression ─────────────────────
    void ApplyDisease()
    {
        // Cognitive delay — more disease = longer delays
        if (cogDelay)
            cogDelay.cognitiveLoad = diseaseProgression;

        // Eye contact — more disease = more avoidance
        if (eyeContact)
        {
            eyeContact.gazeOffsetMagnitude =
                Mathf.Lerp(0.15f, 0.55f, diseaseProgression);

            eyeContact.confusionLevel =
                diseaseProgression;

            eyeContact.directGazeChance =
                Mathf.Lerp(0.3f, 0.05f, diseaseProgression);
        }

        // Flat affect — more disease = more suppression
        if (flatAffect)
            flatAffect.suppressionLevel =
                Mathf.Lerp(0.4f, 0.9f, diseaseProgression);
    }

    // ── Anxiety Rise Over Time ────────────────────────
    IEnumerator RiseAnxiety()
    {
        float t = 0;

        while (t < anxietyRiseTime)
        {
            t += Time.deltaTime;

            // Anxiety rises from 0 to peak over time
            anxietyLevel = Mathf.Lerp(
                0,
                anxietyPeakValue,
                t / anxietyRiseTime);

            // Send anxiety to fidget system
            //if (fidgetLayer)
              //  fidgetLayer.anxietyLevel = anxietyLevel;

            // Faster gaze shifts as anxiety increases
            if (eyeContact)
                eyeContact.gazeShiftInterval =
                    Mathf.Lerp(4.5f, 1.5f, anxietyLevel / 2f);

            yield return null;
        }
    }

    // ── Test Methods ──────────────────────────────────
    [ContextMenu("Test Early Stage (0.2)")]
    void TestEarlyStage()
    {
        diseaseProgression = 0.2f;
        ApplyDisease();
        Debug.Log("Early stage Alzheimer's applied");
    }

    [ContextMenu("Test Moderate Stage (0.6)")]
    void TestModerateStage()
    {
        diseaseProgression = 0.6f;
        ApplyDisease();
        Debug.Log("Moderate stage Alzheimer's applied");
    }

    [ContextMenu("Test Severe Stage (1.0)")]
    void TestSevereStage()
    {
        diseaseProgression = 1.0f;
        ApplyDisease();
        Debug.Log("Severe stage Alzheimer's applied");
    }

    [ContextMenu("Test Max Anxiety")]
    void TestMaxAnxiety()
    {
        anxietyLevel = 2.0f;
        if (fidgetLayer)  fidgetLayer.anxietyLevel  = 2.0f;
        if (eyeContact)   eyeContact.gazeShiftInterval = 1.0f;
        Debug.Log("Maximum anxiety applied");
    }

    [ContextMenu("Reset All")]
    void ResetAll()
    {
        anxietyLevel = 0f;
        if (fidgetLayer)  fidgetLayer.anxietyLevel  = 0f;
        if (eyeContact)   eyeContact.gazeShiftInterval = 4.5f;
        Debug.Log("All symptoms reset");
    }
}
