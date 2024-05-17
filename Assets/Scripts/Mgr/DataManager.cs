using Newtonsoft.Json;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // �����ֵ�
    public Dictionary<int, SpaceDefine> Spaces;

    // ��Ԫ�ֵ�
    public Dictionary<int, UnitDefine> Units;

    public void Init()
    {
        // �����л� ��ȡJson����
        Spaces = Load<SpaceDefine>("Data/SpaceDefine");
        Units = Load<UnitDefine>("Data/UnitDefine");

    }

    private Dictionary<int, T> Load<T>(string path)
    {
        string json = Resources.Load<TextAsset>(path).text;
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(json);
    }
}

