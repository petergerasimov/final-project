using System.Collections;
using UnityEngine;

public class ToggleInteractable : DistanceInteractable
{
    [SerializeField] public GameObject FluidObject;
    [SerializeField] public GameObject NormalObject;
    [SerializeField] public float FadeDuration = 5f;

    protected override void Start()
    {
        base.Start();
        FluidObject.SetActive(false);
        NormalObject.SetActive(true);
    }

    public override void OnInteract()
    {
        FluidObject.SetActive(true);
        NormalObject.SetActive(false);
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

        while (timeElapsed < FadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, timeElapsed / FadeDuration);
            mat.SetColor(colorProp, new Color(startColor.r, startColor.g, startColor.b, alpha));
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
