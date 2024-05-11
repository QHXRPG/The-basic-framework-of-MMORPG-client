using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto;
using Summer.Network;
using Proto.Message;
using System;
using UnityEngine.UIElements;

public class NetStart : MonoBehaviour
{
    [Header("服务器信息")]
    public string host = "127.0.0.1";
    public int port = 32510;

    [Header("登录参数")]
    public InputField usernameInput;
    public InputField passwordInput;

    private GameObject hero; //当前的角色

    // Start is called before the first frame update
    void Start()
    {
        NetClient.ConnectToServer(host, port);

        MessageRouter.Instance.Subscribe<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceCharaterEnterResponse>(_SpaceCharactersEnterResponse);
        
        
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
                //加载预制体
                var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");
                var hero = Instantiate(prefab);
                hero.transform.position = new Vector3(e.Position.X, e.Position.Y, e.Position.Z);
                Debug.Log(e.Direction.X + " " + e.Direction.Y + " " + e.Direction.Z);
                hero.transform.rotation = Quaternion.Euler(e.Direction.X, e.Direction.Y, e.Direction.Z);
            });
        }
    }

    // 当有角色进入地图时候的通知（这里的 Entity 不是新客户端连接的） 触发多次
    private void _SpaceCharactersEnterResponse(Connection conn, SpaceCharaterEnterResponse msg)
    {
        Debug.Log("角色加入：地图=" + msg.SpaceId + ",entityId=" + msg.EntityList[0].Id);
        var e = msg.EntityList[0];
        
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //加载预制体
            var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");
            var hero = Instantiate(prefab);
            hero.transform.position = new Vector3(e.Position.X, e.Position.Y, e.Position.Z);
            hero.transform.rotation = Quaternion.Euler(e.Direction.X, e.Direction.Y, e.Direction.Z);    
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
        if(hero != null)
        {
            return;
        }
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = 0;
        NetClient.Send(request);
    }
}
