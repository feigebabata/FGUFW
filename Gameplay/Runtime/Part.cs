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

        private async UniTask tryInitial()
        {
            await waitPrevPartInitialed();
            await injectField();
            initialed = true;
        }

        private async UniTask injectField()
        {
            var partFieldInjector = findPartFieldInjector();

            await partFieldInjector.TryInject(this,partTaskCancellationToken);
        }

        private async UniTask waitPrevPartInitialed()
        {
            Part prevPart = findPrevPart();
            if(prevPart!=default)
            {
                while (!prevPart.initialed && !partTaskCancellationToken.IsCancellationRequested)
                {
                    await UniTask.DelayFrame(1);
                }
            }
        }

        private PartFieldInjector findPartFieldInjector()
        {
            PartFieldInjector partFieldInjector=default;

            var node = transform;

            do
            {
                partFieldInjector = node.GetComponent<PartFieldInjector>();

                if(partFieldInjector!=default)
                {
                    return partFieldInjector;
                }

                if(node.GetSiblingIndex()==0)
                {
                    node = node.parent;
                }
                else
                {
                    node = node.parent.GetChild(node.GetSiblingIndex()-1);
                }
            }
            while (node.parent!=default);

            partFieldInjector = GameObject.FindFirstObjectByType<PartFieldInjector>(FindObjectsInactive.Exclude);
            return partFieldInjector;
        }

        private Part findPrevPart()
        {
            var node = transform;

            do
            {
                if(node.GetSiblingIndex()==0)
                {
                    node = node.parent;
                }
                else
                {
                    node = node.parent.GetChild(node.GetSiblingIndex()-1);
                }

                if(node.gameObject.activeSelf)
                {
                    return node.GetComponent<Part>();
                }
            }
            while (node.parent!=default);

            return default;
        }
    }

}