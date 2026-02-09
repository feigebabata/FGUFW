using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FGUFW.MonoGameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [HideInInspector]
        public float SwitingTime;

        [HideInInspector]
        public Effect SwitingEffect = Effect.Active;

        [HideInInspector]
        public AnimationCurve AlphaCurve,ScaleCurve,MoveCurve;

        [HideInInspector]
        public Vector2 MoveVector;

        [HideInInspector]
        public string SortOrder;

        CanvasGroup _group;
        Canvas _canvas;

        /// <summary>
        /// canvas下层 缩放用
        /// </summary>
        RectTransform _panel;

        void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
            _panel = transform.Last().AsRT();
        }

        public virtual IEnumerator Show(MonoBehaviour mb)
        {
            this._canvas.enabled = true;
            _group.interactable = false;

            
            if((this.SwitingEffect & UIPanel.Effect.Active) == UIPanel.Effect.Active)
            {
                gameObject.SetActive(true);
            }

            var t = 0f;
            do
            {
                float progress = 0;
                if(SwitingTime<=0)
                {
                    progress = 1f;
                }
                else
                {
                    progress = Mathf.Clamp01(t/SwitingTime);
                }

                setEffectByProgress(progress);

                if(progress==1)break;

                t += Time.deltaTime;
                
                yield return default;
            }
            while (true);

            _group.interactable = true;
            yield break;
        }

        public IEnumerator Hide(MonoBehaviour mb)
        {
            _group.interactable = false;

            var t = SwitingTime;
            do
            {
                float progress = 1;
                if(SwitingTime<=0)
                {
                    progress = 0;
                }
                else
                {
                    progress = Mathf.Clamp01(t/SwitingTime);
                }

                setEffectByProgress(progress);

                if(progress==0)break;

                t -= Time.deltaTime;
                
                yield return default;
            }
            while (true);

            if((this.SwitingEffect & UIPanel.Effect.Active) == UIPanel.Effect.Active)
            {
                gameObject.SetActive(false);
            }

            this._canvas.enabled = false;

            yield break;
        }

        void setEffectByProgress(float progress)
        {
            if((this.SwitingEffect & UIPanel.Effect.Alpha) == UIPanel.Effect.Alpha)
            {
                _group.alpha = AlphaCurve.Evaluate(progress);
            }
            if((this.SwitingEffect & UIPanel.Effect.Scale) == UIPanel.Effect.Scale)
            {
                _panel.localScale = ScaleCurve.Evaluate(progress) * Vector3.one;
            }
            if((this.SwitingEffect & UIPanel.Effect.Move) == UIPanel.Effect.Move)
            {
                var canvasSize = transform.AsRT().sizeDelta;
                canvasSize *= MoveVector;
                
                _panel.anchoredPosition = MoveCurve.Evaluate(progress)*canvasSize;
            }
        }

        public void SetSortOrder(int order)
        {
            _canvas.sortingOrder = order;
            SortOrder = ((UIPanelSortOrder)order).ts();
        }

        [Flags]
        public enum Effect
        {
            Nothing = 0,
            Active = 1 << 0,
            Alpha = 1 << 1,
            Move = 1 << 2,
            Scale = 1 << 3,
        }

    }



}
