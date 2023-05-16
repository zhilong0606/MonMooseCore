using System.Collections;
using UnityEngine;

namespace MonMoose.Core
{
    public class CustomLerpInitializer : Initializer
    {
        private string path = "Exporter/Configs/Lerp/ScriptableObjectInventory";

        protected override IEnumerator OnProcess()
        {
            GameObject go = ResourceManager.instance.LoadSync<GameObject>(path);
            if (go != null)
            {
                ScriptableObjectInventory inventory = go.GetComponent<ScriptableObjectInventory>();
                for (int i = 0; i < inventory.Count; ++i)
                {
                    LerpUtility.AddCustomFunc(i, inventory[i] as CurveLerpFunc);
                }
            }
            yield return null;
        }
    }
}
