using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FGUFW.Flipbook.Editor
{
    [CustomEditor(typeof(SpriteFlipbookAnim))]
    public class SpriteFlipbookAnimEditor : UnityEditor.Editor
    {
        private SpriteFlipbookAnim _target;

        private void OnEnable() 
        {
            _target = target as SpriteFlipbookAnim;
            _target.Apply();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("初始化"))
            {
                _target.transform.localScale = Vector3.one;

                if(!_target.Render.sprite.IsNull())
                {
                    var flipboolSize = _target.FlipbookSize;
                    var textureHeight = _target.Render.sprite.texture.height;
                    var scale = _target.Render.sprite.pixelsPerUnit;
                    _target.SizeHeight = textureHeight/scale/flipboolSize.y;
                }

            }

            // if(EditorApplication.isPlaying)
            // {
            //     if(GUILayout.Button("Apply"))
            //     {
            //         _target.Apply();
            //     }
            // }
            // else
            // {
            //     _target.Apply();
            // }

            if(_target.EditorAutoApple)
            {
                _target.Apply();
            }

            if(GUILayout.Button("Apply"))
            {
                _target.Apply();
            }
        }

    }
}
