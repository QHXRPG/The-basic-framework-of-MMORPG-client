using Newtonsoft.Json;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // �����ֵ�
    public Dictionary<int, SpaceDefine> Spaces;
    public void Init()
    {
        string json = Resources.Load<TextAsset>("Data/SpaceDefine").text;

        // �����л�
        Spaces = JsonConvert.DeserializeObject<Dictionary<int, SpaceDefine>>(json);
    }
}

public class SpaceDefine
{
    public int SID; // �������
    public string Name; // ����
    public string Resource; // ��Դ
    public string Kind; // ����
    public int AllowPK; // ����PK��1����0������
}
