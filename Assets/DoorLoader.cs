using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorLoader : DistanceInteractable
{
    public List<GameObject> Scenes;
    public int CurrentSceneIndex = 0;

    private bool m_isLoading;

    public override void OnInteract()
    {
        if (m_isLoading) return;
        StartCoroutine(LoadNextScene());
    }

    public void ResetPhysX()
    {
        PhysXUtility.ResetPhysX();
    }

    private IEnumerator LoadNextScene()
    {
        if (Scenes == null || Scenes.Count == 0)
        {
            Debug.LogWarning("DoorLoader: scenes list is empty");
            yield break;
        }
        m_isLoading = true;

        GameObject canvasGameObj = new GameObject("LoadingScreenCanvas");
        Canvas canvas = canvasGameObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        canvasGameObj.AddComponent<CanvasScaler>();
        canvasGameObj.AddComponent<GraphicRaycaster>();

        GameObject bgGameObj = new GameObject("Background");
        bgGameObj.transform.SetParent(canvasGameObj.transform, false);
        Image bgImage = bgGameObj.AddComponent<Image>();
        bgImage.color = Color.black;
        RectTransform bgRect = bgImage.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject textGameObj = new GameObject("LoadingText");
        textGameObj.transform.SetParent(canvasGameObj.transform, false);
        Text loadingText = textGameObj.AddComponent<Text>();
        loadingText.text = "Loading...";

        loadingText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loadingText.color = Color.white;
        loadingText.fontSize = 40;
        loadingText.alignment = TextAnchor.LowerRight;

        RectTransform textRect = loadingText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(1, 0);
        textRect.anchorMax = new Vector2(1, 0);
        textRect.pivot = new Vector2(1, 0);
        textRect.anchoredPosition = new Vector2(-50, 50);
        textRect.sizeDelta = new Vector2(400, 100);

        Scenes[CurrentSceneIndex].SetActive(false);
        CurrentSceneIndex = (CurrentSceneIndex + 1) % Scenes.Count;
        Scenes[CurrentSceneIndex].SetActive(true);
        // ResetPhysX();

        Destroy(canvasGameObj);
        m_isLoading = false;
    }
}
