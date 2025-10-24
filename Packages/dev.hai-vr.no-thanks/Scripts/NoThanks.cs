using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase.Editor.Api;
using Tools = VRC.Tools;

public class NoThanks
{
    [MenuItem("CONTEXT/PipelineManager/No Thanks - Assign New Avatar ID")]
    public static void AssignAvatarIDPipelineManager(MenuCommand command) => AssignNewId(((PipelineManager)command.context).transform);

    [MenuItem("CONTEXT/VRCAvatarDescriptor/No Thanks - Assign New Avatar ID")]
    public static void AssignAvatarIDVRCAvatarDescriptor(MenuCommand command) => AssignNewId(((PipelineManager)command.context).transform);

    [MenuItem("Tools/No Thanks - Assign New Avatar ID")]
    public static void AssignAvatarID(MenuCommand command) => AssignNewId(Selection.activeTransform);

    [MenuItem("Tools/No Thanks - Assign New Avatar ID", true)]
    public static bool AssignAvatarIDValidate(MenuCommand command)
    {
        var pipelineManager = Selection.activeTransform?.GetComponent<PipelineManager>();
        if (pipelineManager == null) Selection.activeTransform?.GetComponentInParent<PipelineManager>(true);
        if (pipelineManager == null) return false;

        return string.IsNullOrWhiteSpace(pipelineManager.blueprintId);
    }

    private static async Task AssignNewId(Transform context)
    {
        var pipelineManager = context?.GetComponent<PipelineManager>();
        if (pipelineManager == null) context?.GetComponentInParent<PipelineManager>(true);
        if (pipelineManager == null) return;
        
        var result = await BoganAvatarRecord(new VRCAvatar
        {
            Name = "temp",
            Description = "temp",
            Tags = new List<string>(),
            ReleaseStatus = "private"
        }).ConfigureAwait(false);

        pipelineManager.blueprintId = result.ID;
    }

    private static async Task<VRCAvatar> BoganAvatarRecord(VRCAvatar data, CancellationToken cancellationToken = default)
    {
        var newAvatarData = new Dictionary<string, object>
        {
            {"name", data.Name},
            {"description", data.Description},
            {"tags", data.Tags},
            {"releaseStatus", data.ReleaseStatus},
            {"platform", Tools.Platform},
            {"unityVersion", Tools.UnityVersion.ToString()},
            {"assetVersion", 1}
        };
        return await VRCApi.Post<Dictionary<string, object>, VRCAvatar>("avatars", newAvatarData, cancellationToken: cancellationToken);
    }
}
