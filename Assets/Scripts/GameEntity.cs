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

    // �����������ͬ������
    IEnumerator SyncRequest()
    {
        while (true)
        {
            SpaceEntitySyncRequest request = new SpaceEntitySyncRequest();
            request.EntitySync = new NEntitySync();
            request.EntitySync.Entity = new NEntity();
            request.EntitySync.Entity.Position = ToNVector3(this.position);
            request.EntitySync.Entity.Direction = ToNVector3(this.direction);
            request.EntitySync.Entity.Id = entityId;
            Debug.Log(request);
            NetClient.Send(request);

            yield return new WaitForSeconds(0.1f); // �ȴ�0.1��
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMine)  // ��������ɫ�����Լ��ģ�ʵʱͬ������˴���������
        {
            this.transform.position = new Vector3(position.x, position.y, position.z);
            this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
        }
        else
        {
            // ��ҿ��ƵĽ�ɫ��ʵʱ����ҿ��ƵĽ�ɫ�����Դ���������
            this.position = transform.position;
            var q = transform.rotation;
            this.direction = new Vector3(q.x, q.y, q.z);
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
            this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
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
}
