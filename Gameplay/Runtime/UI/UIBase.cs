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
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = UICamera.I.Camera;
        }

        public virtual void Show()
        {
            canvas.RegisterSort();
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            canvas.UnregisterSort();
            gameObject.SetActive(false);
        }    

        public void Release()
        {
            AssetHelper.ReleaseInstance(gameObject);
        }
    }

}
