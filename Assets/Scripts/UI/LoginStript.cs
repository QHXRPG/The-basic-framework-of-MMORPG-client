using Proto.Message;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginStripts : MonoBehaviour
{
    public InputField UserName;
    public InputField Password;
    public UnityEngine.UI.Button buttonLogin;
    // Start is called before the first frame update
    void Start()
    {
        // 服务端发过来的登录响应
        MessageRouter.Instance.Subscribe<UserLoginReponse>(_UserLoginReponse);
    }

    // 用户登录响应
    private void _UserLoginReponse(Connection sender, UserLoginReponse msg)
    {
        Debug.Log(msg);
        var ok = new Chibi.Free.Dialog.ActionButton("确定", () =>
        {
            Debug.Log("click ok");
        }, new Color(0f, 0.9f, 0.9f));
        Chibi.Free.Dialog.ActionButton[] button = { ok };
        MyDialog.Show("系统消息，", msg.Message, button);

        if(msg.Success)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene("SelectHero");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoLogin()
    {
        Debug.Log($"User={UserName.text}, Psw={Password.text}");
        var msg = new UserLoginRequest();
        msg.Username = UserName.text;
        msg.Password = Password.text;
        NetClient.Send(msg);
    }
}
