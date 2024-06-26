﻿using GameClient.Mgr;
using Proto.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Entities
{
    public class Actor : Entity
    {
        public NetActor Info;
        public UnitDefine UnitDefine;
        public SkillManager SkillMgr;
        public Actor(NetActor Info) : base(Info.Entity)
        {
            this.Info = Info;
            this.UnitDefine = DataManager.Instance.Units[Info.Tid];
            this.SkillMgr = new SkillManager(this);
        }
    }
}
