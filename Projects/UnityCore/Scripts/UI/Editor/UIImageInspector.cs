#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace MonMoose.Core
{
    [CustomEditor(typeof(UIImage))]
    public class UIImageInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIImage image = target as UIImage;
            if (image != null)
            {
                if (GUILayout.Button("SetNativeSize"))
                {
                    image.SetNativeSize();
                }
                if (GUILayout.Button("SetUIMat"))
                {
                    GameObject go = Resources.Load("UIImageTemplate") as GameObject;
                    image.material = go.GetComponent<UIImage>().material;
                    if (image.sprite != null)
                    {
                        if (image.sprite.rect.width > image.sprite.rect.height)
                        {
                            image.TextureLayout = ETextureLayout.Horizonatal;
                        }
                        else
                        {
                            image.TextureLayout = ETextureLayout.Vertical;
                        }
                    }
                    image.SetAllDirty();
                }
            }
        }
    }
}
#endif