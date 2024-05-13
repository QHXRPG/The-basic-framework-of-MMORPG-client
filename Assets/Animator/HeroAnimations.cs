using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimations : MonoBehaviour
{
    public enum HState
    {
        None = 0,
        Idle = 1,
        Run = 2,
        Attack = 3,
        Die = 4,
        Gethit = 5
    }
    public HState state = HState.Idle;
    Animator animator;



    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void SetFalseAll()
    {
        animator.SetBool("idle", false);
        animator.SetBool("run", false);
        animator.SetBool("attack1", false);
        animator.SetBool("die", false);
        animator.SetBool("gethit", false);
    }

    public void PlayIdle()
    {
        if(state == HState.Attack || state == HState.Gethit)
            return;
        SetFalseAll();
        animator.SetBool("idle", true);
        state = HState.Idle;  // 播放了不同的动画，就把当前状态切换为这个动画所对应的状态
    }

    public void PlayRun()
    {
        if (state == HState.Attack)
            return;
        SetFalseAll();
        animator.SetBool("run", true);
        state= HState.Run;
    }

    public void PlayAttack1()
    {
        SetFalseAll();
        animator.SetBool("attack1", true);
        state = HState.Attack;
    }

    public void PlayDie()
    {
        SetFalseAll();
        animator.SetBool("die", true);
        state = HState.Die;
    }

    public void PlayGethit()
    {
        SetFalseAll();
        animator.SetBool("gethit", true);
        state = HState.Gethit;

    }
    public void Attack01End()
    {
        state = HState.None;
        PlayIdle();
    }
    public void GetHitEnd()
    {
        state = HState.None;
        PlayIdle();
    }
}
