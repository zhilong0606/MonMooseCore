using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonMoose.Core
{
    public class SceneInfo
    {
        public bool useIndex;
        public string path;
        public int index;
        public Scene scene;

        public SceneInfo(string path)
        {
            this.path = path;
            useIndex = false;
        }

        public SceneInfo(int index)
        {
            this.index = index;
            useIndex = true;
        }
    }
}
