using System;

namespace FGUFW.MonoGameplay
{
    public interface IPartUpdate
    {
        void OnUpdate(in PlayFrameData playFrameData);
    }

    [Serializable]
    public struct PlayFrameData
    {
        public int Index;
        public float WorldTime;
        public float DeltaTime;
    }
}