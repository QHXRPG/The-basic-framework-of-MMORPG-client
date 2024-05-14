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

    private int SelectedRoleIndex = -1;  // ѡ��Ľ�ɫ

    private int SelectCreateJobId = 1;  // �봴����ְҵ

    string[] Jobs = new string[] { "", "��Ѫϵ", "а��ϵ", "����ϵ", "����ϵ" };

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

        // �������������
        foreach (var p in PanelList)
        {
            p?.SetActive(false);
        }
        // ��������
        CharacterListRequest CLrequest = new CharacterListRequest();
        NetClient.Send(CLrequest);

    }

    // ɾ����ɫ����Ӧ
    private void _CharacterDeleteResponse(Connection conn, CharacterDeleteResponse msg)
    {
        // ����ˢ���б�
        CharacterListRequest characterListRequest = new CharacterListRequest();
        NetClient.Send(characterListRequest);
    }

    // ��ɫ�б����Ӧ
    private void _CharacterListResponse(Connection conn, CharacterListResponse msg)
    {
        roles.Clear();  
        // ������ɫ�б�
        foreach (var p in msg.CharacterList)
        {
            roles.Add(new RoleInfo() { Name = p.Name, Job = p.TypeId, Level = p.Level, RoleId=p.Id});
        }
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // �������������
            foreach (var p in PanelList)
            {
                p?.SetActive(false);
            }

            //�ٸ��ݽ�ɫ�б������ʾ
            if(PanelList.Count != 0)
                for (int i = 0; i < roles.Count; i++)
                {
                    if (PanelList[i])
                        PanelList[i].SetActive(true);
                        PanelList[i].transform.Find("Text (����)").GetComponent<Text>().text = roles[i].Name;
                        PanelList[i].transform.Find("Text (ְҵ)").GetComponent<Text>().text = Jobs[roles[i].Job];
                        PanelList[i].transform.Find("Text (�ȼ�)").GetComponent<Text>().text = $"Lv.{roles[i].Level}";
                }
        });
    }

    // ��ɫ������Ӧ
    private void _CharacterCreateResponse(Connection sender, CharacterCreateResponse msg)
    {
        // ����
        MyDialog.ShowMessage("ϵͳ��Ϣ", msg.Message);

        if (msg.Success == false)
        {
            // �Զ�����
            ToSelect();
        }
    }

    // ��ȡ��ɫ�б�
    public void LoadRoles()
    {
        // �������������
        foreach (var p in PanelList)
        {
            p.SetActive(false);
        }

        //�ٸ��ݽ�ɫ�б������ʾ
        for (int i = 0;i<roles.Count;i++)
        {
            PanelList[i].SetActive(true);
            PanelList[i].transform.Find("Text (����)").GetComponent<Text>().text = roles[i].Name;
            PanelList[i].transform.Find("Text (ְҵ)").GetComponent<Text>().text = Jobs[roles[i].Job];
            PanelList[i].transform.Find("Text (�ȼ�)").GetComponent<Text>().text = $"Lv.{roles[i].Level}";
        }
    }

    // ɾ����ɫ
    public void DelectRole()
    {
        if (SelectedRoleIndex < 0) return;

        var ok = new Chibi.Free.Dialog.ActionButton("ȷ��", () =>
        {
            var role = roles[SelectedRoleIndex];
            Debug.Log("ɾ����ɫ��" + role.RoleId);
            CharacterDeleteRequest characterDeleteRequest = new CharacterDeleteRequest();
            characterDeleteRequest.CharacterId = role.RoleId;
            NetClient.Send(characterDeleteRequest);
        }, new Color(0f, 0.9f, 0.9f));

        var cannel = new Chibi.Free.Dialog.ActionButton("ȡ��", () =>
        {}, new Color(0f, 0.9f, 0.9f));

        Chibi.Free.Dialog.ActionButton[] buttons = { ok, cannel };
        MyDialog.Show("��ʾ��", "�Ƿ�ɾ���ý�ɫ��", buttons);
    }

    // ��ʼ��Ϸ
    public void EnterGame()
    {
        if (SelectedRoleIndex < 0) return;
        var role = roles[SelectedRoleIndex];
        Debug.Log("��ʼ��Ϸ��" + role.Name);
        GameObject.Find("NetStart").GetComponent<NetStart>().EnterGame(role.RoleId);
    }

    // ���ѡ���˽�ɫ
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


    // ѡ�񴴽��Ľ�ɫ
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

    // ������ɫ
    public void CreateRole()
    {
        var roleName = GameObject.Find("InputRoleName").GetComponent<InputField>().text;
        Debug.Log($"Job={Jobs[SelectCreateJobId]}, Name={roleName}, Lv=1");

        // ���ʹ�����ɫ����
        CharacterCreateRequest characterCreateRequest = new CharacterCreateRequest();
        characterCreateRequest.Name = roleName;
        characterCreateRequest.JobType = SelectCreateJobId;
        NetClient.Send(characterCreateRequest);
    }

    public void ToSelect()
    {

        RoleSelectPanel.SetActive(true);
        RoleCreatePanel.SetActive(false);

        // ��������
        CharacterListRequest CLrequest = new CharacterListRequest();
        NetClient.Send(CLrequest);
    }



    // ��ɫ��Ϣ
    class RoleInfo
    {
        public GameObject TargetPanel;
        public string Name;
        public int Job;   // ְҵ����
        public int Level;
        public int RoleId;  // ��ɫId  ������Ψһ
    }

}
