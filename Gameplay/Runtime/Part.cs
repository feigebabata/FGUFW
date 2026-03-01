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

        void ODestroy()
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
            var partFieldInjector = FindPartFieldInjector(transform);

            await partFieldInjector.TryInject(this,partTaskCancellationToken);
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

        public static PartFieldInjector FindPartFieldInjector(Transform node)
        {
            PartFieldInjector partFieldInjector=node.GetObjectFormUpOrSelf<PartFieldInjector>(true);

            if(partFieldInjector==default)
            {
                partFieldInjector = GameObject.FindFirstObjectByType<PartFieldInjector>(FindObjectsInactive.Exclude);
                
            }

            return partFieldInjector;
        }


    }

}