using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to: CHAR_Father_Patient
/// Controls all 5 symptoms from two variables: diseaseProgression and anxietyLevel.
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
    public float diseaseProgression = 0.6f;

    [Range(0, 2)]
    public float anxietyLevel     = 0f;
    public float anxietyRiseTime  = 45f;
    public float anxietyPeakValue = 1.5f;

    // ── Awake — auto-wire sibling components ──────────
    void Awake()
    {
        if (!shufflingGait) shufflingGait = GetComponent<ShufflingGait>();
        if (!fidgetLayer)   fidgetLayer   = GetComponent<FidgetLayer>();
        if (!cogDelay)      cogDelay      = GetComponent<CognitiveDelaySystem>();
        if (!eyeContact)    eyeContact    = GetComponent<EyeContactController>();
        if (!flatAffect)    flatAffect    = GetComponent<FlatAffectController>();
    }

    // ── Start ─────────────────────────────────────────
    void Start()
    {
        ApplyDisease();
        StartCoroutine(RiseAnxiety());
    }

    // ── Apply Disease Progression ─────────────────────
    void ApplyDisease()
    {
        if (fidgetLayer)
            fidgetLayer.anxietyLevel = anxietyLevel;

        if (cogDelay)
            cogDelay.cognitiveLoad = diseaseProgression;

        if (eyeContact)
        {
            eyeContact.gazeOffsetMagnitude = Mathf.Lerp(0.15f, 0.55f, diseaseProgression);
            eyeContact.confusionLevel      = diseaseProgression;
            eyeContact.directGazeChance    = Mathf.Lerp(0.3f, 0.05f, diseaseProgression);
        }

        if (flatAffect)
            flatAffect.suppressionLevel = Mathf.Lerp(0.4f, 0.9f, diseaseProgression);
    }

    // ── Anxiety Rise Over Time ────────────────────────
    IEnumerator RiseAnxiety()
    {
        float t = 0;
        while (t < anxietyRiseTime)
        {
            t += Time.deltaTime;
            anxietyLevel = Mathf.Lerp(0, anxietyPeakValue, t / anxietyRiseTime);

            if (fidgetLayer)
                fidgetLayer.anxietyLevel = anxietyLevel;

            if (eyeContact)
                eyeContact.gazeShiftInterval = Mathf.Lerp(4.5f, 1.5f, anxietyLevel / 2f);

            yield return null;
        }
    }

    // ── Test Methods ──────────────────────────────────
    [ContextMenu("Test Early Stage (0.2)")]
    void TestEarlyStage()    { diseaseProgression = 0.2f; ApplyDisease(); }

    [ContextMenu("Test Moderate Stage (0.6)")]
    void TestModerateStage() { diseaseProgression = 0.6f; ApplyDisease(); }

    [ContextMenu("Test Severe Stage (1.0)")]
    void TestSevereStage()   { diseaseProgression = 1.0f; ApplyDisease(); }

    [ContextMenu("Test Max Anxiety")]
    void TestMaxAnxiety()
    {
        anxietyLevel = 2.0f;
        if (fidgetLayer) fidgetLayer.anxietyLevel = 2.0f;
        if (eyeContact)  eyeContact.gazeShiftInterval = 1.0f;
    }

    [ContextMenu("Reset All")]
    void ResetAll()
    {
        anxietyLevel = 0f;
        if (fidgetLayer) fidgetLayer.anxietyLevel = 0f;
        if (eyeContact)  eyeContact.gazeShiftInterval = 4.5f;
    }
}
