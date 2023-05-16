using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        public static T GetComponentSafely<T>(this Component compo) where T : Component
        {
            if (compo == null)
            {
                return null;
            }
            GameObject go = compo.gameObject;
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component compo) where T : Component
        {
            if (compo == null)
            {
                return null;
            }
            GameObject go = compo.gameObject;
            if (go == null)
            {
                return null;
            }
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static void SetActiveSafely(this Component compo, bool isActive)
        {
            if (compo != null)
            {
                compo.gameObject.SetActiveSafely(isActive);
            }
        }
    }
}
