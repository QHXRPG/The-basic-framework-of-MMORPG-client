using Assets.Scripts.u3d_scripts;
using GameClient.Entities;
using Proto.Message;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


/*
        负责向界面当中 添加/移除游戏对象
 */
public class GameObjManager : MonoBehaviour
{

    public static GameObjManager Instance;

    // <EntityId, GameObject>
    private static Dictionary<int, GameObject> dict = new Dictionary<int, GameObject>();    

    private void Start()
    {
        Instance = this;

        // 使用  Out 级别（主线程）
        QHXRPG.Event.RegisterOut("CharacterEnter", this, "CharacterEnter");
        QHXRPG.Event.RegisterOut("CharacterLeave", this, "CharacterLeave");
        QHXRPG.Event.RegisterOut("EntitySync", this, "EntitySync");
    }

    private void OnDestroy()
    {
        QHXRPG.Event.UnregisterOut("CharacterEnter", this, "CharacterEnter");
        QHXRPG.Event.UnregisterOut("CharacterLeave", this, "CharacterLeave");
        QHXRPG.Event.UnregisterOut("EntitySync", this, "EntitySync");
    }

    public void Init()
    {
        

    }

    // 创建角色
    public void CharacterEnter(NetActor NetActor)
    {
        // 可能是也怪，也可能是玩家
        if(! dict.ContainsKey(NetActor.Entity.Id))  // 当 实体-角色 字典中没有当前角色的实体
        {
            bool isMine = (NetActor.Entity.Id == GameApp.Character.entityId);
            Vector3 initPos = V3.Of(NetActor.Entity.Position) / 1000f;  // 出生点

            // 计算地面的坐标
            if(initPos.y == 0)
            {
                initPos = GameTools.CalculateGroundPosition(initPos);
                Debug.Log("Pos :" + initPos);
            }

            //加载预制体
            UnitDefine unitDefine = DataManager.Instance.Units[NetActor.Tid];
            var prefab = Resources.Load<GameObject>(unitDefine.Resource);

            // 把所有创建的角色都挂在 NetStart 空对象下，这样就不会被销毁
            var gameObject = Instantiate(prefab, initPos, Quaternion.identity, this.transform);

            gameObject.layer = 6; // 将角色加入到6号图层中
            
            var gameEntity = gameObject.GetComponent<GameEntity>();

            Debug.Log(NetActor.Entity.Id);
            gameEntity.isMine = isMine; // 标明这是其他人的角色还是自己的角色
            gameEntity.entityName = NetActor.Name;
            // 把网络端的数据设置为客户端的数据
            gameEntity?.SetData(NetActor.Entity);


            // 给自己的角色加上英雄控制器
            if (isMine)
            {
                gameObject.AddComponent<HeroController>();
            }

            if(NetActor.EntityType == EntityType.Character)
            {
                gameObject.name = "Player-" + NetActor.Entity.Id;
            }

            if (NetActor.EntityType == EntityType.Monster)
            {
                gameObject.name = "Monster-" + NetActor.Entity.Id;
            }


            dict.Add(NetActor.Entity.Id, gameObject);
        }

    }
    
    // 当角色离开地图时，删除该角色对应的实体
    public void CharacterLeave(int entityId)
    {
        if (dict.ContainsKey(entityId))
        {
            //获取游戏对象并删除
            var gameObject = dict[entityId]; 
            Destroy(gameObject);

            dict.Remove(entityId);
        }
    }

    // 实体位置同步
    public void EntitySync(NetEntitySync entitySync)
    {
        int entityid = entitySync.Entity.Id;
        var gameObject = dict.GetValueOrDefault(entityid, null);
        if (gameObject == null) return;
        Vector3 Pos = V3.Of(entitySync.Entity.Position) / 1000f;  // 出生点

        // 计算地面的坐标
        if (Pos.y == 0)
        {
            Pos = GameTools.CalculateGroundPosition(Pos, 20);
            entitySync.Entity.Position = V3.ToVec3(Pos*1000);
            Debug.Log("Pos :" + Pos);
        }
        var gameEntity = gameObject.GetComponent<GameEntity>();
        gameEntity.SetData(entitySync.Entity);
        if(entitySync.Force)
        {
            // 把网络类型转成Unity本地类型
            Vector3 target = V3.Of(entitySync.Entity.Position) * 0.001f;
            gameEntity.Move(target);    
        }
    }
}
