// Assets/Editor/ProjectFolderSetup.cs
// Run via: Tools > Setup > Create Project Folder Structure
// Safe to run multiple times — skips folders that already exist.

using UnityEditor;
using UnityEngine;
using System.IO;

public static class ProjectFolderSetup
{
    private static readonly string[] Folders =
    {
        // ── Root container ───────────────────────────────────────────────────
        "Assets/_Project",

        // ── Animations ───────────────────────────────────────────────────────
        "Assets/_Project/Animations",
        "Assets/_Project/Animations/Characters",      // Mixamo .anim clips go here
        "Assets/_Project/Animations/Controllers",     // Animator Controllers
        "Assets/_Project/Animations/Environment",

        // ── Art source files ─────────────────────────────────────────────────
        "Assets/_Project/Art",
        "Assets/_Project/Art/Characters",
        "Assets/_Project/Art/Characters/Avaturn",     // GLB / raw Avaturn exports
        "Assets/_Project/Art/Characters/Rigs",        // Blender-exported FBX rigs
        "Assets/_Project/Art/Characters/Mixamo",      // Mixamo FBX (anim-only)
        "Assets/_Project/Art/Environment",
        "Assets/_Project/Art/Props",
        "Assets/_Project/Art/UI",

        // ── Audio ─────────────────────────────────────────────────────────────
        "Assets/_Project/Audio",
        "Assets/_Project/Audio/Music",
        "Assets/_Project/Audio/SFX",
        "Assets/_Project/Audio/Voice",
        "Assets/_Project/Audio/Mixers",

        // ── Materials & Shaders ───────────────────────────────────────────────
        "Assets/_Project/Materials",
        "Assets/_Project/Materials/Characters",
        "Assets/_Project/Materials/Environment",
        "Assets/_Project/Materials/Props",
        "Assets/_Project/Materials/UI",
        "Assets/_Project/Shaders",

        // ── Prefabs ───────────────────────────────────────────────────────────
        "Assets/_Project/Prefabs",
        "Assets/_Project/Prefabs/Characters",
        "Assets/_Project/Prefabs/Environment",
        "Assets/_Project/Prefabs/Props",
        "Assets/_Project/Prefabs/UI",
        "Assets/_Project/Prefabs/XR",                // XR Rig, hand prefabs, etc.

        // ── Scenes ────────────────────────────────────────────────────────────
        "Assets/_Project/Scenes",
        "Assets/_Project/Scenes/Main",
        "Assets/_Project/Scenes/Testing",
        "Assets/_Project/Scenes/Prototypes",

        // ── Scripts ───────────────────────────────────────────────────────────
        "Assets/_Project/Scripts",
        "Assets/_Project/Scripts/Core",
        "Assets/_Project/Scripts/Characters",
        "Assets/_Project/Scripts/Interactions",
        "Assets/_Project/Scripts/Managers",
        "Assets/_Project/Scripts/UI",
        "Assets/_Project/Scripts/XR",
        "Assets/_Project/Scripts/Utilities",

        // ── Project settings assets ───────────────────────────────────────────
        "Assets/_Project/Settings",                   // URP Renderer Data, Input Actions

        // ── Textures ──────────────────────────────────────────────────────────
        "Assets/_Project/Textures",
        "Assets/_Project/Textures/Characters",
        "Assets/_Project/Textures/Environment",
        "Assets/_Project/Textures/Props",
        "Assets/_Project/Textures/UI",
        "Assets/_Project/Textures/Skyboxes",

        // ── XR-specific ───────────────────────────────────────────────────────
        "Assets/_Project/XR",
        "Assets/_Project/XR/InputActions",            // .inputactions asset
        "Assets/_Project/XR/Rigs",                    // XR Origin variants
        "Assets/_Project/XR/HandPoses",
    };

    [MenuItem("Tools/Setup/Create Project Folder Structure")]
    public static void CreateFolderStructure()
    {
        int created = 0;
        int skipped = 0;

        foreach (string folderPath in Folders)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.Log($"[FolderSetup] Already exists: {folderPath}");
                skipped++;
                continue;
            }

            string parent     = Path.GetDirectoryName(folderPath)!.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            string guid       = AssetDatabase.CreateFolder(parent, folderName);

            if (!string.IsNullOrEmpty(guid))
            {
                Debug.Log($"[FolderSetup] Created: {folderPath}");
                created++;
            }
            else
            {
                Debug.LogError($"[FolderSetup] FAILED to create: {folderPath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"[FolderSetup] Done — {created} created, {skipped} already existed.");
        EditorUtility.DisplayDialog(
            "Folder Structure",
            $"Complete!\n\n{created} folders created\n{skipped} already existed",
            "OK");
    }

    // Validate menu item — always enabled
    [MenuItem("Tools/Setup/Create Project Folder Structure", true)]
    private static bool ValidateCreateFolderStructure() => true;
}
