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
using UnityEditor.PackageManager.Requests;

public class NetStart : MonoBehaviour
{
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
        NetClient.ConnectToServer(host, port);

        MessageRouter.Instance.Subscribe<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceCharaterEnterResponse>(_SpaceCharactersEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceEntitySyncResponse>(_SpaceEntitySyncResponse);
        MessageRouter.Instance.Subscribe<HeartBeatResponse>(_HeartBeatResponse);

        // 心跳包任务，每秒一次
        StartCoroutine(SendHeartMessage());
    }

    // 来自服务器的心跳响应
    private void _HeartBeatResponse(Connection sender, HeartBeatResponse msg)
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


    // 收到角色的同步信息, 别人移动了，然后通过服务器把这个信息传给我们
    private void _SpaceEntitySyncResponse(Connection sender, SpaceEntitySyncResponse msg)
    {
        int entityId = msg.EntitySync.Entity.Id;               // 拿到对方 Entity 的 id
        GameObject co = GameObject.Find("Player-" + entityId); // 通过这个 id 找到对方的预制体
        if(co != null)
        {
            // 拿到对方预制体的 GameEntity，通过 GameEntity.SetData 更新 当前客户端他的信息
            co.GetComponent<GameEntity>().SetData(msg.EntitySync.Entity);
        }
    }

    // 加入游戏的响应结果(这里的 Entity 是新客户端连接的) 触发一次
    private void _GameEnterResponse(Connection conn, GameEnterResponse msg)
    {
        Debug.Log("加入游戏的响应结果："+ msg.Success);
        if(msg.Success)
        {
            Debug.Log("角色信息：" + msg.Entity);
            var e = msg.Entity;

            //为 自己的角色创建实例
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // 找到加入游戏的按钮并隐藏(一个玩家只能创建一个角色在同一个客户端)
                GameObject.Find("Button (EnterGame)")?.SetActive(false);

                //加载预制体
                var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");
                var hero = Instantiate(prefab);
                hero.name = "Player-You";
                hero.GetComponent<GameEntity>().isMine = true; // 标明这是自己的角色

                // 把网络端的数据设置为客户端的数据
                GameEntity gameEntity = hero.GetComponent<GameEntity>();
                if (gameEntity != null)
                {
                    gameEntity.SetData(e, hero.GetComponent<GameEntity>().isMine); 
                }
                hero.AddComponent<HeroController>();  // 给自己的角色加上英雄控制器
            });
        }
    }

    // 当有角色进入地图时候的通知（这里的 Entity 不是新客户端连接的） 触发多次
    private void _SpaceCharactersEnterResponse(Connection conn, SpaceCharaterEnterResponse msg)
    {
        Debug.Log("角色加入：地图=" + msg.SpaceId + ",entityId=" + msg.EntityList[0].Id);
        var e = msg.EntityList[0];  // 取出一个 Entity

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //加载预制体
            var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");
            var hero = Instantiate(prefab);
            hero.name = "Player-" + e.Id;
            hero.GetComponent<GameEntity>().isMine = false; // 标明这是其他人的角色

            // 把网络端的数据设置为客户端的数据
            GameEntity gameEntity = hero.GetComponent<GameEntity>();
            if (gameEntity != null)
            {
                gameEntity.SetData(e, hero.GetComponent<GameEntity>().isMine);
            }
        });
    }
      


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
        
    }

    /// <summary>
    /// 加入游戏
    /// </summary>
    public void EnterGame()
    {
        if(hero != null) // hero已经创建，不会发送请求
        {
            return;
        }
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = 0;
        NetClient.Send(request);
    }

    private void OnApplicationQuit()
    {
        NetClient.Close();
    }

}
