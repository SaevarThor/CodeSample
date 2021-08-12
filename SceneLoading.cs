using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public static event Action OnLoadingComplete;
    public static event Action OnReLoadingComplete;
    public static event Action OnBackToHubComplete; 
    private static Transform _roomConnectionPoint; 
    private static int curSceneIndex = 3; 
    private static int _reloadSceneIndex; 
    private static bool _isLoading; 
    private static bool _isUnReloading; 
    private static bool _waitingForUnload; 
    private static bool _isReloading; 
    private static int _hubworldIndex = 2; 
    private static AsyncOperation _loadScene; 
    private static AsyncOperation _reLoadScene; 
    private static AsyncOperation _reUnLoadScene; 
    [SerializeField] private Vector3 _hubSpawn; 

    private static bool _backToHub; 

    private void Start() 
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);     
    }

    public static void LoadScene(Transform roomConnection = null)
    {
        _loadScene = SceneManager.LoadSceneAsync(curSceneIndex, LoadSceneMode.Additive);  
        _roomConnectionPoint = roomConnection; 
        _isLoading = true; 

        if (_loadScene.isDone)
            OnLoadingComplete?.Invoke();
    }

    public static void LoadSceneIndex(int index)
    {
        SceneManager.UnloadSceneAsync(index -1); 
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive); 
    }

    public static void UnLoadScene(int sceneIndex)
    {
        SceneManager.UnloadSceneAsync(sceneIndex); 
    }

    public static void BackToHubWorld()
    {
        SceneManager.UnloadSceneAsync(curSceneIndex); 
        _loadScene = SceneManager.LoadSceneAsync(_hubworldIndex, LoadSceneMode.Additive); 
        curSceneIndex++; 
        _backToHub = true; 
    }

    public static void StartReloadScene(int SceneIndex)
    {
        _reloadSceneIndex = SceneIndex; 
        _reUnLoadScene = SceneManager.UnloadSceneAsync(SceneIndex); 
        _waitingForUnload = true;
    }

    public static void EndReload(int SceneIndex)
    {
        _reLoadScene = SceneManager.LoadSceneAsync(SceneIndex, LoadSceneMode.Additive); 
        _isReloading = true; 
    }

    private void Update() 
    {
        if (_isLoading)
        {
            if (_loadScene.isDone)
            {
                OnLoadingComplete?.Invoke();    
                _isLoading = false; 
            }
        }

        if (_waitingForUnload)
        {
            if (_reUnLoadScene == null) return; 
            if (_reUnLoadScene.isDone)
            {
                Debug.Log("[SceneLoading] EndReload"); 
                EndReload(_reloadSceneIndex); 
                _waitingForUnload = false; 
            }
        }

        if (_isReloading)
        {
            if (_reLoadScene == null) return; 
            if (_reLoadScene.isDone)
            {
                Debug.Log("[SceneLoading] OnReLoading Invoked"); 
                OnReLoadingComplete?.Invoke();
                _isReloading = false;
            }
        }

        if (_backToHub)
        {
            if (_loadScene.isDone)
            {
                OnBackToHubComplete?.Invoke();
                _backToHub = false; 
            }
        }
    }

    public static Transform GetConnection()
    {
        if (_roomConnectionPoint == null)
            Debug.Log("[SceneLoading] Attempting to get room connection but the var is null"); 

        return _roomConnectionPoint; 
    }
}