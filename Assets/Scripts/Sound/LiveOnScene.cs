using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiveOnScene : MonoBehaviour
{
    [SerializeField] private int sceneToLive;

    private static LiveOnScene _instance;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex != sceneToLive)
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            _instance = null;
            Destroy(gameObject);
        }
    }
}
