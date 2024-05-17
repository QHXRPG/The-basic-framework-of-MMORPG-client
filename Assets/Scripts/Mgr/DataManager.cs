using Newtonsoft.Json;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // 场景字典
    public Dictionary<int, SpaceDefine> Spaces;

    // 单元字典
    public Dictionary<int, UnitDefine> Units;

    public void Init()
    {
        // 反序列化 读取Json数据
        Spaces = Load<SpaceDefine>("Data/SpaceDefine");
        Units = Load<UnitDefine>("Data/UnitDefine");

    }

    private Dictionary<int, T> Load<T>(string path)
    {
        string json = Resources.Load<TextAsset>(path).text;
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(json);
    }
}

