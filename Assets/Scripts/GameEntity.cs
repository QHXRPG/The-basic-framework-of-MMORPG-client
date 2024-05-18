using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using Proto.Message;


// ά�� ʵ����Ϣ����ɫ������ȣ� ������Ԥ������
public class GameEntity : MonoBehaviour
{

    public int entityId;
    public Vector3 position;
    public Vector3 direction;
    public bool isMine;   // �Ƿ����Լ����ƵĽ�ɫ
    public string entityName = "QHXRPG";

    private CharacterController characterController;  // ��ɫ������

    private float fallSpeed = 0f;  // �����ٶ�
    private float fallSpeedMax = 30f;  // ��������ٶ�

    public float speed; // �ƶ��ٶ�

    public EntityState entityState;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();  

        // ����Э�̣� ÿ��10��, ��������ϴ�hero�����ԣ�λ�á�����ȣ�
        StartCoroutine(SyncRequest());
    }

    // ����Ϊȫ�ֵģ�������ÿ�ε���ʱ����Ҫnew���������ٶ���ѹ��
    SpaceEntitySyncRequest request = new SpaceEntitySyncRequest()
    {
        EntitySync = new NEntitySync()
        {
            Entity = new NEntity()
            { 
                Position = new NVector3(), 
                Direction = new NVector3()
            }
        }
    };


    // �����������ͬ������
    IEnumerator SyncRequest()
    {
        while (true)
        {
            // ��ǰ�ͻ���ֻѭ�������˷����Լ���ɫ����Ϣ�����˵Ľ�ɫ��Ϣ�ɱ��˵Ŀͻ���ȥ����
            if(isMine && transform.hasChanged) // ���Լ��Ľ�ɫ�����ҷ�����λ�÷���ı仯
            {
                request.EntitySync.Entity.Id = entityId;   //�������ʵ��id
                request.EntitySync.State = entityState;     //�������ͬ��״̬
                SetValue(this.position * 1000, request.EntitySync.Entity.Position);
                SetValue(this.direction * 1000, request.EntitySync.Entity.Direction);
                Debug.Log(request);
                NetClient.Send(request);
                transform.hasChanged = false;   
            }


            yield return new WaitForSeconds(0.1f); // �ȴ�0.1��
        }
    }

    private void OnGUI()
    {
        // �����ǳƾ����ɫ�ĸ߶�
        float height = 1.8f;
        var playerCamera = Camera.main;

        // �����ɫͷ������������
        var pos = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);

        // ����������ת��Ļ����
        Vector2 uiPos = playerCamera.WorldToScreenPoint(pos);
        uiPos = new Vector2(uiPos.x, Screen.height - uiPos.y);

        // ����������Ҫռ�õĳߴ�
        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(entityName));

        // �������־��ο����Ͻ������Լ���ߣ�
        GUI.color = Color.yellow;
        var rect = new Rect(uiPos.x - (nameSize.x) / 2, uiPos.y - nameSize.y, nameSize.x, nameSize.y);
        GUI.Label(rect, entityName);
    }



    // Update is called once per frame
    void Update()
    {
        if(!isMine)  // ��������ɫ�����Լ��ģ�ʵʱͬ������˴���������
        {
            // ���ò�ֵ����ͻ��������ʾ�����ƶ�ʱ���ٵ�����
            Move(Vector3.Lerp(transform.position, position, Time.deltaTime * 5f));

            //��Ԫ��
            var targetRotation = Quaternion.Euler(direction);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, 
                                                    targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            // ��ҿ��ƵĽ�ɫ��ʵʱ����ҿ��ƵĽ�ɫ�����Դ���������
            this.position = transform.position;
            this.direction = transform.rotation.eulerAngles;
        }

        // ģ������
        if(!characterController.isGrounded)
        {
            //������������
            fallSpeed += 9.8f * Time.deltaTime; // ���������ٶ�
            if(fallSpeed >fallSpeedMax)
            {  fallSpeedMax = fallSpeed; }

            // ģ��������׹
            characterController.Move(new Vector3 (0, -fallSpeed* Time.deltaTime, 0));    
        }
        else
        {
            characterController.Move(new Vector3(0, -0.01f, 0));
            fallSpeed = 0f; // �����ٶȹ���
        }
    }

    // ������˵���������Ϊ�ͻ��˵�����
    public void SetData(NEntity nEntity, bool isMine = false)
    {
        this.entityId = nEntity.Id;
        var p = nEntity.Position;
        var d = nEntity.Direction;
        this.position = ToVector3(nEntity.Position);
        this.direction = ToVector3(nEntity.Direction);
        this.speed = nEntity.Speed * 0.001f;
        if (isMine) 
        {
            this.transform.rotation = Quaternion.Euler(direction);
            Move(position);
        }
    }


    // �ƶ���ָ��λ��
    public void Move(Vector3 target)
    {
        var controller = GetComponent<CharacterController>();
        Vector3 movement = target - controller.transform.position;
        controller.Move(movement);

    }


    // ��Unity����ά����*1000  תΪint�����࣬�ٷ��������
    private NVector3 ToNVector3(Vector3 v)
    {
        v *= 1000;
        return new NVector3() { X = (int)v.x, Y = (int)v.y, Z = (int)v.z };
    }

    // ��int������*0.001��תΪUnity����ά����
    private Vector3 ToVector3(NVector3 v)
    {
        return new Vector3() { x = v.X, y = v.Y, z = v.Z } * 0.001f;
    }


    // ��Unity����ά����*1000  תΪint�����࣬�ٷ��������(����Ҫnew�����ˣ������˶�������)
    private void SetValue(Vector3 a, NVector3 b)
    {
        b.X = (int)a.x;
        b.Y = (int)a.y;
        b.Z = (int)a.z;
    }
}
