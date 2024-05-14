using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto;
using Proto.Message;
using Summer.Network;
using System;

public class RoleListController : MonoBehaviour
{
    public GameObject RoleSelectPanel;
    public GameObject RoleCreatePanel;

    private int SelectedRoleIndex = -1;  // 选择的角色

    private int SelectCreateJobId = 1;  // 想创建的职业

    string[] Jobs = new string[] { "", "嗜血系", "邪恶系", "混沌系", "幻象系" };

    public List<GameObject> PanelList = new List<GameObject>();

    List<RoleInfo> roles = new List<RoleInfo>();
    // Start is called before the first frame update
    void Start()
    {
        MessageRouter.Instance.Subscribe<CharacterCreateResponse>(_CharacterCreateResponse);
        MessageRouter.Instance.Subscribe<CharacterListResponse>(_CharacterListResponse);
        MessageRouter.Instance.Subscribe<CharacterDeleteResponse>(_CharacterDeleteResponse);

        for (int i = 1; i <= 4; i++)
        {
            PanelList.Add(GameObject.Find($"HeroPanel-{i}"));
        }

        // 先隐藏所有面板
        foreach (var p in PanelList)
        {
            p?.SetActive(false);
        }
        // 加载数据
        CharacterListRequest CLrequest = new CharacterListRequest();
        NetClient.Send(CLrequest);

    }

    // 删除角色的响应
    private void _CharacterDeleteResponse(Connection conn, CharacterDeleteResponse msg)
    {
        // 请求刷新列表
        CharacterListRequest characterListRequest = new CharacterListRequest();
        NetClient.Send(characterListRequest);
    }

    // 角色列表的响应
    private void _CharacterListResponse(Connection conn, CharacterListResponse msg)
    {
        roles.Clear();  
        // 解析角色列表
        foreach (var p in msg.CharacterList)
        {
            roles.Add(new RoleInfo() { Name = p.Name, Job = p.TypeId, Level = p.Level, RoleId=p.Id});
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // 先隐藏所有面板
            foreach (var p in PanelList)
            {
                p?.SetActive(false);
            }

            //再根据角色列表逐个显示
            if(PanelList.Count != 0)
                for (int i = 0; i < roles.Count; i++)
                {
                    if (PanelList[i])
                        PanelList[i].SetActive(true);
                        PanelList[i].transform.Find("Text (名字)").GetComponent<Text>().text = roles[i].Name;
                        PanelList[i].transform.Find("Text (职业)").GetComponent<Text>().text = Jobs[roles[i].Job];
                        PanelList[i].transform.Find("Text (等级)").GetComponent<Text>().text = $"Lv.{roles[i].Level}";
                }
        });
    }

    // 角色创建响应
    private void _CharacterCreateResponse(Connection sender, CharacterCreateResponse msg)
    {
        // 弹窗
        MyDialog.ShowMessage("系统消息", msg.Message);

        if (msg.Success == false)
        {
            // 自动返回
            ToSelect();
        }
    }

    // 获取角色列表
    public void LoadRoles()
    {
        // 先隐藏所有面板
        foreach (var p in PanelList)
        {
            p.SetActive(false);
        }

        //再根据角色列表逐个显示
        for (int i = 0;i<roles.Count;i++)
        {
            PanelList[i].SetActive(true);
            PanelList[i].transform.Find("Text (名字)").GetComponent<Text>().text = roles[i].Name;
            PanelList[i].transform.Find("Text (职业)").GetComponent<Text>().text = Jobs[roles[i].Job];
            PanelList[i].transform.Find("Text (等级)").GetComponent<Text>().text = $"Lv.{roles[i].Level}";
        }
    }

    // 删除角色
    public void DelectRole()
    {
        if (SelectedRoleIndex < 0) return;

        var ok = new Chibi.Free.Dialog.ActionButton("确定", () =>
        {
            var role = roles[SelectedRoleIndex];
            Debug.Log("删除角色：" + role.RoleId);
            CharacterDeleteRequest characterDeleteRequest = new CharacterDeleteRequest();
            characterDeleteRequest.CharacterId = role.RoleId;
            NetClient.Send(characterDeleteRequest);
        }, new Color(0f, 0.9f, 0.9f));

        var cannel = new Chibi.Free.Dialog.ActionButton("取消", () =>
        {}, new Color(0f, 0.9f, 0.9f));

        Chibi.Free.Dialog.ActionButton[] buttons = { ok, cannel };
        MyDialog.Show("提示！", "是否删除该角色？", buttons);
    }

    // 开始游戏
    public void EnterGame()
    {
        if (SelectedRoleIndex < 0) return;
        var role = roles[SelectedRoleIndex];
        Debug.Log("开始游戏：" + role.Name);
        GameObject.Find("NetStart").GetComponent<NetStart>().EnterGame(role.RoleId);
    }

    // 点击选择了角色
    public void RoleClick(int num)
    {

        SelectedRoleIndex = num;
        RoleInfo info = roles[num];
        Debug.Log($"{num}, {roles[num].RoleId}");
        var p2 = GameObject.Find("Panel-2");
        p2.transform.Find("BG/Name/TextRight").GetComponent<Text>().text = info.Name;
        p2.transform.Find("BG/Job/TextRight").GetComponent<Text>().text = Jobs[info.Job];
        p2.transform.Find("BG/Lv/TextRight").GetComponent<Text>().text = $"Lv.{info.Level}";
        for(int i=0; i<PanelList.Count;i++) 
        {
            PanelList[i].transform.Find("Image").gameObject.SetActive(i==num);
        }
    }


    // 选择创建的角色
    public void SelectCreateJob(int index)
    {
        SelectCreateJobId = index;
        GameObject.Find("Image/TextJob").GetComponent<Text>().text = Jobs[SelectCreateJobId];
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void ToCreate()
    {
        RoleSelectPanel.SetActive(false);
        RoleCreatePanel.SetActive(true);
    }

    // 创建角色
    public void CreateRole()
    {
        var roleName = GameObject.Find("InputRoleName").GetComponent<InputField>().text;
        Debug.Log($"Job={Jobs[SelectCreateJobId]}, Name={roleName}, Lv=1");

        // 发送创建角色请求
        CharacterCreateRequest characterCreateRequest = new CharacterCreateRequest();
        characterCreateRequest.Name = roleName;
        characterCreateRequest.JobType = SelectCreateJobId;
        NetClient.Send(characterCreateRequest);
    }

    public void ToSelect()
    {

        RoleSelectPanel.SetActive(true);
        RoleCreatePanel.SetActive(false);

        // 加载数据
        CharacterListRequest CLrequest = new CharacterListRequest();
        NetClient.Send(CLrequest);
    }



    // 角色信息
    class RoleInfo
    {
        public GameObject TargetPanel;
        public string Name;
        public int Job;   // 职业种类
        public int Level;
        public int RoleId;  // 角色Id  主键、唯一
    }

}
