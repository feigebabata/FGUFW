using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FGUFW
{
    [ExecuteInEditMode]
    public class AnimationCurveHelper : MonoBehaviour
    {
        public AnimationCurve Template;

        void OnEnable()
        {
            Debug.Log(Template.ToCodeText("Template"));

        }

        public AnimationCurve TempLinear = Linear;
        public AnimationCurve TempInOutGentle = InOutGentle;
        public AnimationCurve TempUpGentle = UpGentle;
        public AnimationCurve TempUpDown = UpDown;
        public AnimationCurve TempElastic = Elastic;
        public AnimationCurve TempOutElastic = OutElastic;

        //--------------------------------------------------------------------------------------------------

        /// <summary>
        /// 线性
        /// </summary>
        public static AnimationCurve Linear = new AnimationCurve
        {
            keys = new Keyframe[]
            {
                new Keyframe{time=0,value=0,inTangent=1,outTangent=1,inWeight=0,outWeight=0},
                new Keyframe{time=1,value=1,inTangent=1,outTangent=1,inWeight=0,outWeight=0},
            },
            preWrapMode = WrapMode.ClampForever,
            postWrapMode = WrapMode.ClampForever,
        };

        /// <summary>
        /// 首尾平中间陡
        /// </summary>
        public static AnimationCurve InOutGentle = new AnimationCurve
        {
            keys = new Keyframe[]
            {
                new Keyframe{time=0f,value=0f,inTangent=0f,outTangent=0f,inWeight=0f,outWeight=0f},
                new Keyframe{time=1f,value=1f,inTangent=0f,outTangent=0f,inWeight=0f,outWeight=0f},
            },
            preWrapMode = WrapMode.ClampForever,
            postWrapMode = WrapMode.ClampForever,
        };

        /// <summary>
        /// 越来越陡
        /// </summary>
        public static AnimationCurve UpGentle = new AnimationCurve
        {
            keys = new Keyframe[]
            {
                new Keyframe{time=0f,value=0f,inTangent=0f,outTangent=0f,inWeight=0f,outWeight=0f},
                new Keyframe{time=1f,value=1f,inTangent=2f,outTangent=2f,inWeight=0f,outWeight=0f},
            },
            preWrapMode = WrapMode.ClampForever,
            postWrapMode = WrapMode.ClampForever,
        };

        /// <summary>
        /// 起伏
        /// </summary>
        public static AnimationCurve UpDown = new AnimationCurve
        {
            keys = new Keyframe[]
            {
                new Keyframe{time=0f,value=0f,inTangent=8.141438f,outTangent=8.141438f,inWeight=0f,outWeight=0.1039179f},
                new Keyframe{time=0.25f,value=1f,inTangent=-0.01655903f,outTangent=-0.01655903f,inWeight=0.3333333f,outWeight=0.3749266f},
                new Keyframe{time=0.5f,value=0f,inTangent=-5.838706f,outTangent=-5.838706f,inWeight=0.3333333f,outWeight=0.1460991f},
                new Keyframe{time=0.75f,value=-1f,inTangent=-0.002081815f,outTangent=-0.002081815f,inWeight=0.3333333f,outWeight=0.3742657f},
                new Keyframe{time=1f,value=0f,inTangent=6.935236f,outTangent=6.935236f,inWeight=0.02278411f,outWeight=0f},
            },
            preWrapMode = WrapMode.Loop,
            postWrapMode = WrapMode.Loop,
        };

        /// <summary>
        /// 弹性
        /// </summary>
        public static AnimationCurve Elastic = new AnimationCurve
        {
            keys = new Keyframe[]
            {
                new Keyframe{time=0f,value=0f,inTangent=-1.129893f,outTangent=-1.129893f,inWeight=0f,outWeight=0.1008965f},
                new Keyframe{time=1f,value=1f,inTangent=-1.234349f,outTangent=-1.234349f,inWeight=0.08791655f,outWeight=0f},
            },
            preWrapMode = WrapMode.ClampForever,
            postWrapMode = WrapMode.ClampForever,
        };


        public static AnimationCurve OutElastic = new AnimationCurve
        {
            keys = new Keyframe[]
            {
                new Keyframe{time=0f,value=0f,inTangent=1.093894f,outTangent=1.093894f,inWeight=0f,outWeight=0.1635692f},
                new Keyframe{time=0.7479452f,value=1.005202f,inTangent=1.287608f,outTangent=1.287608f,inWeight=0.1035871f,outWeight=0.3333333f},
                new Keyframe{time=1f,value=1f,inTangent=-1.966983f,outTangent=-1.966983f,inWeight=0.2131746f,outWeight=0f},
            },
            preWrapMode = WrapMode.ClampForever,
            postWrapMode = WrapMode.ClampForever,
        };

    }
}