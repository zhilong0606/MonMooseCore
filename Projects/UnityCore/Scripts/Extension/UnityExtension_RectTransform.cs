using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        public static Vector2 CenterPosition(this RectTransform rectTransform)
        {
            float posX = rectTransform.position.x - rectTransform.rect.width * rectTransform.lossyScale.x * (rectTransform.pivot.x - 0.5f);
            float posY = rectTransform.position.y - rectTransform.rect.height * rectTransform.lossyScale.y * (rectTransform.pivot.y - 0.5f);
            return new Vector2(posX, posY);
        }
    }
}
