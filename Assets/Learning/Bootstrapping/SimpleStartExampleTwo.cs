using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[DefaultExecutionOrder(-1)]
public class SimpleStartExampleTwo : MonoBehaviour, IService {
    [SerializeField] private AssetReferenceGameObject monoServiceTwoReference;
    private MonoServiceOne monoServiceOne;
    private MonoServiceTwo monoServiceTwo;
    private bool initialized;
    // add more services to initialize here
    public async void Awake() {
        GameObject simpleStartGameObject = new ("SimpleStartExampleTwoHolder");
        simpleStartGameObject.transform.SetParent(transform.parent);
        monoServiceOne = simpleStartGameObject.AddComponent<MonoServiceOne>();

        // Addressables: instantiate the prefab under the holder
        var handle = monoServiceTwoReference.InstantiateAsync(simpleStartGameObject.transform);
        await handle.Task;
        
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load/instantiate MonoServiceTwo from Addressables.");
            return;
        }

        // Grab the component from the spawned prefab
        var go = handle.Result;
        monoServiceTwo = go.GetComponent<MonoServiceTwo>();
        if (monoServiceTwo == null)
        {
            Debug.LogError("Instantiated prefab does not have MonoServiceTwo component.");
            return;
        }
        
        ServiceLocator.Instance.RegisterService(this);
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize() {
        if (initialized) return;
        else initialized = true;

        ServiceLocator.Instance.RegisterService(monoServiceOne);
        ServiceLocator.Instance.RegisterService(monoServiceTwo);
    }
    public void Dispose() {
        ServiceLocator.Instance.RemoveService<MonoServiceOne>();
        ServiceLocator.Instance.RemoveService<MonoServiceTwo>();

        initialized = false;
        if (monoServiceTwo != null)
        {
            Addressables.ReleaseInstance(monoServiceTwo.gameObject);
            monoServiceTwo = null;
        }
    }
}
