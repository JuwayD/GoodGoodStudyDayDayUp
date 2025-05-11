using UnityEngine;
using UnityEngine.Playables;

public class JumpMixer:PlayableBehaviour
{
    public Transform jumpObject;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();
        var offset = Vector3.zero;
        Transform transform = null;
        for (int i = 0; i < inputCount; i++)
        {
            var inputPlayable = (ScriptPlayable<JumpBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();
            var weight = playable.GetInputWeight(i);
            float jumpTime = (float)(playable.GetTime() / playable.GetDuration());
            offset = (Vector3.up * input.jumpHeight) * weight;
            transform = input.jumpObject;
            Debug.Log("JumpMixer: " + jumpTime + " " + weight + " " + offset + " index: " + i);
        }
        if (transform != null)
        {
            var position1 = transform.position;
            position1 -= offset;
            transform.position = Vector3.zero;
        }
    }
}