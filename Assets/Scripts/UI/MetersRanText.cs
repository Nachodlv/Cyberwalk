using System;
using TMPro;
using UnityEngine;

public class MetersRanText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        text.text = "0 m";
    }

    private void Update()
    {
        text.text = $"{(int) GameMode.Singleton.MetersWalked} m";
    }
}
