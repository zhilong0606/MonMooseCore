using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ColliderConfig : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_linkedObj;
        [SerializeField]
        private EnumString<ColliderType> m_colliderType = new EnumString<ColliderType>();

        public GameObject linkedObj
        {
            get { return m_linkedObj; }
        }

        public ColliderType colliderType
        {
            get { return m_colliderType.value; }
        }
    }
}
