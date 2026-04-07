using UnityEngine;
using UnityEditor;

public static class FingerPoseFix
{
    [MenuItem("Tools/Fix Father Right Hand Finger Pose")]
    public static void FixFingerPose()
    {
        string clipPath = "Assets/Characters/Father/Animations/AnimatorController/Sitting Idle.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            Debug.LogError("Could not load clip: " + clipPath);
            return;
        }

        float duration = clip.length;

        // Humanoid muscle curves for right hand fingers
        // Value range: -1 (fully curled) to 1 (fully extended)
        // For holding flat paper: slight curl on fingers, thumb pressed in
        var fingerCurves = new (string muscle, float value)[]
        {
            // Index
            ("RightHand.Index.1 Stretched",  -0.4f),
            ("RightHand.Index.2 Stretched",  -0.4f),
            ("RightHand.Index.3 Stretched",  -0.3f),
            ("RightHand.Index.Spread",        0.0f),
            // Middle
            ("RightHand.Middle.1 Stretched", -0.4f),
            ("RightHand.Middle.2 Stretched", -0.4f),
            ("RightHand.Middle.3 Stretched", -0.3f),
            ("RightHand.Middle.Spread",       0.0f),
            // Ring
            ("RightHand.Ring.1 Stretched",   -0.35f),
            ("RightHand.Ring.2 Stretched",   -0.35f),
            ("RightHand.Ring.3 Stretched",   -0.3f),
            ("RightHand.Ring.Spread",         0.0f),
            // Little (Pinky)
            ("RightHand.Little.1 Stretched", -0.3f),
            ("RightHand.Little.2 Stretched", -0.3f),
            ("RightHand.Little.3 Stretched", -0.25f),
            ("RightHand.Little.Spread",       0.0f),
            // Thumb - press onto paper
            ("RightHand.Thumb.1 Stretched",  -0.2f),
            ("RightHand.Thumb.2 Stretched",  -0.3f),
            ("RightHand.Thumb.3 Stretched",  -0.2f),
            ("RightHand.Thumb.Spread",        0.3f),
        };

        foreach (var (muscle, value) in fingerCurves)
        {
            AnimationCurve curve = AnimationCurve.Linear(0f, value, duration, value);
            for (int i = 0; i < curve.keys.Length; i++)
            {
                var key = curve.keys[i];
                key.inTangent = 0f;
                key.outTangent = 0f;
                curve.MoveKey(i, key);
            }

            EditorCurveBinding binding = new EditorCurveBinding
            {
                path = "",
                type = typeof(Animator),
                propertyName = muscle
            };
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();
        Debug.Log("[FingerPoseFix] Right hand finger pose applied to Sitting Idle.anim");
    }
}
