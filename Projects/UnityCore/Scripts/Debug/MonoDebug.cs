using System;
using System.Collections;
using UnityEngine;

namespace MonMoose.Core
{
    public class MonoDebug : MonoBehaviour
    {
        public void Awake()
        {
            Debug.Log(string.Format("[MonoDebug] Awake:{0}", gameObject.name));
        }

        public void Start()
        {
            Debug.Log(string.Format("[MonoDebug] Start:{0}", gameObject.name));
        }

        public void OnEnable()
        {
            Debug.Log(string.Format("[MonoDebug] OnEnable:{0}", gameObject.name));
        }

        public void OnDisable()
        {
            Debug.Log(string.Format("[MonoDebug] OnDisable:{0}", gameObject.name));
        }

        public void OnDestroy()
        {
            Debug.Log(string.Format("[MonoDebug] OnDestroy:{0}", gameObject.name));
        }
    }
}
