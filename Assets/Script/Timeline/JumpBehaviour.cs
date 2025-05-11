using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class JumpBehaviour:PlayableBehaviour
{
    public Transform jumpObject;
    public float jumpHeight = 1f;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float jumpTime = (float)(playable.GetTime() / playable.GetDuration());
        var position = jumpObject.position;
        Vector3 jumpPosition = position + Vector3.up *jumpHeight;
        jumpObject.position = jumpPosition;
        Debug.Log($"Jumping jumpHeight: {jumpHeight}");
    }
}