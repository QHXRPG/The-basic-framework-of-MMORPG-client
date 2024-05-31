using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto;
using Summer.Network;
using Proto.Message;
using System;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Assets.Scripts.u3d_scripts;
using GameClient.Mgr;
using GameClient.Entities;


public class NetStart : MonoBehaviour
{
    public List<GameObject> keepAlive;

    [Header("服务器信息")]
    public string host = "127.0.0.1";
    public int port = 32510;

    [Header("登录参数")]
    public InputField usernameInput;
    public InputField passwordInput;
    public Text PingText;

    private GameObject hero; //当前的角色

    // Start is called before the first frame update
    void Start()
    {
        // 忽略6号图层之间的碰撞
        Physics.IgnoreLayerCollision(6, 6, true);
        NetClient.ConnectToServer(host, port);

        // 设置不被销毁的对象
        foreach (GameObject go in keepAlive) 
        {
            DontDestroyOnLoad(go);
        }

        MessageRouter.Instance.Subscribe<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceCharaterEnterResponse>(_SpaceCharactersEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<HeartBeatResponse>(_HeartBeatResponse);
        MessageRouter.Instance.Subscribe<SpaceCharaterLeaveResponse>(_SpaceCharaterLeaveResponse);

        // 心跳包任务，每秒一次
        StartCoroutine(SendHeartMessage());

        SceneManager.LoadScene("LoginScene");

        DataManager.Instance.Init();

        // 注册当前类中 加入游戏 的事件， 函数为 EnterGame
        QHXRPG.Event.RegisterIn("EnterGame", this, "EnterGame");
    }

    //有角色离开地图
    private void _SpaceCharaterLeaveResponse(Connection conn, SpaceCharaterLeaveResponse msg)
    {
        EntityManager.Instance.RemoveEntity(msg.EntityId);
    }

    // 来自服务器的心跳响应
    private void _HeartBeatResponse(Connection conn, HeartBeatResponse msg)
    {
        var t = DateTime.Now - lastBeatTime; // 计算延迟

        //主线程是唯一能够访问和修改Unity场景、游戏对象以及调用Unity API的线程。
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            int ms = (int)Math.Round(t.TotalMilliseconds);
            if (ms <= 0) ms = 1;
            PingText.text = $"Ping:{ms}ms";
        });

    }


    private HeartBeatRequest beatRequest = new HeartBeatRequest();
    DateTime lastBeatTime = DateTime.MinValue;  
    // 向服务器发送心跳
    IEnumerator SendHeartMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f); // 等待1秒
            NetClient.Send(beatRequest);
            lastBeatTime = DateTime.Now; // 记录发送时间
        }
    }


    // 角色的同步信息, 别人移动了，通过服务器把这个信息传给我们
    private void _SpaceEntitySyncResponse(Connection sender, SpaceEntitySyncResponse msg)
    {
        // Debug.Log(msg);
        EntityManager.Instance.ONetEntitySync(msg.EntitySync);
    }


    // 加入游戏的响应结果(这里的 Entity 是新客户端连接的) 本人
    private void _GameEnterResponse(Connection conn, GameEnterResponse msg)
    {
        Debug.Log("加入游戏的响应结果："+ msg.Success);
        if(msg.Success)
        {
            Debug.Log("角色信息：" + msg);
            var info = msg.Charater;
            info.Entity = msg.Entity;

            GameApp.LoadSpace(msg.Charater.SpaceId);  // 进入场景
            EntityManager.Instance.ONetEntityEnter(info);

            // 把当前游戏的对象取出来
            GameApp.Character = EntityManager.Instance.GetEntity<Character>(msg.Entity.Id);
        }
    }


    // 当有角色进入地图时候的通知（不是当前客户端） 触发多次
    private void _SpaceCharactersEnterResponse(Connection conn, SpaceCharaterEnterResponse msg)
    {
        Debug.Log("角色加入：地图=" + msg.SpaceId + ",entityId=" + msg.CharacterList[0].Id);

        foreach (var info in msg.CharacterList) 
        {
            EntityManager.Instance.ONetEntityEnter(info);
            
        }
    }
      

    // OnUpdate is called once per frame
    void Update()
    {
        QHXRPG.Event.Tick();
        if (Input.GetMouseButtonDown(0))  // 当鼠标左键被按下
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // 从鼠标点击位置发出一条射线
            RaycastHit hitInfo;  // 存储射线投射结果的数据
            LayerMask actorLayer = LayerMask.GetMask("Actor");
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, actorLayer))  // 检测射线是否与特定图层的物体相交
            {
                GameObject clickedObject = hitInfo.collider.gameObject;  // 获取被点击的物体
                                                                         // 在这里可以对获取到的物体进行处理
                Debug.Log("选择目标: " + clickedObject.name);

                // 通过所点击对象的 GameEntity 组件找到 entityId
                // 再通过 entityId 找到 entity，进而找到 Actor
                int entityId = clickedObject.GetComponent<GameEntity>().entityId;
                GameApp.Target = EntityManager.Instance.GetEntity<Actor>(entityId);
            }
        }

    }

    private void FixedUpdate()
    {
        EntityManager.Instance.OnUpdate(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        QHXRPG.Event.UnregisterIn("EnterGame", this, "EnterGame");
    }

    public void Login()
    {
        
    }

    /// <summary>
    /// 加入游戏
    /// </summary>
    public void EnterGame(int roleId)
    {
        if(hero != null) // hero已经创建，不会发送请求
        {
            return;
        }
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = roleId;
        NetClient.Send(request);
    }

    private void OnApplicationQuit()
    {
        NetClient.Close();
    }

}
