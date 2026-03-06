using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FGUFW.Gameplay
{
    /// <summary>
    /// 业务功能的大模块单位
    /// </summary>
    public abstract class Play:Part
    {
        protected override void OnPartInitialed()
        {
            waitSubPartInitialed();
        }

        private async void waitSubPartInitialed()
        {
            Queue<Transform> queue = new ();
            queue.Enqueue(transform);

            while (queue.Count>0)
            {
                var part = queue.Dequeue().GetComponent<Part>();

                // Debug.Log($"wait {part.name} initialed");
                await UniTask.WaitUntil(()=>part.initialed,PlayerLoopTiming.Update,partTaskCancellationToken);

                foreach (Transform item in part.transform)
                {
                    queue.Enqueue(item);
                }
            }

            OnAllPartInitialed();
        }
        
        protected abstract void OnAllPartInitialed();

    }
}