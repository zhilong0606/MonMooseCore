using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        public static void SetPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        public static void SetPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        public static void SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.position;
            pos.x = x;
            transform.localPosition = pos;
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.localPosition = pos;
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.position;
            pos.z = z;
            transform.localPosition = pos;
        }
    }
}
