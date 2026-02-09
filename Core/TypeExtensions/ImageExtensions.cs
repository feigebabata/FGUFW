using UnityEngine;
using System;
using UnityEngine.UI;

namespace FGUFW
{
    public static class ImageExtensions
    {
        public static void SetSizeFlexibleWidth(this Image self)
        {
            var sprite = self.sprite;
            if(sprite.IsNull())return;

            var size = sprite.textureRect.size;

            var sizeDelta = self.rectTransform.sizeDelta;
            sizeDelta.y = sizeDelta.x * size.y / size.x;

            self.rectTransform.sizeDelta = sizeDelta;
        }

        public static void SetSizeFlexibleHeight(this Image self)
        {
            var sprite = self.sprite;
            if(sprite.IsNull())return;

            var size = sprite.textureRect.size;

            var sizeDelta = self.rectTransform.sizeDelta;
            sizeDelta.x = sizeDelta.y * size.x / size.y;

            self.rectTransform.sizeDelta = sizeDelta;
        }
    }
}
