using UnityEditor;
using UnityEngine;

namespace GHSS.EditorTools
{
    /// <summary>
    /// Applies the WebGL build settings for the test-assignment submission:
    /// Development Build off, no debug symbols, no compression (works from any
    /// static host with zero server configuration), Main scene as the one and
    /// only build scene. Touches only Player/Build Settings - never the scene
    /// content itself.
    /// </summary>
    internal static class WebGLPlayerSettingsSetup
    {
        private const string MainScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("GHSS/Setup/Apply WebGL Build Settings")]
        private static void ApplySettings()
        {
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = false;

            PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainScenePath, true)
            };

            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                "WebGL build settings applied",
                "Development Build: off\n" +
                "Debug Symbols: off\n" +
                "Compression Format: Disabled\n" +
                "Scenes in Build: " + MainScenePath + " (only)\n\n" +
                "Scene content was not touched.",
                "OK");
        }
    }
}
