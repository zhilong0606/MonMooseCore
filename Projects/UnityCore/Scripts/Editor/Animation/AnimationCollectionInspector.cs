using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomEditor(typeof(AnimationCollection))]
    public class AnimationCollectionInspector : Editor
    {
        public static List<AnimationClip> collectedAnimationClipList = new List<AnimationClip>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var collection = target as AnimationCollection;
            bool isDirty = false;
            if (GUILayout.Button("Clear"))
            {
                collection.clipList.Clear();
                isDirty = true;
            }
            if (GUILayout.Button("AddCollect"))
            {
                collection.clipList.AddRange(collectedAnimationClipList);
                collection.clipList = collection.clipList.Distinct().ToList();
                collection.clipList.Sort(OnSort);
                isDirty = true;
            }
            if (GUILayout.Button("UseCollect"))
            {
                collection.clipList = collectedAnimationClipList.Distinct().ToList();
                collection.clipList.Sort(OnSort);
                isDirty = true;
            }
            if (isDirty)
            {
                //serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                //AssetDatabase.SaveAssetIfDirty(target);
            }
            AssetDatabase.SaveAssets();
        }

        private static int OnSort(AnimationClip a, AnimationClip b)
        {
            return string.Compare(a.name, b.name, StringComparison.Ordinal);
        }
    }
}
