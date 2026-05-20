using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FGUFW
{
    public interface IAssetLoader
    {
        T Load<T>(string path);
        UniTask<T> LoadAsync<T>(string path,CancellationToken cancellationToken);
        GameObject Instantiate(string path,Transform parent);
        UniTask<GameObject> InstantiateAsync(string path,Transform parent,CancellationToken cancellationToken);
        UniTask LoadSceneAsync(string path,LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        void ReleaseInstance(GameObject game);
    }
}
