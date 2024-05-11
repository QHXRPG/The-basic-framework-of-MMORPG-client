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


        MessageRouter.Instance.Subscribe<SpaceCharaterEnterResponse>(_SpaceCharactersEnterResponse);
        
        
    }

    //当有角色进入地图时候的通知（肯定不是自己）
    private void _SpaceCharactersEnterResponse(Connection conn, SpaceCharaterEnterResponse msg)
    {
        //msg.SpaceId;
        //msg.EntityList
        Debug.Log("角色加入：地图=" + msg.SpaceId + ",entityId=" + msg.EntityList[0].Id);
        var e = msg.EntityList[0];
        
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
        Debug.Log("fasong");
        NetClient.Send(request);
    }
}
