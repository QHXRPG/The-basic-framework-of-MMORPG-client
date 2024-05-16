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
        QHXRPG.Event.FireOut("CharacterLeave", msg.EntityId);
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
        Debug.Log(msg);

        // 涉及到游戏对象的获取和访问，必须保证该过程在UI线程（主线程）中进行
        QHXRPG.Event.FireOut("EntitySync", msg.EntitySync);
    }

    // 加入游戏的响应结果(这里的 Entity 是新客户端连接的) 触发一次 本人
    private void _GameEnterResponse(Connection conn, GameEnterResponse msg)
    {
        Debug.Log("加入游戏的响应结果："+ msg.Success);
        if(msg.Success)
        {
            Debug.Log("角色信息：" + msg);
            var chr = msg.Charater;
            chr.Entity = msg.Entity;
            GameApp.CharacterId = chr.Id; // 记录全局游戏的id
            QHXRPG.Event.FireOut("CharacterEnter", chr); // 调用进入游戏的方法
            GameApp.LoadSpace(msg.Charater.SpaceId);  // 进入场景
        }
    }

    // 当有角色进入地图时候的通知（不是当前客户端） 触发多次
    private void _SpaceCharactersEnterResponse(Connection conn, SpaceCharaterEnterResponse msg)
    {
        Debug.Log("角色加入：地图=" + msg.SpaceId + ",entityId=" + msg.CharacterList[0].Id);
        var character = msg.CharacterList[0];  // 取出一个 Entity

        QHXRPG.Event.FireOut("CharacterEnter", character);
    }
      


    // Update is called once per frame
    void Update()
    {
        QHXRPG.Event.Tick();
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
