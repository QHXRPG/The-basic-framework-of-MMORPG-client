using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Dropdown dropdown;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
