using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FGUFW
{
    public struct CoroutineGroup
    {
        private MonoBehaviour _mb;
        private List<Coroutine> _ls;

        public CoroutineGroup(MonoBehaviour mb)
        {
            _mb = mb;
            _ls = new List<Coroutine>();
        }

        public Coroutine Start(IEnumerator enumerator)
        {
            var c = _mb.StartCoroutine(enumerator);
            _ls.Add(c);
            return c;
        }


        public IEnumerator WaitAllStop()
        {
            foreach (var item in _ls)
            {
                yield return item;
            }
        }

    }

}