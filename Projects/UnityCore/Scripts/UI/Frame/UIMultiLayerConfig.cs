using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMultiLayerConfig : MonoBehaviour
    {
        [SerializeField] private bool m_isImmortal = false;
        [SerializeField] private List<SceneLayerBase> m_layerList = new List<SceneLayerBase>();

        public bool isImmortal
        {
            get { return m_isImmortal; }
        }

        public List<SceneLayerBase> layerList
        {
            get { return m_layerList; }
        }
    }
}
