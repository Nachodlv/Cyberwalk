﻿using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        panel.SetActive(!panel.activeSelf);
        Time.timeScale = panel.activeSelf ? 0 : 1;
    }
}
