using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FGUFW
{
    [AddComponentMenu("UI/ToggleGO", 10)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class ToggleGO : MonoBehaviour, IPointerClickHandler
    {
        public GameObject On;
        public GameObject Off;

        public Toggle.ToggleEvent onValueChanged;

        [SerializeField]
        private bool _isOn;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if(_isOn!=value)
                {
                    onValueChanged.Invoke(value);
                }
                _isOn = value;
                resetView();
            }
        }

        void OnValidate()
        {
            resetView();
        }

        private void resetView()
        {
            On?.SetActive(_isOn);
            Off?.SetActive(!_isOn);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsOn = !IsOn;
        }
    }
}
