using Proto.Message;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;



/*
        ��������浱�� ���/�Ƴ���Ϸ����
 */
public class GameObjManager : MonoBehaviour
{

    public static GameObjManager Instance;

    private void Start()
    {
        Instance = this;

        // ʹ��  Out �������̣߳�
        QHXRPG.Event.RegisterOut("CharacterEnter", this, "CreateCharaterObject");
    }

    public void Init()
    {
        

    }

    public void CreateCharaterObject(NCharacter nCharacter)
    {
        //����Ԥ����
        var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");
        var hero = Instantiate(prefab);
        hero.layer = 6; // ����ɫ���뵽6��ͼ����
        hero.name = "Player-" + nCharacter.Entity.Id;
        Debug.Log(nCharacter.Entity.Id);
        hero.GetComponent<GameEntity>().isMine = false; // �������������˵Ľ�ɫ

        // ������˵���������Ϊ�ͻ��˵�����
        hero.GetComponent<GameEntity>()?.SetData(nCharacter.Entity, true);
        DontDestroyOnLoad(hero);
    }
}
