using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    [RequireComponent(typeof(ContentSizeFitter))]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/SpriteNumberText", 10)]
    public class SpriteNumberText : MonoBehaviour
    {
        public const int CHAR_COUNT = 11;

        [Header("Sprite: 0~9+.")]
        public Sprite[] Chars=new Sprite[CHAR_COUNT];

        [SerializeField] float _number;
        public float Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;

                resetNumberSprite();
            }
        }

        [SerializeField] float _size=50;
        public float Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;

                resetNumberSprite();
            }
        }

        ContentSizeFitter _contentSizeFitter;
        private ContentSizeFitter contentSizeFitter
        {
            get
            {
                if(_contentSizeFitter==default)
                {
                    _contentSizeFitter = GetComponent<ContentSizeFitter>();
                }
                return _contentSizeFitter;
            }
        }

        HorizontalLayoutGroup _horizontalLayoutGroup;
        private HorizontalLayoutGroup horizontalLayoutGroup
        {
            get
            {
                if(_horizontalLayoutGroup==default)
                {
                    _horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
                }
                return _horizontalLayoutGroup;
            }
        }

        void OnValidate()
        {
            resetNumberSprite();
        }

        private void resetNumberSprite()
        {
            if(transform.childCount==0)
            {
                var item = new GameObject();
                item.transform.SetParent(transform);
                item.AddComponent<Image>();
            }

            if(Chars.Length!=CHAR_COUNT)
            {
                var newChars = new Sprite[CHAR_COUNT];
                for (int i = 0; i < CHAR_COUNT; i++)
                {
                    if(i<Chars.Length)
                    {
                        newChars[i] = Chars[i];
                    }
                }
                Chars = newChars;
            }

            if(_size==0)
            {
                _size=1;
            }

            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            var text = _number.ts();
            transform.Foreach<Image,char>(text,(img,code)=>
            {
                img.name = code.ts();
                var numSprite = Chars[getCodeIndex(code)];
                img.sprite = numSprite;
                img.rectTransform.sizeDelta = new Vector2(_size,_size);
                if(!numSprite.IsNull())
                {
                    img.SetSizeFlexibleHeight();
                }
            });
        }

        int getCodeIndex(char code)
        {
            if (code >= '0' && code <= '9')return code - '0';
            return 10; // 小数点
        }


    }
    
}
