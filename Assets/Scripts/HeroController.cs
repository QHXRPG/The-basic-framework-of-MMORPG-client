using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    //根据标签player获取英雄对象
    private GameObject hero;

    public static float originalSpeed = 4;
    public float speed = originalSpeed; //每秒移动的距离

    //是否调整视角
    public bool AdjustView{get; set;}
    //是否调整距离
    public bool AdjustDistance{get; set;}

    private Camera camera;
    private CharacterController characterController;

    //记录摄像机相对位置
    Vector3 offset;


    HeroAnimations anim;

    void Start()
    {
        camera = Camera.main;
        //根据标签player获取英雄对象
        hero = this.gameObject;
        //摄像机移动到英雄后方且对准英雄
        camera.transform.position = hero.transform.position - hero.transform.forward * 8 + Vector3.up * 3;
        camera.transform.LookAt(hero.transform);
        offset = camera.transform.position - hero.transform.position;
        anim = GetComponent<HeroAnimations>();  // 在当前游戏对象身上取出 HeroAnimations组件
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        camera = Camera.main;
        if (Input.GetKeyDown(KeyCode.Space)) // 空格为攻击动作
        { 
            anim.PlayAttack1();
        }

        //摄像机跟随英雄移动
        camera.transform.position = hero.transform.position + offset;

        //鼠标滚轮控制摄像机距离英雄的距离
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            camera.transform.position += camera.transform.forward * 1.5f * wheel;
            offset = camera.transform.position - hero.transform.position;
        }

        //鼠标右键控制摄像机绕英雄旋转
        if (Input.GetMouseButton(1))
        {
            //Debug.Log("Mouse 0");
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            camera.transform.RotateAround(hero.transform.position, Vector3.up, x * 2);
            camera.transform.RotateAround(hero.transform.position, camera.transform.right, -y * 2);
            offset = camera.transform.position - hero.transform.position;
        }

        //offset最大距离为20
        offset = Vector3.ClampMagnitude(offset, 20);

        this.speed = GetComponent<GameEntity>().speed;

        //this._speed = GetComponent<GameEntity>()._speed;

        //控制英雄移动
        float h = 0;
        float v = 0;
        if(h==0)h=Input.GetAxis("Horizontal");
        if(v==0)v=Input.GetAxis("Vertical");
        if (h != 0 || v != 0)  // 开始移动
        {
            //播放跑步动画
            anim.PlayRun();
            if (anim.state == HeroAnimations.HState.Run)
            {
                // 检测玩家按下了左 Shift 键
                // 如果按下了左 Shift 键，将移动速度调整为原始速度的两倍
                if (Input.GetKey(KeyCode.LeftShift)) speed = originalSpeed * 2.0f;
                else speed = originalSpeed; // 如果没有按下左 Shift 键，恢复到原始的移动速度

                //摇杆控制英雄沿着摄像机的方向移动
                Vector3 dir = camera.transform.forward * v + camera.transform.right * h;
                dir.y = 0;
                dir.Normalize();
                hero.transform.forward = dir;
                characterController.Move(dir * speed * Time.deltaTime);

            }
        }
        else  // 不移动的时候
        {
            //播放待机动画
            anim.PlayIdle();
        }


        //用射线检测摄像机与英雄之间是否有障碍物
        RaycastHit hit;
        // if (Physics.Linecast(hero.transform._position, camera.transform._position, out hit))
        // {
        //     //临时移动摄像机到障碍物的位置
        //     camera.transform._position = hit.point;
        // }
        
        //射线检测摄像机不能穿过地面
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Actor");
        if (Physics.Linecast(hero.transform.position+Vector3.up*0.5f, 
                            camera.transform.position-Vector3.up*0.3f, out hit, ~layerMask))
        {
            //临时移动摄像机到障碍物的位置
            camera.transform.position = hit.point + Vector3.up * 0.5f;
        }

    }


}
