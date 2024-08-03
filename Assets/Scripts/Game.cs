using GameClient.Entities;
using GameClient.Mgr;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace GameClient
{
    public class Game
    {
        public static Actor GetUnit(int entityId)
        {
            return EntityManager.Instance.GetEntity<Actor>(entityId);
        }
    }
}

