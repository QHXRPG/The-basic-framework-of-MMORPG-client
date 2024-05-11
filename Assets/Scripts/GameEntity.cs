using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;

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

    public void SetData()
    {

    }
}
