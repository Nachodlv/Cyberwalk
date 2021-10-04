using System;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [SerializeField] private GameObject onSprite;
    [SerializeField] private GameObject offSprite;

    private bool _audioOn;

    private void Awake()
    {
        _audioOn = AudioListener.volume > 0.01f;
        SwitchSprites();
    }

    public void Toggle()
    {
        if (_audioOn)
        {
            AudioListener.volume = 0.0f;
            _audioOn = false;
        }
        else
        {
            _audioOn = true;
            AudioListener.volume = 1.0f;
        }
        SwitchSprites();
    }

    private void SwitchSprites()
    {
        if (_audioOn)
        {
            onSprite.SetActive(true);
            offSprite.SetActive(false);
        }
        else
        {
            onSprite.SetActive(false);
            offSprite.SetActive(true);
        }
    }

}
