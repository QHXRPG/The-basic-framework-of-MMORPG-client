using Proto.Message;
using Summer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginStripts : MonoBehaviour
{
    public InputField UserName;
    public InputField Password;
    public InputField UserNameRegister;
    public InputField PasswordRegister;

    public GameObject CanvasLogin;
    public GameObject CanvasRegister;

    public UnityEngine.UI.Button buttonLogin;
    // Start is called before the first frame update
    void Start()
    {
        // ����˷������ĵ�¼��Ӧ
        MessageRouter.Instance.Subscribe<UserLoginReponse>(_UserLoginReponse);

        // ����˷�������ע����Ӧ
        MessageRouter.Instance.Subscribe<UserRegisterReponse>(_UserRegisterReponse);

        try
        {
            CanvasLogin.SetActive(true);
            CanvasRegister.SetActive(false);
        }
        catch { }
    }

    private void _UserRegisterReponse(Connection sender, UserRegisterReponse msg)
    {
        Debug.Log(msg);

        // ����
        MyDialog.ShowMessage("ϵͳ��Ϣ", msg.Message);


        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (msg.Code == 0)
            {
                //ע��ʧ�ܣ��ص���ǰҳ��
            }
            else
            {
                // ע��ɹ����ص�¼ҳ��
                try
                {
                    CanvasLogin.SetActive(true);
                    CanvasRegister.SetActive(false);
                }
                catch { }
            }
        });
    }

    // �û���¼��Ӧ
    private void _UserLoginReponse(Connection sender, UserLoginReponse msg)
    {
        Debug.Log(msg);

        // ����
        MyDialog.ShowMessage("ϵͳ��Ϣ", msg.Message);

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

    public void ToRegister()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            CanvasLogin.SetActive(false);
            CanvasRegister.SetActive(true);
        });
    }

    public void Register()
    {
        Debug.Log($"ע�᣺User={UserNameRegister.text}, Psw={PasswordRegister.text}");
        var msg = new UserRegisterRequest();
        msg.Username = UserNameRegister.text;
        msg.Password = PasswordRegister.text;
        NetClient.Send(msg);
    }
}
