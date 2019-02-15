using System;
using UnityEngine;

/// <summary>
/// 参考：http://qiita.com/RyotaMurohoshi/items/bb5ba4339dc8a1186290
/// </summary>

[Serializable]
public class CloudBuildManifest
{
    public static CloudBuildManifest Load()
    {
        var json = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
        if (json == null)
        {
            return null;
        }
        else
        {
            return JsonUtility.FromJson<CloudBuildManifest>(json.text);
        }
    }

    [SerializeField]
    string scmCommitId;
    public string ScmCommitId { get { return scmCommitId; } }

    [SerializeField]
    string scmBranch;
    public string ScmBranch { get { return scmBranch; } }

    [SerializeField]
    string buildNumber;
    public string BuildNumber { get { return buildNumber; } }

    [SerializeField]
    string buildStartTime;
    public string BuildStartTime { get { return buildStartTime; } }

    [SerializeField]
    string projectId;
    public string ProjectId { get { return projectId; } }

    [SerializeField]
    string bundleId;
    public string BundleId { get { return bundleId; } }

    [SerializeField]
    string unityVersion;
    public string UnityVersion { get { return unityVersion; } }

    [SerializeField]
    string xcodeVersion;
    public string XCodeVersion { get { return xcodeVersion; } }

    [SerializeField]
    string cloudBuildTargetName;
    public string CloudBuildTargetName { get { return cloudBuildTargetName; } }
}