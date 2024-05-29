using Assets.Scripts.u3d_scripts;
using GameClient;
using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitFrame : MonoBehaviour
{
    public Image HealthBar;
    public Image ManaBar;
    public Actor actor;

    void Start()
    {
        
    }


    void Update()
    {
        if(actor == null) { return; }


        gameObject.GetComponent<CanvasGroup>().alpha = 1;
        transform.Find("Level/Text").GetComponent<Text>().text = actor.Info.Level + "";
        transform.Find("Name").GetComponent<Text>().text = actor.Info.Name + "";

        //设置蓝量和血量
        float hp = actor.Info.Hp / actor.UnitDefine.HPMax;
        float mp = actor.Info.Mp / actor.UnitDefine.MPMax;
        HealthBar.fillAmount = hp;
        ManaBar.fillAmount = mp;    
        
    }
}
