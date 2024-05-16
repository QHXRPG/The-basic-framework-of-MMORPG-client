using Assets.Scripts.u3d_scripts;
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

    // <EntityId, GameObject>
    private static Dictionary<int, GameObject> dict = new Dictionary<int, GameObject>();    

    private void Start()
    {
        Instance = this;

        // ʹ��  Out �������̣߳�
        QHXRPG.Event.RegisterOut("CharacterEnter", this, "CreateCharaterObject");
        QHXRPG.Event.RegisterOut("CharacterLeave", this, "CharacterLeave");
        QHXRPG.Event.RegisterOut("EntitySync", this, "EntitySync");
    }

    private void OnDestroy()
    {
        QHXRPG.Event.UnregisterOut("CharacterEnter", this, "CreateCharaterObject");
        QHXRPG.Event.UnregisterOut("CharacterLeave", this, "CharacterLeave");
        QHXRPG.Event.UnregisterOut("EntitySync", this, "EntitySync");
    }

    public void Init()
    {
        

    }

    // ������ɫ
    public void CreateCharaterObject(NCharacter nCharacter)
    {
        if(! dict.ContainsKey(nCharacter.Entity.Id))  // �� ʵ��-��ɫ �ֵ���û�е�ǰ��ɫ��ʵ��
        {
            bool isMine = (nCharacter.Id == GameApp.CharacterId);

            //����Ԥ����
            var prefab = Resources.Load<GameObject>("Prefabs/DogPBR");

            // �����д����Ľ�ɫ������ NetStart �ն����£������Ͳ��ᱻ����
            var gameObject = Instantiate(prefab, this.transform);

            gameObject.layer = 6; // ����ɫ���뵽6��ͼ����
            gameObject.name = "Player-" + nCharacter.Entity.Id;
            Debug.Log(nCharacter.Entity.Id);
            gameObject.GetComponent<GameEntity>().isMine = isMine; // �������������˵Ľ�ɫ�����Լ��Ľ�ɫ

            // ������˵���������Ϊ�ͻ��˵�����
            gameObject.GetComponent<GameEntity>()?.SetData(nCharacter.Entity, true);

            // ���Լ��Ľ�ɫ����Ӣ�ۿ�����
            if (isMine)
            {
                gameObject.AddComponent<HeroController>();
            }
            dict.Add(nCharacter.Entity.Id, gameObject);
        }

    }
    
    // ����ɫ�뿪��ͼʱ��ɾ���ý�ɫ��Ӧ��ʵ��
    public void CharacterLeave(int entityId)
    {
        if (dict.ContainsKey(entityId))
        {
            //��ȡ��Ϸ����ɾ��
            var gameObject = dict[entityId]; 
            Destroy(gameObject);

            dict.Remove(entityId);
        }
    }

    // ʵ��λ��ͬ��
    public void EntitySync(NEntitySync entitySync)
    {
        int entityid = entitySync.Entity.Id;
        var gameObject = dict.GetValueOrDefault(entityid, null);
        gameObject?.GetComponent<GameEntity>().SetData(entitySync.Entity);
    }
}
