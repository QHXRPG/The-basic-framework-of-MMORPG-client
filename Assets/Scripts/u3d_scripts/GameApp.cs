using GameClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.u3d_scripts
{
    public class GameApp
    {
        //  全局角色
        public static Character Character;

        // 选择的目标
        public static Actor Target;

        // 加载游戏场景
        public static void LoadSpace(int spaceId)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var spaceDefine = DataManager.Instance.Spaces[spaceId];
                SceneManager.LoadScene(spaceDefine.Resource);
            });
        }
    }
}
