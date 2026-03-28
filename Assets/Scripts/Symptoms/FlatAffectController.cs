using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to: CHAR_Father_Patient
/// Suppresses facial BlendShapes to create flat affect.
/// Requires Avaturn character with BlendShapes.
/// </summary>
public class FlatAffectController : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer faceMesh; // Drag Head_Mesh here

    [Header("Settings")]
    [Range(0f, 1f)]
    public float suppressionLevel     = 0.85f; // 0=normal, 1=no expression
    public float suppressionSpeed     = 0.8f;  // How fast expression fades
    public float microExpressionChance= 0.05f; // 5% chance per second

    // ── Private ───────────────────────────────────────
    private int     _count;
    private float[] _current;

    // ── Start ─────────────────────────────────────────
    void Start()
    {
        if (!faceMesh) return;

        // Count how many blendshapes exist
        _count   = faceMesh.sharedMesh.blendShapeCount;
        _current = new float[_count];

        // Read starting blendshape values
        for (int i = 0; i < _count; i++)
            _current[i] = faceMesh.GetBlendShapeWeight(i);

        StartCoroutine(MicroExpressions());
    }

    // ── Update ────────────────────────────────────────
    void Update()
    {
        if (!faceMesh) return;

        for (int i = 0; i < _count; i++)
        {
            // What the animation wants to show
            float natural = faceMesh.GetBlendShapeWeight(i);

            // Scale it down toward zero
            float suppressed = natural * (1f - suppressionLevel);

            // Smoothly move current toward suppressed
            _current[i] = Mathf.MoveTowards(
                _current[i],
                suppressed,
                suppressionSpeed * 100f * Time.deltaTime);

            // Apply the suppressed value
            faceMesh.SetBlendShapeWeight(i, _current[i]);
        }
    }

    // ── Micro Expressions ─────────────────────────────
    IEnumerator MicroExpressions()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (UnityEngine.Random.value < microExpressionChance)
            {
                // Brief flicker of real expression
                float original    = suppressionLevel;
                suppressionLevel  = 0.15f;

                yield return new WaitForSeconds(0.4f);

                // Flat affect returns
                suppressionLevel  = original;
            }
        }
    }

    // ── Test Methods ──────────────────────────────────
    [ContextMenu("Test Full Suppression")]
    void TestFullSuppression()
    {
        suppressionLevel = 1.0f;
        Debug.Log("Full flat affect — no expression");
    }

    [ContextMenu("Test Micro Expression")]
    void TestMicroExpression()
    {
        StartCoroutine(ForceMicroExpression());
    }

    IEnumerator ForceMicroExpression()
    {
        float original   = suppressionLevel;
        suppressionLevel = 0.15f;
        Debug.Log("Micro expression firing!");

        yield return new WaitForSeconds(0.4f);

        suppressionLevel = original;
        Debug.Log("Flat affect returned");
    }

    [ContextMenu("Reset to Normal")]
    void ResetNormal()
    {
        suppressionLevel = 0f;
        Debug.Log("Expression suppression removed");
    }
}
