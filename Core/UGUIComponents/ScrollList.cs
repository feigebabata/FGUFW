using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW
{
    [DisallowMultipleComponent]
    public class ScrollList : MonoBehaviour
    {
        public ScrollRect Scroll;
        public ListDirection Direction;
        public float Spacing=5;
        private float _itemOffset,_resetSpace,_lastResetPosition;
        private int _viewItemCount,_length;
        
        private Action<int,Transform> _resetItem;


        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if(Direction==ListDirection.Vertical)
            {
                _itemOffset = transform.GetChild(0).AsRT().sizeDelta.y+Spacing;
                _viewItemCount = Mathf.CeilToInt(Scroll.transform.AsRT().rect.height/_itemOffset);
            }
            else
            {
                _itemOffset = transform.GetChild(0).AsRT().sizeDelta.x+Spacing;
                _viewItemCount = Mathf.CeilToInt(Scroll.transform.AsRT().rect.width/_itemOffset);
            }
            _resetSpace = _itemOffset * _viewItemCount;
            Scroll.onValueChanged.AddListener(onScroll);
        }

        public void Init(int length,int index,Action<int,Transform> resetItem)
        {
            _length = length;
            _resetItem = resetItem;

            for (int i = 0; i < length && i<_viewItemCount*3; i++)
            {
                if(i<transform.childCount)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    var item = transform.GetChild(0).gameObject.Copy(transform);
                    item.SetActive(false);
                }
            }

            if(Direction==ListDirection.Vertical)
            {
                var anchoredPosition = transform.AsRT().anchoredPosition;
                anchoredPosition.y = index*_itemOffset;
                transform.AsRT().anchoredPosition = anchoredPosition;
                
                var sizeDelta = transform.AsRT().sizeDelta;
                sizeDelta.y = _itemOffset*length;
                transform.AsRT().sizeDelta = sizeDelta;
            }
            else
            {
                var anchoredPosition = transform.AsRT().anchoredPosition;
                anchoredPosition.x = -index*_itemOffset;
                transform.AsRT().anchoredPosition = anchoredPosition;
                
                var sizeDelta = transform.AsRT().sizeDelta;
                sizeDelta.x = _itemOffset*length;
                transform.AsRT().sizeDelta = sizeDelta;
            }

            resetItemPos(index);
        }
        

        private void onScroll(Vector2 arg0)
        {
            if(Direction==ListDirection.Vertical)
            {
                var offset = transform.AsRT().anchoredPosition.y;
                if(Mathf.Abs(offset-_lastResetPosition)>_resetSpace)
                {
                    var index = Mathf.CeilToInt(offset/_itemOffset);
                    resetItemPos(index);
                    _lastResetPosition = offset;
                }
            }
            else
            {
                var offset = transform.AsRT().anchoredPosition.x;
                if(Mathf.Abs(offset-_lastResetPosition)>_resetSpace)
                {
                    var index = Mathf.CeilToInt(offset/_itemOffset);
                    resetItemPos(index);
                    _lastResetPosition = offset;
                }
            }
        }

        private void resetItemPos(int index)
        {
            foreach (Transform item in transform)
            {
                item.gameObject.SetActive(false);
            }
            index -= _viewItemCount;
            for (int i = index<0?0:index,childIndex=0; i<_length && childIndex<transform.childCount; i++,childIndex++)
            {
                var item = transform.GetChild(childIndex).AsRT();
                if(Direction==ListDirection.Vertical)
                {
                    var anchoredPosition = item.anchoredPosition;
                    anchoredPosition.y = -i*_itemOffset;
                    item.anchoredPosition = anchoredPosition;
                }
                else
                {
                    var anchoredPosition = item.anchoredPosition;
                    anchoredPosition.x = i*_itemOffset;
                    item.anchoredPosition = anchoredPosition;
                }
                _resetItem(i,item);
                item.gameObject.SetActive(true);
            }
        }

        public enum ListDirection
        {   
            Vertical = 0,
        
            Horizontal = 1
        }
    }
}
