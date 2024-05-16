using Newtonsoft.Json;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // 场景字典
    public Dictionary<int, SpaceDefine> Spaces;
    public void Init()
    {
        string json = Resources.Load<TextAsset>("Data/SpaceDefine").text;

        // 反序列化
        Spaces = JsonConvert.DeserializeObject<Dictionary<int, SpaceDefine>>(json);
    }
}

public class SpaceDefine
{
    public int SID; // 场景编号
    public string Name; // 名称
    public string Resource; // 资源
    public string Kind; // 类型
    public int AllowPK; // 允许PK（1允许，0不允许）
}
