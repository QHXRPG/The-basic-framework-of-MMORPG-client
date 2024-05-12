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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(position.x, position.y, position.z);
        this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
    }

    // ������˵���������Ϊ�ͻ��˵�����
    public void SetData(NEntity nEntity)
    {
        this.entityId = nEntity.Id;
        var p = nEntity.Position;
        var d = nEntity.Direction;
        this.position = new Vector3(p.X, p.Y , p.Z );
        this.direction = new Vector3(d.X, d.Y , d.Z );
        position *= 0.001f;
        direction *= 0.001f;
    }
}
