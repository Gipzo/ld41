using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PalleteSwap : MonoBehaviour
{

    public Texture LookupTexture;
    public Shader PalleteSwapShader;
    [Range(0, 1f)]

    public float Pallete = 0;

    [Range(0, 8)]
    public int CurrentPallete = 0;
    Material _mat;

    void OnEnable()
    {
        Shader shader = PalleteSwapShader;
        if (_mat == null)
            _mat = new Material(shader);
    }

    void OnDisable()
    {
        if (_mat != null)
            DestroyImmediate(_mat);
    }

    public void TogglePallette()
    {
        CurrentPallete = (CurrentPallete + 1) % 7;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        _mat.SetFloat("_CurrentPallete", (CurrentPallete + 1) / 8f);
        _mat.SetTexture("_PaletteTex", LookupTexture);
        Graphics.Blit(src, dst, _mat);
    }
}
