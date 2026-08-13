using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GHSS.EditorTools
{
    /// <summary>
    /// Android player settings + APK build for the test-assignment submission.
    /// Settings (identifier, ARM64, IL2CPP, orientation) can be applied even
    /// without the Android module installed - they're just data written into
    /// ProjectSettings.asset. Only the actual build requires the module, so
    /// that step checks for it first and fails with a clear message instead of
    /// a cryptic Unity error if it's missing.
    /// </summary>
    internal static class AndroidBuildSetup
    {
        private const string MainScenePath = "Assets/Scenes/Main.unity";
        private const string ApplicationIdentifier = "com.ghss.mergegame";
        private const string ReleaseApkPath = "Build/Android/GHSS.apk";
        private const string DevelopmentApkPath = "Build/Android/GHSS-dev.apk";

        [MenuItem("GHSS/Setup/Apply Android Player Settings")]
        private static void ApplyPlayerSettings()
        {
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationIdentifier);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            EditorUtility.DisplayDialog(
                "Android player settings applied",
                "Application Identifier: " + ApplicationIdentifier + "\n" +
                "Target Architecture: ARM64\n" +
                "Scripting Backend: IL2CPP\n" +
                "Min SDK: " + PlayerSettings.Android.minSdkVersion + " (left unchanged)\n" +
                "Target SDK: " + PlayerSettings.Android.targetSdkVersion + " (left unchanged - Automatic)\n" +
                "Orientation: Portrait",
                "OK");
        }

        [MenuItem("GHSS/Build/Android APK (Release)")]
        private static void BuildRelease()
        {
            if (!RequireAndroidModule()) return;

            ApplyPlayerSettingsSilently();
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.buildAppBundle = false; // .apk, not .aab

            RunBuild(ReleaseApkPath, BuildOptions.None);
        }

        [MenuItem("GHSS/Build/Android APK (Development)")]
        private static void BuildDevelopment()
        {
            if (!RequireAndroidModule()) return;

            ApplyPlayerSettingsSilently();
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.buildAppBundle = false;

            RunBuild(DevelopmentApkPath, BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        private static bool RequireAndroidModule()
        {
            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
                return true;

            EditorUtility.DisplayDialog(
                "Android Build Support missing",
                "The Android module isn't installed for this Unity Editor version.\n\n" +
                "Unity Hub -> Installs -> this Editor version -> gear icon -> Add Modules -> " +
                "Android Build Support (tick SDK, NDK and OpenJDK sub-items too) -> Install, " +
                "then restart the Editor and run this again.",
                "OK");
            return false;
        }

        private static void ApplyPlayerSettingsSilently()
        {
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationIdentifier);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        }

        private static void RunBuild(string outputPath, BuildOptions options)
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { MainScenePath },
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = options
            });

            var summary = report.summary;
            Debug.Log(
                $"GHSS_ANDROID_BUILD_RESULT={summary.result};errors={summary.totalErrors};" +
                $"warnings={summary.totalWarnings};size={summary.totalSize};time={summary.totalTime}");

            if (summary.result != BuildResult.Succeeded)
                throw new Exception($"Android build failed: {summary.result} (errors={summary.totalErrors})");
        }
    }
}
