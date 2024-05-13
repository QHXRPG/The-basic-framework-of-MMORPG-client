using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using Proto.Message;
using static UnityEngine.EventSystems.EventTrigger;


// 维护 实体信息（角色、怪物等） 挂载在预制体上
public class GameEntity : MonoBehaviour
{

    public int entityId;
    public Vector3 position;
    public Vector3 direction;
    public bool isMine;   // 是否是自己控制的角色

    // Start is called before the first frame update
    void Start()
    {
        // 开启协程， 每秒10次, 向服务器上传hero的属性（位置、方向等）
        StartCoroutine(SyncRequest());
    }

    // 设置为全局的，避免在每次调用时候都需要new出来，减少堆区压力
    SpaceEntitySyncRequest request = new SpaceEntitySyncRequest()
    {
        EntitySync = new NEntitySync()
        {
            Entity = new NEntity()
            { 
                Position = new NVector3(), 
                Direction = new NVector3()
            }
        }
    };
    internal float speed = 2;

    // 向服务器发送同步请求
    IEnumerator SyncRequest()
    {
        while (true)
        {
            request.EntitySync.Entity.Id = entityId;
            SetValue(this.position * 1000, request.EntitySync.Entity.Position);
            SetValue(this.direction * 1000, request.EntitySync.Entity.Direction);

            Debug.Log(request);
            NetClient.Send(request);

            yield return new WaitForSeconds(0.2f); // 等待0.1秒
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMine)  // 如果这个角色不是自己的，实时同步服务端传来的属性
        {
            // 利用插值解决客户端这边显示别人移动时卡顿的现象
            this.transform.rotation = Quaternion.Euler(direction);
            this.transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5f);
        }
        else
        {
            // 玩家控制的角色，实时将玩家控制的角色的属性传给服务器
            this.position = transform.position;
            this.direction = transform.rotation.eulerAngles;
        }
    }

    // 把网络端的数据设置为客户端的数据
    public void SetData(NEntity nEntity, bool isMine = false)
    {
        this.entityId = nEntity.Id;
        var p = nEntity.Position;
        var d = nEntity.Direction;
        this.position = ToVector3(nEntity.Position);
        this.direction = ToVector3(nEntity.Direction);
        if (isMine) 
        {
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(direction);
        }
    }


    // 将Unity的三维向量*1000  转为int网络类，再发给服务端
    private NVector3 ToNVector3(Vector3 v)
    {
        v *= 1000;
        return new NVector3() { X = (int)v.x, Y = (int)v.y, Z = (int)v.z };
    }

    // 将int网络类*0.001，转为Unity的三维向量
    private Vector3 ToVector3(NVector3 v)
    {
        return new Vector3() { x = v.X, y = v.Y, z = v.Z } * 0.001f;
    }


    // 将Unity的三维向量*1000  转为int网络类，再发给服务端(不需要new对象了，减少了堆区开销)
    private void SetValue(Vector3 a, NVector3 b)
    {
        b.X = (int)a.x;
        b.Y = (int)a.y;
        b.Z = (int)a.z;
    }
}
