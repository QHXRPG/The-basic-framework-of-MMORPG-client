using Assets.Scripts.u3d_scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Dropdown dropdown;
    public AbilityGroup abilityGroup;
    public UnitFrame playerFrame;  // 主角信息框
    public UnitFrame targetFrame;  // 目标信息框

    // Start is called before the first frame update
    void Start()
    {
        // 添加一个监听器来监听分辨率下拉框的值
        dropdown.onValueChanged.AddListener ((value) =>
        {
            Debug.Log(value.ToString());
            if(value == 0)
            {
                // Screen.SetResolution(宽, 高, 是否全屏);
                Screen.SetResolution(1280, 720, false);
            }
            else if(value == 1) 
            {
                Screen.SetResolution(800, 600, false);
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
        });
    }

    // OnUpdate is called once per frame
    void Update()
    {
        abilityGroup.gameObject.SetActive(GameApp.Character != null);
        playerFrame.gameObject.SetActive(GameApp.Character != null);
        targetFrame.gameObject.SetActive(GameApp.Target != null);

        // 但角色登录才显示技能栏 
        playerFrame.actor = GameApp.Character;
        targetFrame.actor = GameApp.Target; 

    }
}
