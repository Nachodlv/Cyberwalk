﻿using UnityEngine;

public class GameMode : MonoBehaviour
{
    public static GameMode Singleton
    {
        get
        {
            if (!_singleton)
            {
                GameObject gameObject = new GameObject("GameMode");
                _singleton = gameObject.AddComponent<GameMode>();
                _singleton.Initialize();
            }

            return _singleton;
        }
    }

    private static GameMode _singleton;

    private float _startingX;

    public GameObject PlayerCached { get; private set; }
    public Transform PlayerTransformCached { get; private set; }
    public float MetersWalked => PlayerTransformCached.position.x - _startingX ;

    private void Awake()
    {
        if (_singleton)
        {
            Destroy(gameObject);
            return;
        }
        _singleton = this;
        Initialize();
    }

    void Initialize()
    {
        PlayerCached = GameObject.FindGameObjectWithTag("Player");
        PlayerTransformCached = PlayerCached.transform;
        _startingX = PlayerCached.transform.position.x;
    }
}
