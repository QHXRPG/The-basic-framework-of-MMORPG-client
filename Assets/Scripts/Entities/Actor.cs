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
        public NCharacter Info;
        public UnitDefine UnitDefine;
        public Actor(NCharacter Info) : base(Info.Entity)
        {
            this.Info = Info;
            this.UnitDefine = DataManager.Instance.Units[Info.Tid];

        }
    }
}
