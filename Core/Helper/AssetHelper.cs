using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FGUFW
{
    public static class AssetHelper
    {

        public static AsyncOperationHandle<T> LoadAsync<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path);
        }

        public static AsyncOperationHandle<IList<T>> LoadsAsync<T>(string path,Action<T> callback=null)
        {
            return Addressables.LoadAssetsAsync<T>(path,callback);
        }

        public static T Load<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }

        public static IList<T> Loads<T>(string path)
        {
            return Addressables.LoadAssetsAsync<T>(path,null).WaitForCompletion();
        }

        public static AsyncOperationHandle<GameObject> CopyAsync(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent);
        }

        public static GameObject Copy(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent).WaitForCompletion();
        }

        public static SceneInstance LoadScene(string path)
        {
            return Addressables.LoadSceneAsync(path).WaitForCompletion();
        }
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(string path,LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            return Addressables.LoadSceneAsync(path,loadSceneMode);
        }


    }


}
