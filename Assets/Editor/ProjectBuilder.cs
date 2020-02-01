using UnityEditor;
using UnityEngine;

public class Builds
{
    const string BASE_PATH = "Builds/";
    static string[] gameLevels = new[]
    {
        "Assets/Scenes/SampleScene.unity"
    };
    
    [MenuItem("Builds/Windows")]
    public static void BuildWindows()
    {
        PlayerSettings.runInBackground = true;
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"{BASE_PATH}Windows/Erutan.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"Windows build complete");
        else
            Debug.LogError($"Error building Windows { message }");
    }
    
    [MenuItem("Builds/Linux %#L")]
    public static void BuildLinux()
    {
        PlayerSettings.runInBackground = true;
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"{BASE_PATH}Linux/Erutan.x86_64",
            BuildTarget.StandaloneLinux64,
            BuildOptions.None);

        if (message)
            Debug.Log($"Linux build complete");
        else
            Debug.LogError($"Error building Linux { message }");
    }
    
    [MenuItem("Builds/Web")]
    public static void BuildWeb()
    {
        PlayerSettings.runInBackground = true;
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"{BASE_PATH}Web/",
            BuildTarget.WebGL,
            BuildOptions.None);

        if (message)
            Debug.Log($"WebGL build complete");
        else
            Debug.LogError($"Error building WebGL { message }");
    }

    [MenuItem("Builds/Android %#A")]
    public static void BuildAndroid()
    {
        PlayerSettings.runInBackground = true;
        EditorPrefs.SetString("AndroidSdkRoot", System.Environment.GetEnvironmentVariable("ANDROID_HOME"));
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"{BASE_PATH}Android/Erutan.apk",
            BuildTarget.Android,
            BuildOptions.None);

        if (message)
            Debug.Log($"Android build complete");
        else
            Debug.LogError($"Error building Android { message }");
    }
    
    // Seems to be runnable from bash
    // "C:\Program Files\Unity\Editor\Unity.exe -quit -batchmode -executeMethod BuildWin64NoDRMWorldwide"
    [MenuItem("Builds/PC All Platforms")]
    public static void BuildAllPc() {
        BuildWindows();
        BuildLinux();
    }
}