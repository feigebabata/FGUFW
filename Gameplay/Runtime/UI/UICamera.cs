using UnityEngine;

namespace FGUFW.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class UICamera : MonoSingleton<UICamera>
    {
        public Camera Camera{get;private set;}

        protected override bool IsDontDestroyOnLoad()
        {
            this.Camera = GetComponent<Camera>();
            return true;
        }
    }
}
