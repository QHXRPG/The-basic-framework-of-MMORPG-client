using Assets.Scripts.u3d_scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Dropdown dropdown;
    public AbilityGroup abilityGroup;
    public UnitFrame playerFrame;  // ������Ϣ��
    public UnitFrame targetFrame;  // Ŀ����Ϣ��

    // Start is called before the first frame update
    void Start()
    {
        // ���һ���������������ֱ����������ֵ
        dropdown.onValueChanged.AddListener ((value) =>
        {
            Debug.Log(value.ToString());
            if(value == 0)
            {
                // Screen.SetResolution(��, ��, �Ƿ�ȫ��);
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

        // ����ɫ��¼����ʾ������ 
        playerFrame.actor = GameApp.Character;
        targetFrame.actor = GameApp.Target; 

    }
}
