using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Flipbook
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFlipbookAnim : MonoBehaviour
    {
        readonly public static SpritePropertyName PropertyName = new SpritePropertyName();

        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer Render
        {
            get
            {
                if(!(_spriteRenderer is UnityEngine.Object) || _spriteRenderer==null)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }
                return _spriteRenderer;
            }
        }

        private MaterialPropertyBlock _materialPropertyBlock;
        public MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if(_materialPropertyBlock==null)
                {
                    _materialPropertyBlock = new MaterialPropertyBlock();
                }
                return _materialPropertyBlock;
            }
        }

        #if UNITY_EDITOR
        public bool EditorAutoApple = false;
        #endif

        public bool StartFristFrame = true;
        public bool Loop = true;
        public float SizeHeight = 1f;
        public Vector2 Pivot = new Vector2(0.5f,0.5f);
        public Vector2Int FlipbookSize = new Vector2Int(1,1);
        public int FrameRate=1;
        public int Start;
        public int Length=1;
        public float OffsetTime;

        private void Reset() 
        {
            Render.drawMode = SpriteDrawMode.Sliced;
            #if UNITY_EDITOR
            Render.sharedMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/FGUFW/Flipbook/Runtime/SpriteFilebookAnimAuto.mat");
            #endif
            Render.size = Vector2.one;
            Apply();
        }

        private void Awake() 
        {
            Apply();
        }

        void OnEnable()
        {
            if(StartFristFrame)Apply();
        }

        public void Apply()
        {
            Render.GetPropertyBlock(PropertyBlock);
            var size = Vector2.one;
            if(!Render.sprite.IsNull())
            {
                var girdSize = new Vector2(Render.sprite.texture.width/FlipbookSize.x,Render.sprite.texture.height/FlipbookSize.y);
                PropertyBlock.SetTexture(PropertyName.MainTex,Render.sprite.texture);
                size = new Vector2(girdSize.x*SizeHeight/girdSize.y,SizeHeight);
                Render.size = size;
            }

            if(StartFristFrame)OffsetTime = - Time.time;

            PropertyBlock.SetVector(PropertyName.Pivot,Pivot);
            PropertyBlock.SetVector(PropertyName.FlipbookSize,new Vector4(FlipbookSize.x,FlipbookSize.y));
            PropertyBlock.SetFloat(PropertyName.FrameRate,FrameRate);
            PropertyBlock.SetFloat(PropertyName.Start,Start);
            PropertyBlock.SetFloat(PropertyName.Length,Length);
            PropertyBlock.SetFloat(PropertyName.OffsetTime,OffsetTime);
            PropertyBlock.SetFloat(PropertyName.Loop,Loop?1:0);
            PropertyBlock.SetVector(PropertyName.ClipSize,new Vector4(size.x,size.y));
            Render.SetPropertyBlock(PropertyBlock);

        }

        public class SpritePropertyName
        {
            public readonly int MainTex;
            public readonly int Pivot;
            public readonly int FlipbookSize;
            public readonly int FrameRate;
            public readonly int Start;
            public readonly int Length;
            public readonly int OffsetTime;
            public readonly int Loop;
            public readonly int ClipSize;

            public SpritePropertyName()
            {
                MainTex = Shader.PropertyToID("_MainTex");
                Pivot = Shader.PropertyToID("_Pivot");
                FlipbookSize = Shader.PropertyToID("_FlipbookSize");
                FrameRate = Shader.PropertyToID("_FrameRate");
                Start = Shader.PropertyToID("_Start");
                Length = Shader.PropertyToID("_Length");
                OffsetTime = Shader.PropertyToID("_OffsetTime");
                Loop = Shader.PropertyToID("_Loop");
                ClipSize = Shader.PropertyToID("_ClipSize");
            }
        }

    }
}
