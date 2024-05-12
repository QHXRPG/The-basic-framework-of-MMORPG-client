using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using Proto.Message;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMine)  // 如果这个角色不是自己的，实时同步服务端传来的属性
        {
            this.transform.position = new Vector3(position.x, position.y, position.z);
            this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
        }
        else
        {
            // 玩家控制的角色，实时将玩家控制的角色的属性传给服务器
            this.position = transform.position;
            var q = transform.rotation;
            this.direction = new Vector3(q.x, q.y, q.z);

            // 发送同步消息给服务器
        }
    }

    // 把网络端的数据设置为客户端的数据
    public void SetData(NEntity nEntity, bool isMine = false)
    {
        this.entityId = nEntity.Id;
        var p = nEntity.Position;
        var d = nEntity.Direction;
        this.position = new Vector3(p.X, p.Y , p.Z );
        this.direction = new Vector3(d.X, d.Y , d.Z );
        position *= 0.001f;
        direction *= 0.001f;
        if (isMine) 
        {
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
        }
    }
}
