using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW.UGUILayout
{
    [ExecuteAlways]
    // [RequireComponent(typeof(CanvasRenderer))]
    public class LayoutGroup : MonoBehaviour
    {
        public RectOffset Padding = new RectOffset();
        public float Spacing;
        public Mode Type;
        public Align Alignment;
        public float MaxSize = float.MaxValue;

        // [HideInInspector]
        public bool IsChild;

        private RectTransform _rectTransform;


        void OnEnable()
        {
            if(!IsChild)
            {
                Execute();
            }
        }

        public void Execute()
        {
            _rectTransform = transform as RectTransform;
            
            if(this.Type == Mode.Horizontal)
            {
                setHorizontal();
            }
            else if(this.Type == Mode.Vertical)
            {
                setVertical();
            }
        }

        public void SetAnchors(RectTransform rect)
        {
            var anchorMin = rect.anchorMin;
            var anchorMax = rect.anchorMax;
            var pivot = rect.pivot;

            if(Type== Mode.Horizontal)
            {
                if(Alignment == Align.LeftOrBottom)
                {
                    anchorMin.x = 0;
                    anchorMax.x = 0;
                    pivot.x = 0;
                }
                else
                {
                    anchorMin.x = 1;
                    anchorMax.x = 1;
                    pivot.x = 1;
                }
            }
            else
            {
                if(Alignment == Align.LeftOrBottom)
                {
                    anchorMin.y = 0;
                    anchorMax.y = 0; ;
                    pivot.y = 0;
                }
                else
                {
                    anchorMin.y = 1;
                    anchorMax.y = 1;
                    pivot.y = 1;
                }
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
        }

        private void setVertical()
        {
            float align = 1;
            float size = Padding.bottom;


            if(this.Alignment == Align.RightOrTop)
            {
                align = -1;
                size = Padding.top;
            }

            int childCount = 0;

            foreach(RectTransform child in _rectTransform)
            {
                if(!child.gameObject.activeSelf)continue;

                SetAnchors(child);

                var layoutGroup = child.GetComponent<LayoutGroup>();
                if(layoutGroup != default)
                {
                    layoutGroup.IsChild = true;
                    layoutGroup.Execute();
                }
                var pos = child.anchoredPosition;
                pos.y = size*align;
                child.anchoredPosition = pos;

                size += child.sizeDelta.y+Spacing;
                childCount++;
            }
            if(childCount>1)size -= Spacing;
            if(this.Alignment == Align.RightOrTop)
            {
                size += Padding.bottom;
            }
            else if(this.Alignment == Align.LeftOrBottom)
            {
                size += Padding.top;
            }

            var sizeDelta = _rectTransform.sizeDelta;
            sizeDelta.y = Mathf.Clamp(size,0,MaxSize);
            _rectTransform.sizeDelta = sizeDelta;
        }

        private void setHorizontal()
        {
            float align = 1;
            float size = Padding.left;


            if(this.Alignment == Align.RightOrTop)
            {
                align = -1;
                size = Padding.right;
            }
            
            // _rectTransform.anchorMin = anchorMin;
            // _rectTransform.anchorMax = anchorMax;
            int childCount = 0;

            foreach(RectTransform child in _rectTransform)
            {
                if(!child.gameObject.activeSelf)continue;

                SetAnchors(child);

                var layoutGroup = child.GetComponent<LayoutGroup>();
                if(layoutGroup != default)
                {
                    layoutGroup.Execute();
                }
                var pos = child.anchoredPosition;
                pos.x = size*align;
                child.anchoredPosition = pos;

                size += child.sizeDelta.x+Spacing;
                childCount++;
            }            
            if(childCount>1)size -= Spacing;
            // Debug.Log($"{name}" + size);
            if(this.Alignment == Align.RightOrTop)
            {
                size += Padding.left;
            }
            else if(this.Alignment == Align.LeftOrBottom)
            {
                size += Padding.right;
            }

            var sizeDelta = _rectTransform.sizeDelta;
            sizeDelta.x = Mathf.Clamp(size,0,MaxSize);
            _rectTransform.sizeDelta = sizeDelta;
        }

        public enum Mode
        {
            Horizontal,
            Vertical,
        }

        public enum Align
        {
            LeftOrBottom,
            RightOrTop,
        }
    }

}
