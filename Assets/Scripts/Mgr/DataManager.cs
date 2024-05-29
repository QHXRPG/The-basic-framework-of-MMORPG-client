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

    // �����ֵ� 
    public Dictionary<int, SkillDefine> Skills;

    public void Init()
    {
        // �����л� ��ȡJson����
        Spaces = Load<SpaceDefine>("Data/SpaceDefine");
        Units = Load<UnitDefine>("Data/UnitDefine");
        Skills = Load<SkillDefine>("Data/SkillDefine");

    }

    private Dictionary<int, T> Load<T>(string path)
    {
        string json = Resources.Load<TextAsset>(path).text;
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(json);
    }
}

