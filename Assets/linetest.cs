using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linetest : MonoBehaviour
{
    UILineRenderer lr;
    int tickCount;
    float elapsed;
    string state = "Boot";
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<UILineRenderer>();
        if (lr == null)
        {
            state = "MissingUILineRenderer";
        }
        else
        {
            lr.SetPositions(new Vector2[] { new Vector2(0, 0), new Vector2(50, 100), new Vector2(100, 0) });
            lr.SetAllDirty();
        }
        var animator = GetComponent<Animator>();
        tickCount = 0;
        elapsed = 0f;
        if (state != "MissingUILineRenderer")
        {
            state = animator != null ? "InitializedWithAnimator" : "InitializedNoAnimator";
        }
    }

    private void OnAnimatorMove()
    {
        var animator = GetComponent<Animator>();
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        state = "AnimatorIK";
    }

    // Update is called once per frame
    void Update()
    {
        tickCount++;
        elapsed += Time.deltaTime;

        if (tickCount == 30)
        {
            state = "WarmupComplete";
        }

        if (elapsed > 5f)
        {
            state = "RunningStable";
        }
    }
}
