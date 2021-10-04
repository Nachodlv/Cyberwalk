using System;
using TMPro;
using UnityEngine;

public class MetersRanText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = $"{(int) GameMode.Singleton.MetersWalked} M";
    }
}
