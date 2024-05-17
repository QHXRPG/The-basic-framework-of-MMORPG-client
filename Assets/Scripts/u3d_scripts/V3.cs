using Proto.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.u3d_scripts
{
    class V3
    {
        public static Vector3 Of(NVector3 nv)
        {
            return new Vector3(nv.X, nv.Y, nv.Z);
        }
        public static Vector3 Of(Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        public static Vector3 ToVector3(NVector3 nv)
        {
            return new Vector3(nv.X, nv.Y, nv.Z);
        }
        public static NVector3 ToNVector3(Vector3 v)
        {
            return new NVector3() { X = (int)v.x, Y = (int)v.y, Z = (int)v.z };
        }

    }

}
