using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FGUFW.Gameplay
{
    public abstract class Part:MonoBehaviour
    {
        private CancellationTokenSource _partTaskCancellationTokenSource;
        protected CancellationToken partTaskCancellationToken=>_partTaskCancellationTokenSource.Token;

        internal bool initialed{get;private set;}

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Reset()
        {
            gameObject.name = this.GetType().Name;
        }

        void OnEnable()
        {
            _partTaskCancellationTokenSource = new CancellationTokenSource();
        }

        void OnDisable()
        {
            _partTaskCancellationTokenSource.Cancel();
        }

        void Start()
        {
            tryInitial().Forget();
        }

        void OnDestroy()
        {
            OnPartDestroy();
        }

        protected abstract void OnPartInitialed();
        protected abstract void OnPartDestroy();

        private async UniTask tryInitial()
        {
            await waitPrevPartInitialed();
            await injectField();
            initialed = true;

            OnPartInitialed();
        }

        private async UniTask injectField()
        {
            var partDIContainer = FindPartDIContainer(transform);

            await partDIContainer.TryInject(this,partTaskCancellationToken);
        }

        private async UniTask waitPrevPartInitialed()
        {
            Part prevPart = transform.GetObjectFormUp<Part>(true);

            if(prevPart!=default && prevPart!=this)
            {
               await UniTask.WaitUntil(()=>prevPart.initialed,PlayerLoopTiming.Update,partTaskCancellationToken);
            }
            prevPart = default;
        }

        public static PartDIContainer FindPartDIContainer(Transform node)
        {
            PartDIContainer partDIContainer=node.GetObjectFormUpOrSelf<PartDIContainer>(true);

            if(partDIContainer==default)
            {
                partDIContainer = GameObject.FindFirstObjectByType<PartDIContainer>(FindObjectsInactive.Exclude);
                
            }

            return partDIContainer;
        }


    }

}