using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GHSS.EditorTools
{
    /// <summary>
    /// Fixes the two objective mobile-portrait problems found in Main.unity:
    /// the camera's orthographic size only fit a landscape-ish aspect (the 7x9
    /// grid would clip on a real phone's narrow portrait screen), and the
    /// Canvas Scaler was left on "Constant Pixel Size" (fixed-pixel UI looks
    /// tiny/wrong across different phone resolutions/DPIs).
    ///
    /// Applies changes to the *currently open* scene through Unity's own APIs
    /// (not by editing Main.unity on disk) - safe to run while the Editor has
    /// the scene open, unlike a direct file edit. Marks the scene dirty; still
    /// needs Ctrl+S afterward, same as any other change made through the Editor.
    ///
    /// Deliberately does not touch UI element positions (TimerPanel/StartButton/
    /// text anchors) - those were hand-tuned; only camera and Canvas Scaler,
    /// which are objectively wrong for mobile regardless of layout choices.
    /// </summary>
    internal static class MobileSceneSetup
    {
        // Fits the 7x9 grid (cells 0..6 x 0..8, plus a little padding) within
        // even a narrow ~9:20 portrait aspect ratio without clipping.
        private const float PortraitOrthographicSize = 9f;

        [MenuItem("GHSS/Setup/Apply Mobile Portrait Scene Settings")]
        private static void ApplySettings()
        {
            var camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
            var canvas = Object.FindFirstObjectByType<Canvas>();

            if (camera == null && canvas == null)
            {
                EditorUtility.DisplayDialog(
                    "Mobile scene setup",
                    "Found neither a Camera nor a Canvas in the open scene. " +
                    "Open Main.unity first.",
                    "OK");
                return;
            }

            var report = "";

            if (camera != null)
            {
                camera.orthographic = true;
                camera.orthographicSize = PortraitOrthographicSize;
                report += $"Camera '{camera.name}': orthographic size -> {PortraitOrthographicSize} (fits the 7x9 grid on a narrow portrait screen).\n";
                EditorUtility.SetDirty(camera);
            }

            var scaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                report += "Canvas Scaler: Scale With Screen Size, reference 1080x1920, match 0.5 (was Constant Pixel Size).\n";
                EditorUtility.SetDirty(scaler);
            }

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            report += "Player Settings: default orientation -> Portrait (was Auto Rotation).\n";

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "Mobile scene setup applied",
                report + "\nScene marked dirty - save it (Ctrl+S) to keep these changes.",
                "OK");
        }
    }
}
