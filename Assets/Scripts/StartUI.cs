using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class StartUI : MonoBehaviour
{
    [Header("References")]
    public GameObject OverlayRoot;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI PromptText;
    public Button StartButton;
    public TextMeshProUGUI StartButtonLabel;

    [Header("Colors")]
    public Color OverlayColor = new Color(0.06f, 0.12f, 0.22f, 0.68f);
    public Color CardColor = new Color(0.42f, 0.82f, 1f, 0.95f);
    public Color CardBorderColor = new Color(1f, 0.92f, 0.36f, 1f);
    public Color TitleColor = new Color(1f, 0.96f, 0.42f, 1f);
    public Color PromptColor = new Color(1f, 1f, 1f, 1f);
    public Color ButtonColor = new Color(1f, 0.62f, 0.2f, 1f);
    public Color ButtonTextColor = new Color(0.12f, 0.14f, 0.2f, 1f);

    private CanvasGroup OverlayCanvasGroup;
    private bool HasStarted;

    private void Awake()
    {
        EnsureUI();

        if (StartButton != null)
        {
            StartButton.onClick.RemoveListener(StartGame);
            StartButton.onClick.AddListener(StartGame);
        }

        ShowImmediate();
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        if (HasStarted)
        {
            return;
        }

        HasStarted = true;
        Time.timeScale = 1f;
        HideImmediate();
    }

    private void ShowImmediate()
    {
        if (OverlayRoot != null)
        {
            OverlayRoot.SetActive(true);
        }

        if (OverlayCanvasGroup != null)
        {
            OverlayCanvasGroup.alpha = 1f;
            OverlayCanvasGroup.interactable = true;
            OverlayCanvasGroup.blocksRaycasts = true;
        }
    }

    private void HideImmediate()
    {
        if (OverlayCanvasGroup != null)
        {
            OverlayCanvasGroup.alpha = 0f;
            OverlayCanvasGroup.interactable = false;
            OverlayCanvasGroup.blocksRaycasts = false;
        }

        if (OverlayRoot != null)
        {
            OverlayRoot.SetActive(false);
        }
    }

    private void EnsureUI()
    {
        EnsureEventSystem();

        if (OverlayRoot != null && TitleText != null && StartButton != null)
        {
            OverlayCanvasGroup = OverlayRoot.GetComponent<CanvasGroup>();
            if (OverlayCanvasGroup == null)
            {
                OverlayCanvasGroup = OverlayRoot.AddComponent<CanvasGroup>();
            }

            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        GameObject overlayObject = new GameObject("StartOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        OverlayRoot = overlayObject;
        overlayObject.transform.SetParent(canvas.transform, false);

        RectTransform overlayRect = overlayObject.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayImage = overlayObject.GetComponent<Image>();
        overlayImage.color = OverlayColor;

        OverlayCanvasGroup = overlayObject.GetComponent<CanvasGroup>();

        GameObject cardObject = new GameObject("StartCard", typeof(RectTransform), typeof(Image), typeof(Outline));
        cardObject.transform.SetParent(overlayObject.transform, false);

        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(580f, 360f);
        cardRect.anchoredPosition = Vector2.zero;

        Image cardImage = cardObject.GetComponent<Image>();
        cardImage.color = CardColor;

        Outline cardOutline = cardObject.GetComponent<Outline>();
        cardOutline.effectColor = CardBorderColor;
        cardOutline.effectDistance = new Vector2(5f, -5f);

        TitleText = CreateTMP(
            parent: cardObject.transform,
            objectName: "Title",
            text: "MonkeyJump",
            size: 72,
            color: TitleColor,
            anchorMin: new Vector2(0.5f, 1f),
            anchorMax: new Vector2(0.5f, 1f),
            anchoredPosition: new Vector2(0f, -70f),
            dimensions: new Vector2(520f, 100f),
            alignment: TextAlignmentOptions.Center
        );

        PromptText = CreateTMP(
            parent: cardObject.transform,
            objectName: "Prompt",
            text: "Press start to begin",
            size: 40,
            color: PromptColor,
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            anchoredPosition: new Vector2(0f, 10f),
            dimensions: new Vector2(500f, 90f),
            alignment: TextAlignmentOptions.Center
        );

        GameObject buttonObject = new GameObject("StartButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(cardObject.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.sizeDelta = new Vector2(320f, 90f);
        buttonRect.anchoredPosition = new Vector2(0f, 38f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = ButtonColor;

        StartButton = buttonObject.GetComponent<Button>();

        StartButtonLabel = CreateTMP(
            parent: buttonObject.transform,
            objectName: "Label",
            text: "Jouer",
            size: 42,
            color: ButtonTextColor,
            anchorMin: Vector2.zero,
            anchorMax: Vector2.one,
            anchoredPosition: Vector2.zero,
            dimensions: Vector2.zero,
            alignment: TextAlignmentOptions.Center
        );
    }

    private void EnsureEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem = eventSystemObject.GetComponent<EventSystem>();
        }

        StandaloneInputModule standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (standaloneModule != null)
        {
            Destroy(standaloneModule);
        }

        InputSystemUIInputModule inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (inputSystemModule == null)
        {
            inputSystemModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        BaseInputModule[] modules = eventSystem.GetComponents<BaseInputModule>();
        for (int i = 0; i < modules.Length; i++)
        {
            if (modules[i] != inputSystemModule)
            {
                Destroy(modules[i]);
            }
        }

        eventSystem.SetSelectedGameObject(null);
    }

    private TextMeshProUGUI CreateTMP(
        Transform parent,
        string objectName,
        string text,
        float size,
        Color color,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 dimensions,
        TextAlignmentOptions alignment
    )
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;

        if (dimensions != Vector2.zero)
        {
            rect.sizeDelta = dimensions;
        }
        else
        {
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        TextMeshProUGUI tmp = textObject.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = size * 0.6f;
        tmp.fontSizeMax = size;
        tmp.fontStyle = FontStyles.Bold;

        return tmp;
    }
}
