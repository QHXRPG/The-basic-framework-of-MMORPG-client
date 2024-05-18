using Assets.Scripts.u3d_scripts;
using Proto.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Summer;

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
    GameEntity gameEntity;

    void Start()
    {
        gameEntity = GetComponent<GameEntity>();
        animator = GetComponent<Animator>();
        QHXRPG.Event.RegisterOut("EntitySync", this, "EntitySync");
    }

    // 实体位置同步
    public void EntitySync(NEntitySync entitySync)
    {
        int entityid = entitySync.Entity.Id;
        if(entityid != gameEntity.entityId) { return; }
        switch(entitySync.State) 
        {
            case EntityState.Idle:
                PlayIdle(); break;
            case EntityState.Move:
                PlayRun(); break;   
        }
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
        gameEntity.entityState = Proto.Message.EntityState.Idle;
    }

    public void PlayRun()
    {
        if (state == HState.Attack)
            return;
        SetFalseAll();
        animator.SetBool("run", true);
        state= HState.Run;
        gameEntity.entityState = Proto.Message.EntityState.Move;
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

    public void Attack02End()
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
