using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreateAssetBundles
{
    private const string assetBundleDirectory = "Assets/AssetBundles";
    private const string targetDirectory = "../1.6/AssetBundles";

    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        BuildFor(BuildTarget.StandaloneWindows64);
        BuildFor(BuildTarget.StandaloneLinux64);
        BuildFor(BuildTarget.StandaloneOSX);
    }

    private static void BuildFor(BuildTarget target)
    {
        string fullDir = Path.Combine(assetBundleDirectory, $"{target}");
        if (!Directory.Exists(fullDir))
        {
            Directory.CreateDirectory(fullDir);
        }
        AssetBundleManifest output = BuildPipeline.BuildAssetBundles(fullDir, BuildAssetBundleOptions.ChunkBasedCompression, target);
        foreach (string bundle in output.GetAllAssetBundles())
        {
            string bundleFile = Path.Combine(fullDir, bundle);
            string manifestFile = Path.Combine(fullDir, $"{bundle}.manifest");
            string targetBundleFile = Path.Combine(targetDirectory, $"{bundle}_{target.GetPostfix()}");
            string targetManifestFile = Path.Combine(targetDirectory, $"{bundle}_{target.GetPostfix()}.manifest");
            File.Copy(bundleFile, targetBundleFile, true);
            File.Copy(manifestFile, targetManifestFile, true);
        }
    }

    private static string GetPostfix(this BuildTarget target)
    {
        return target switch
        {
            BuildTarget.StandaloneWindows64 => "win",
            BuildTarget.StandaloneLinux64 => "linux",
            BuildTarget.StandaloneOSX => "mac",
            _ => ""
        };
    }
}