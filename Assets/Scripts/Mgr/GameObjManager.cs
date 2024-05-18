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
        QHXRPG.Event.RegisterOut("CharacterEnter", this, "CharacterEnter");
        QHXRPG.Event.RegisterOut("CharacterLeave", this, "CharacterLeave");
        QHXRPG.Event.RegisterOut("EntitySync", this, "EntitySync");
    }

    private void OnDestroy()
    {
        QHXRPG.Event.UnregisterOut("CharacterEnter", this, "CharacterEnter");
        QHXRPG.Event.UnregisterOut("CharacterLeave", this, "CharacterLeave");
        QHXRPG.Event.UnregisterOut("EntitySync", this, "EntitySync");
    }

    public void Init()
    {
        

    }

    // ������ɫ
    public void CharacterEnter(NCharacter nCharacter)
    {
        // ������Ҳ�֣�Ҳ���������
        if(! dict.ContainsKey(nCharacter.Entity.Id))  // �� ʵ��-��ɫ �ֵ���û�е�ǰ��ɫ��ʵ��
        {
            bool isMine = (nCharacter.Id == GameApp.CharacterId);
            Vector3 initPos = V3.Of(nCharacter.Entity.Position) / 1000f;  // ������

            // ������������
            if(initPos.y == 0)
            {
                initPos = GameTools.CalculateGroundPosition(initPos);
                Debug.Log("initPos :" + initPos);
            }

            //����Ԥ����
            UnitDefine unitDefine = DataManager.Instance.Units[nCharacter.Tid];
            var prefab = Resources.Load<GameObject>(unitDefine.Resource);

            // �����д����Ľ�ɫ������ NetStart �ն����£������Ͳ��ᱻ����
            var gameObject = Instantiate(prefab, initPos, Quaternion.identity, this.transform);

            gameObject.layer = 6; // ����ɫ���뵽6��ͼ����
            
            var gameEntity = gameObject.GetComponent<GameEntity>();

            Debug.Log(nCharacter.Entity.Id);
            gameEntity.isMine = isMine; // �������������˵Ľ�ɫ�����Լ��Ľ�ɫ
            gameEntity.entityName = nCharacter.Name;
            // ������˵���������Ϊ�ͻ��˵�����
            gameEntity?.SetData(nCharacter.Entity);


            // ���Լ��Ľ�ɫ����Ӣ�ۿ�����
            if (isMine)
            {
                gameObject.AddComponent<HeroController>();
            }

            if(nCharacter.EntityType == EntityType.Character)
            {
                gameObject.name = "Player-" + nCharacter.Entity.Id;
            }

            if (nCharacter.EntityType == EntityType.Monster)
            {
                gameObject.name = "Monster-" + nCharacter.Entity.Id;
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
        if (gameObject == null) return;
        var gameEntity = gameObject.GetComponent<GameEntity>();
        gameEntity.SetData(entitySync.Entity);
        if(entitySync.Force)
        {
            // ����������ת��Unity��������
            Vector3 target = V3.Of(entitySync.Entity.Position) * 0.001f;
            gameEntity.Move(target);    
        }
    }
}
