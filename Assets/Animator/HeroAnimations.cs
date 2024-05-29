using Assets.Scripts.u3d_scripts;
using Proto.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Summer;
using Animancer;

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
    GameEntity gameEntity; 
    NamedAnimancerComponent _animancer;
    
    

    void Start()
    {
        gameEntity = GetComponent<GameEntity>();
        _animancer = GetComponent<NamedAnimancerComponent>();
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

    public AnimancerState Play(string name, Action OnEnd=null)
    {
        if (_animancer == null) return null;
        AnimancerState state = _animancer.TryPlay(name);
        if (state == null)
        {
            Debug.LogWarning($"动画名称 {name} 不存在");
        }
        else
        {
            if (OnEnd != null)  // 事件回调不为空
            {
                state.Events.OnEnd = OnEnd;
            }
        }
        return state;
    }

    // OnUpdate is called once per frame
    void Update()
    {

    }


    private void SetFalseAll()
    {

    }

    public void PlayIdle()
    {
        if(state == HState.Attack || state == HState.Gethit)
            return;
        Play("Idle");
        state = HState.Idle;
        gameEntity.entityState = Proto.Message.EntityState.Idle;
    }

    public void PlayRun()
    {
        if (state == HState.Attack)
            return;
        SetFalseAll();
        Play("Run");
        state = HState.Run;
        gameEntity.entityState = Proto.Message.EntityState.Move;
    }

    public void PlayAttack()
    {
        Play("Attack", OnEnd : OnAttackEnd);
        state = HState.Attack;
    }

    public void PlayDie()
    {
        Play("Die");
        state = HState.Die;
    }

    public void PlayGethit()
    {
        Play("Gethit", OnEnd : GetHitEnd);
        state = HState.Gethit;

    }
    public void OnAttackEnd()
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
