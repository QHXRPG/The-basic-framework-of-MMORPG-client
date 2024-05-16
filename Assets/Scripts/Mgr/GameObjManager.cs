using Proto.Message;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;



/*
        负责向界面当中 添加/移除游戏对象
 */
public class GameObjManager : MonoBehaviour
{

    public static GameObjManager Instance;

    private void Start()
    {
        Instance = this;

        // 使用  Out 级别（主线程）
        QHXRPG.Event.RegisterOut("CharacterEnter", this, "CreateCharaterObject");
    }

    public void Init()
    {
        

    }

    public void CreateCharaterObject(NCharacter nCharacter)
    {
        //加载预制体
        var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");
        var hero = Instantiate(prefab);
        hero.layer = 6; // 将角色加入到6号图层中
        hero.name = "Player-" + nCharacter.Entity.Id;
        Debug.Log(nCharacter.Entity.Id);
        hero.GetComponent<GameEntity>().isMine = false; // 标明这是其他人的角色

        // 把网络端的数据设置为客户端的数据
        hero.GetComponent<GameEntity>()?.SetData(nCharacter.Entity, true);
        DontDestroyOnLoad(hero);
    }
}
