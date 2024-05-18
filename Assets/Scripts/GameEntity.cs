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
    public string entityName = "QHXRPG";

    private CharacterController characterController;  // 角色控制器

    private float fallSpeed = 0f;  // 下落速度
    private float fallSpeedMax = 30f;  // 最大下落速度

    public float speed; // 移动速度

    public EntityState entityState;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();  

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


    // 向服务器发送同步请求
    IEnumerator SyncRequest()
    {
        while (true)
        {
            // 当前客户端只循环向服务端发送自己角色的信息，别人的角色信息由别人的客户端去发送
            if(isMine && transform.hasChanged) // 是自己的角色，并且发生了位置方向的变化
            {
                request.EntitySync.Entity.Id = entityId;   //向服务器实体id
                request.EntitySync.State = entityState;     //向服务器同步状态
                SetValue(this.position * 1000, request.EntitySync.Entity.Position);
                SetValue(this.direction * 1000, request.EntitySync.Entity.Direction);
                Debug.Log(request);
                NetClient.Send(request);
                transform.hasChanged = false;   
            }


            yield return new WaitForSeconds(0.1f); // 等待0.1秒
        }
    }

    private void OnGUI()
    {
        // 设置昵称距离角色的高度
        float height = 1.8f;
        var playerCamera = Camera.main;

        // 计算角色头顶的世界坐标
        var pos = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);

        // 把世界坐标转屏幕坐标
        Vector2 uiPos = playerCamera.WorldToScreenPoint(pos);
        uiPos = new Vector2(uiPos.x, Screen.height - uiPos.y);

        // 计算文字需要占用的尺寸
        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(entityName));

        // 计算文字矩形框（左上角坐标以及宽高）
        GUI.color = Color.yellow;
        var rect = new Rect(uiPos.x - (nameSize.x) / 2, uiPos.y - nameSize.y, nameSize.x, nameSize.y);
        GUI.Label(rect, entityName);
    }



    // Update is called once per frame
    void Update()
    {
        if(!isMine)  // 如果这个角色不是自己的，实时同步服务端传来的属性
        {
            // 利用插值解决客户端这边显示别人移动时卡顿的现象
            Move(Vector3.Lerp(transform.position, position, Time.deltaTime * 5f));

            //四元数
            var targetRotation = Quaternion.Euler(direction);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, 
                                                    targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            // 玩家控制的角色，实时将玩家控制的角色的属性传给服务器
            this.position = transform.position;
            this.direction = transform.rotation.eulerAngles;
        }

        // 模拟重力
        if(!characterController.isGrounded)
        {
            //计算重力增量
            fallSpeed += 9.8f * Time.deltaTime; // 计算下落速度
            if(fallSpeed >fallSpeedMax)
            {  fallSpeedMax = fallSpeed; }

            // 模拟重力下坠
            characterController.Move(new Vector3 (0, -fallSpeed* Time.deltaTime, 0));    
        }
        else
        {
            characterController.Move(new Vector3(0, -0.01f, 0));
            fallSpeed = 0f; // 下落速度归零
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
        this.speed = nEntity.Speed * 0.001f;
        if (isMine) 
        {
            this.transform.rotation = Quaternion.Euler(direction);
            Move(position);
        }
    }


    // 移动到指定位置
    public void Move(Vector3 target)
    {
        var controller = GetComponent<CharacterController>();
        Vector3 movement = target - controller.transform.position;
        controller.Move(movement);

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
