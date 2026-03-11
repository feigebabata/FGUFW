using UnityEngine;

namespace FGUFW.Gameplay
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIBase:MonoBehaviour
    {
        protected Canvas canvas;

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = UICamera.I.Camera;
        }

        public virtual void Show()
        {
            canvas.OrderIssue();
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            canvas.OrderRecycle();
            gameObject.SetActive(false);
        }    
    }

}