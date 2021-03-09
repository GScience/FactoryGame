using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BuildHelper
{
    private static string GetOutputPath()
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; ++i)
        {
            if (args[i].ToLower() == "-outputpath" && i < args.Length - 1)
                return args[i + 1];
        }
        throw new ArgumentException("Not define output");
    }

    private static string GetOutputName()
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; ++i)
        {
            if (args[i].ToLower() == "-outputname" && i < args.Length - 1)
                return args[i + 1];
        }
        throw new ArgumentException("Not define output");
    }

    private static void BuildAssetBundles(BuildTarget target, string path)
    {
        Debug.Log("[BuildHelper]Start building asset bundles in path " + path);
        var ab = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ForceRebuildAssetBundle, target);
        if (ab == null)
            throw new Exception("Failed to build asset bundles to " + path);
        Debug.Log("[BuildHelper]Asset bundles are built");
    }

    [MenuItem("Assets/Create Asset Bundles")]
    public static void BuildAssetBundlesMenu()
    {
        var outputPath = Application.dataPath + "/AssetBundles/";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildAssetBundles(BuildTarget.StandaloneWindows, Application.dataPath + "/AssetBundles/");
    }

    private static void Build(BuildTarget target)
    {
        var outputPath = GetOutputPath();
        var outputName = GetOutputName();

        // build ab
        BuildAssetBundles(target, outputPath + "/" + outputName + "_Data/AssetBundles/");

        // build player
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = (from scene in EditorBuildSettings.scenes select scene.path).ToArray();
        options.target = target;
        options.locationPathName = outputPath + "/" + outputName + ".exe";

        var playerBuildReport = BuildPipeline.BuildPlayer(options);

        if (playerBuildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception("Failed to build player!" + playerBuildReport.summary);
    }

    public static void BuildWindows()
    {
        try
        {
            Build(BuildTarget.StandaloneWindows);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw e;
        }
    }
}
