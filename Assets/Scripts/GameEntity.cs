using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using Proto.Message;
using static UnityEngine.EventSystems.EventTrigger;


// ά�� ʵ����Ϣ����ɫ������ȣ� ������Ԥ������
public class GameEntity : MonoBehaviour
{

    public int entityId;
    public Vector3 position;
    public Vector3 direction;
    public bool isMine;   // �Ƿ����Լ����ƵĽ�ɫ

    // Start is called before the first frame update
    void Start()
    {
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
    internal float speed = 2;

    // �����������ͬ������
    IEnumerator SyncRequest()
    {
        while (true)
        {
            request.EntitySync.Entity.Id = entityId;
            SetValue(this.position * 1000, request.EntitySync.Entity.Position);
            SetValue(this.direction * 1000, request.EntitySync.Entity.Direction);

            Debug.Log(request);
            NetClient.Send(request);

            yield return new WaitForSeconds(0.2f); // �ȴ�0.1��
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMine)  // ��������ɫ�����Լ��ģ�ʵʱͬ������˴���������
        {
            // ���ò�ֵ����ͻ��������ʾ�����ƶ�ʱ���ٵ�����
            this.transform.rotation = Quaternion.Euler(direction);
            this.transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5f);
        }
        else
        {
            // ��ҿ��ƵĽ�ɫ��ʵʱ����ҿ��ƵĽ�ɫ�����Դ���������
            this.position = transform.position;
            this.direction = transform.rotation.eulerAngles;
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
        if (isMine) 
        {
            this.transform.position = position;
            this.transform.rotation = Quaternion.Euler(direction);
        }
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
