using System.Collections;
using UnityEngine;

public class ToggleInteractable : DistanceInteractable
{
    public GameObject fluidObject;
    public GameObject normalObject;
    public float fadeDuration = 5f;

    protected override void Start()
    {
        base.Start();
        fluidObject.SetActive(false);
        normalObject.SetActive(true);
    }

    public override void OnInteract()
    {
        fluidObject.SetActive(true);
        normalObject.SetActive(false);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Renderer r = GetComponent<Renderer>();
        if (r == null || r.material == null) yield break;

        Material mat = r.material;
        string colorProp = mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";

        if (!mat.HasProperty(colorProp)) yield break;

        Color startColor = mat.GetColor(colorProp);
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, timeElapsed / fadeDuration);
            mat.SetColor(colorProp, new Color(startColor.r, startColor.g, startColor.b, alpha));
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
