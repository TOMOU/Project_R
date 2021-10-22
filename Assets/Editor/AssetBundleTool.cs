// ==================================================
// AssetBundleTool.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.IO;
using UnityEngine;
using UnityEditor;

public class AssetBundleTool
{
    [MenuItem("Tools/AssetBundle/Build AssetBundle - Android")]
    private static void BuildAllAssetBundles_Android()
    {
        string path = "Assets/AssetBundles/Android";
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    [MenuItem("Tools/AssetBundle/Build AssetBundle - IOS")]
    private static void BuildAllAssetBundles_IOS()
    {
        string path = "Assets/AssetBundles/IOS";
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    [MenuItem("Tools/Generic Build/To Windows", false, 1)]
    private static void Build_Windows()
    {
        GenericBuild("Windows");
    }

    [MenuItem("Tools/Generic Build/To OSX", false, 2)]
    private static void Build_OSX()
    {
        GenericBuild("OSX");
    }

    [MenuItem("Tools/Generic Build/To Android", false, 3)]
    private static void Build_Android()
    {
        GenericBuild("Android");
    }

    [MenuItem("Tools/Generic Build/To iOS", false, 4)]
    private static void Build_iOS()
    {
        GenericBuild("iOS");
    }

    private static void GenericBuild(string platform)
    {
        // Get Current Time
        System.DateTime now = System.DateTime.Now;
        string tc = string.Format("{3}_Project_R_{0:D2}{1:D2}{2:D2}", now.Year % 100, now.Month, now.Day, platform);

        // Create Root Folder (Program/Build)
        string path = Directory.GetParent(Application.dataPath).Parent.FullName + "/Build";

        int tryCount = 1;

        if (platform == "Windows")
        {
            while (Directory.Exists(path + "/" + tc + string.Format("_{0}", tryCount)) == true)
            {
                tryCount++;
            }

            tc += string.Format("_{0}", tryCount);

            // Create Sub-Root Folder(Program/Build/Windows)
            if (Directory.Exists(path) == false)
            {
                path = Directory.CreateDirectory(path).FullName + "/" + tc;
            }
            else
            {
                path = path + "/" + tc;
            }

            // Create Platform Folder(Program/Build/Windows/YYYYMMDDHHMM)
            if (Directory.Exists(path) == false)
            {
                path = Directory.CreateDirectory(path).FullName + "/";
            }
            else
            {
                path = path + "/";
            }
        }
        else
        {
            while (File.Exists(path + "/" + tc + string.Format("_{0}.apk", tryCount)) == true)
            {
                tryCount++;
            }

            tc += string.Format("_{0}", tryCount);

            if (Directory.Exists(path) == false)
            {
                path = Directory.CreateDirectory(path).FullName + "/";
            }
            else
            {
                path += "/";
            }
        }

        BuildTargetGroup group = BuildTargetGroup.Unknown;
        BuildTarget target = BuildTarget.NoTarget;

        switch (platform)
        {
            case "Windows":
                group = BuildTargetGroup.Standalone;
                target = BuildTarget.StandaloneWindows;
                break;

            case "OSX":
                group = BuildTargetGroup.Standalone;
                target = BuildTarget.StandaloneOSX;
                break;

            case "Android":
                group = BuildTargetGroup.Android;
                target = BuildTarget.Android;
                break;

            case "iOS":
                group = BuildTargetGroup.iOS;
                target = BuildTarget.iOS;
                break;
        }

        if (group == BuildTargetGroup.Unknown || target == BuildTarget.NoTarget)
        {
            Debug.LogWarningFormat("Can't build process in {0}", platform);
        }

        BuildPlayer(group, target, tc, path);
    }

    static void BuildPlayer(BuildTargetGroup group, BuildTarget target, string filename, string path)
    {
        Debug.LogFormat("Build {0} target in {1}", target, path);

        string fileExtension = "";

        // configure path variables based on the platform we're targeting
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                fileExtension = ".exe";
                break;

            case BuildTarget.StandaloneOSX:
                fileExtension = ".app";
                break;

            case BuildTarget.Android:
                fileExtension = ".apk";
                break;

            case BuildTarget.iOS:
                fileExtension = ".ipa";
                break;
        }

        EditorUserBuildSettings.SwitchActiveBuildTargetAsync(group, target);

        // build out the player
        string playerPath = path + filename + fileExtension;
        BuildPipeline.BuildPlayer(GetScenePaths(), playerPath, target, BuildOptions.None);
    }

    static void CopyFromProjectAssets(string fullDataPath, string assetsFolderPath, bool deleteMetaFiles = true)
    {
        Debug.Log("CopyFromProjectAssets: copying over " + assetsFolderPath);
        FileUtil.ReplaceDirectory(Application.dataPath + "/" + assetsFolderPath, fullDataPath + assetsFolderPath); // copy over languages

        // delete all meta files
        if (deleteMetaFiles)
        {
            var metaFiles = Directory.GetFiles(fullDataPath + assetsFolderPath, "*.meta", SearchOption.AllDirectories);
            foreach (var meta in metaFiles)
            {
                FileUtil.DeleteFileOrDirectory(meta);
            }
        }
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

    static string GetProjectFolderPath()
    {
        var s = UnityEngine.Application.dataPath;
        s = s.Substring(s.Length - 7, 7);
        return s;
    }
}