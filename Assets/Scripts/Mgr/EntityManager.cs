using GameClient.Entities;
using Proto.Message;
using Summer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.EventSystems.EventTrigger;

namespace GameClient.Mgr
{
    public class EntityManager : Singleton<EntityManager>
    {
        public EntityManager() 
        { 

        }
        
        // 线程安全字典
        private ConcurrentDictionary<int, Entity> _dict = new ConcurrentDictionary<int, Entity>(); 

        public void AddEntity(Entity entity) 
        {
            UnityEngine.Debug.Log("AddEntity" + entity.entityId);
            _dict[entity.entityId] = entity;
        }

        public void RemoveEntity(int entityId) 
        {
            UnityEngine.Debug.Log("RemoveEntity" + entityId);
            _dict.Remove(entityId, out Entity entity);
            QHXRPG.Event.FireOut("CharacterLeave", entityId);
        }

        public void ONetEntityEnter(NetActor info)
        {

            if (info.EntityType == EntityType.Character) 
            {
                AddEntity(new Character(info));
            }
            if(info.EntityType == EntityType.Monster)
            {
                AddEntity(new Monster(info));
            }
            QHXRPG.Event.FireOut("CharacterEnter", info); // 调用进入游戏的方法
        }


        // 处理位置同步
        public void ONetEntitySync(NetEntitySync NetEntitySync)
        {
            var entityId = NetEntitySync.Entity.Id;
            UnityEngine.Debug.Log("ONetEntitySync" + entityId);
            var entity = _dict.GetValueOrDefault(entityId);    
            if(entity != null) 
            {
                entity.State = NetEntitySync.State;
                entity.EntityData = NetEntitySync.Entity;

                // 涉及到游戏对象的获取和访问，必须保证该过程在UI线程（主线程）中进行
                QHXRPG.Event.FireOut("EntitySync", NetEntitySync);
            }

        }

        public T GetEntity<T> (int entityId) where T : Entity
        {
            return (T)_dict.GetValueOrDefault(entityId);
        }
        public List<T> GetEntities<T>(Predicate<T> match)
        {
            return _dict.Values.OfType<T>().Where(e => match.Invoke(e)).ToList();
        }

        // 由主线程调用
        public void OnUpdate(float delta)
        {
            foreach (var entity in _dict.Values) 
            {
                var actor = entity as Actor;
                actor.SkillMgr?.OnUpdate(delta);
            }
        }
    }
}
