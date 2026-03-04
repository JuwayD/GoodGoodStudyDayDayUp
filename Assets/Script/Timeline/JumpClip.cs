using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class JumpClip:PlayableAsset, ITimelineClipAsset
{
    public JumpBehaviour template = new JumpBehaviour();
    public ClipCaps clipCaps { get { return ClipCaps.All; } }
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<JumpBehaviour>.Create(graph, template);
        return playable;
        var state = new AnimationState();
    }

}