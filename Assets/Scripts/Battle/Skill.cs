
using GameClient.Entities;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameClient.Battle
{

    public class Skill
    {
        public enum SkillState
        {
            None,
            Casting,
            Active
        }
        public SkillDefine Define;  // 技能设定
        public Actor Owner;         // 技能归属者
        public float Cooldown;      // 冷却时间
        public float _time;       // 技能运行时间
        public SkillState State;   // 当前技能状态

        private Sprite _icon;

        public bool IsUnitTarget { get => Define.TargetType == "单位"; }

        public bool IsPointTarget { get => Define.TargetType == "点"; }

        public bool IsNullTarget { get => Define.TargetType == "None"; }

        public Sprite Icon
        {
            get
            {
                if(_icon == null)  // 为空就加载，不为空不加载
                {
                    _icon = Resources.Load<Sprite>(Define.Icon); 
                }
                return _icon;
            }
        }

        public Skill(Actor owner, int skid)
        {
            owner = owner;
            Define = DataManager.Instance.Skills[skid];
        }

        public void OnUpdate(float delta)
        {
            // delta : 上一帧到当前帧所消耗的时间
            if (Cooldown > 0) 
            {
                Cooldown -= delta;
            }
            else if(Cooldown < 0)
            {
                Cooldown = 0;
            }
            _time += delta; //在每一帧更新中增加时间的累计，用于记录技能的持续时间

            // 开始-前摇-激活-结束
            if (State == SkillState.Casting && _time >= Define.CastTime)
            {
                State = SkillState.Active;
            }
            if(State == SkillState.Active)
            {
                Log.Information("Skill Active {0}", Define.Name);
            }
        }

        public void Use(SCObject target)
        {
            Cooldown = Define.CD;
        }
    }
}
