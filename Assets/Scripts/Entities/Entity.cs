using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.u3d_scripts;
using Proto.Message;
using UnityEngine;

namespace GameClient
{
    // 客户端实体， 与服务器保持一致
    public class Entity
    {
        public EntityState State; // 状态
        private int _speed;  // 移动速度
        private Vector3 _position;  // 位置
        private Vector3 _direction; //方向
        private int spaceId; // 所在地图ID 
        private NetEntity _netObj;  // 网络对象
        private long _lastUpdate; // 最后一次更新时间戳

        public int SpaceId
        {
            get { return spaceId; }
            set { spaceId = value; }
        }

        public int entityId { get { return _netObj.Id; } }

        // 设置 Entity 时， NetEntity的数值也会跟着更新
        public Vector3 Position { 
            get { return _position; }
            set 
            { 
                _position = value;
                _netObj.Position = V3.ToVec3(value);
                _lastUpdate = DateTimeOffset.Now.ToUnixTimeMilliseconds();  // 更新时间戳
            }
        }

        public Vector3 Direction { 
            get { return _direction;}
            set 
            { 
                _direction = value;
                _netObj.Direction = V3.ToVec3(value);
            }
        }

        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                _netObj.Speed = value;
            }
        }


        // 距离上一次位置更新的时间间隔()
        public float PositionTime
        {
            get
            {
                return (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastUpdate) * 0.001f;
            }
        }


        public Entity(NetEntity entity)
        {
            _netObj = new NetEntity();
            _netObj.Id = entity.Id; 
            this.EntityData = entity;
        }

        // 把网络数值覆盖到本地去
        public NetEntity EntityData
        {
            get { return _netObj; }
            set
            {
                Position = V3.ToVector3(value.Position);
                Direction = V3.ToVector3(value.Direction);
                Speed = value.Speed;
            }
        }
    }
}
