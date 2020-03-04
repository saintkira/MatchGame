using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private Action action;

    private bool isPlay = false;
    public Timer(Action action, float timer)
    {
        this.action = action;
        this.timer = timer;
    }

    private float timer;

    public float Time
    {
        get
        {
            return timer;
        }
    }


    public void StartTimer(MonoBehaviour mono)
    {
        mono.StartCoroutine(UpdateTimer());
    }
    private IEnumerator UpdateTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            if (timer <= 0)
                action();

        }


    }
}