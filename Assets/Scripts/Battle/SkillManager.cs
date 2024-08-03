using GameClient.Battle;
using GameClient.Entities;
using Proto.Message;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace GameClient.Mgr
{
    // 技能管理器，每个Actor都有独立的技能管理器
    public class SkillManager
    {
        // 归属对象
        private Actor owner;

        // 技能列表
        public List<Skill> Skills = new List<Skill>();  

        public SkillManager(Actor owner) 
        {
            this.owner = owner;
            this.InitSkills();  
        }


        public void InitSkills()
        {
            // 解析服务端传来的数据
            foreach (var skInfo in owner.Info.Skills)
            {
                var skill = new Skill(owner, skInfo.Id);
                Skills.Add(skill);
                Log.Information($"角色{owner.UnitDefine.Name}加载技能{skill.Define.ID}-{skill.Define.Name}");
            }
        }

        public void OnUpdate(float delta) 
        {
            foreach (var skill in Skills) 
            {
                skill.OnUpdate(delta);
            }
        }

        // 根据服务端传过来的技能id找到技能并返回
        public Skill GetSkill(int skillId)
        {
            return Skills.FirstOrDefault(skill => skill.Define.ID == skillId);
        }
    }
}
