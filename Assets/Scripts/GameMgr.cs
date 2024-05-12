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

    // Update is called once per frame
    void Update()
    {
        
    }
}
