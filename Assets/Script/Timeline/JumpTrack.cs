using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.5f, 0.5f, 0.5f)]
[TrackClipType(typeof(JumpClip))]
public class JumpTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<JumpMixer>.Create(graph, inputCount);
        return mixer;
    }
}
