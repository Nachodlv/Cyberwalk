using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightAnimation : MonoBehaviour
{
    [Header("References")]
    public Material LightMaterial;
    public MeshRenderer Renderer;

    [Header("Glow animation")]
    public float GlowOffset = 0.5f;
    public float Speed = 10.0f;
    public float GlowScale = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        Material MatInstance = Instantiate(LightMaterial);

        Renderer.material = MatInstance;
    }

    // Update is called once per frame
    void Update()
    {
        Color GlowColor = Renderer.sharedMaterial.GetColor("_TintColor");
        GlowColor.a = GlowOffset + ((Mathf.Sin(Time.time * Speed) + 1) / 2) * Time.deltaTime * GlowScale;

        Renderer.sharedMaterial.SetColor("_TintColor", GlowColor);
    }
}
