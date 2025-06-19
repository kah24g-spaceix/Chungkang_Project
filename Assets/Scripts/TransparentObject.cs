using System.Collections;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    private Material material;
    private Coroutine fadeCoroutine;
    public float fadeSpeed = 2f;
    private bool isTransparent = false;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    public void BecomeTransparent()
    {
        if (!isTransparent)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        isTransparent = true;

        SetMaterialToFade();

        Color color = material.color;

        while (color.a > 0.3f)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            material.color = color;
            yield return null;
        }

        color.a = 0.3f;
        material.color = color;
    }

    public void BecomeOpaque()
    {
        if (isTransparent)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        Color color = material.color;

        while (color.a < 1f)
        {
            color.a += Time.deltaTime * fadeSpeed;
            material.color = color;
            yield return null;
        }

        color.a = 1f;
        material.color = color;

        SetMaterialToOpaque();

        isTransparent = false;
    }

    private void SetMaterialToFade()
    {
        material.SetFloat("_Mode", 2);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    private void SetMaterialToOpaque()
    {
        material.SetFloat("_Mode", 0);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }
}
