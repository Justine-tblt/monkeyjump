using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BirdMovement : MonoBehaviour
{
    private CharacterController Controller;
    private Vector3 Velocity;
    private bool Cooldown;
    private Quaternion InitialRotation;
    private bool IsRestarting;
    public string GroundTag = "Ground";
    public LayerMask GroundLayers;
    private bool InvalidGroundTagWarned;

    public float m_FallSpeed;

    [Header("Score UI")]
    public TMP_FontAsset ScoreFont;
    public Color ScoreTextColor = new Color(1f, 0.97f, 0.65f, 1f);
    public Color ScoreBackgroundColor = new Color(0.08f, 0.12f, 0.2f, 0.82f);

    public Animator BirdAnimator;

    public TextMeshProUGUI ScoreText;
    private int Score;

    [Header("Game Over")]
    public GameOverUI GameOverUI;
    private bool IsGameOver;

    [Header("Camera Switch")]
    public CameraScoreSwitcher CameraSwitcher;

    private void OnTriggerEnter(Collider other)
    {
        if (IsGroundCollider(other))
        {
            HandleDeath();
            return;
        }

        if(other.tag == "Score")
        {
            Score++;

            if (CameraSwitcher == null)
            {
                CameraSwitcher = FindFirstObjectByType<CameraScoreSwitcher>();
            }

            if (CameraSwitcher != null)
            {
                CameraSwitcher.OnScoreChanged(Score);
            }
        }

        if(other.tag == "Obstacle")
        {
            HandleDeath();
        }
    }

    private void Start()
    {
        Controller = gameObject.GetComponent<CharacterController>();
        InitialRotation = transform.rotation;
        SetupScoreUI();

        if (CameraSwitcher == null)
        {
            CameraSwitcher = FindFirstObjectByType<CameraScoreSwitcher>();
        }

        if (CameraSwitcher != null)
        {
            CameraSwitcher.OnScoreChanged(Score);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsGroundHit(hit))
        {
            HandleDeath();
        }
    }

    private void Update()
    {
        if (ScoreText != null)
        {
            ScoreText.text = Score.ToString();
        }

        if (IsGameOver)
        {
            return;
        }

        Velocity.y += -m_FallSpeed * Time.deltaTime;

        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            Jump();

        transform.rotation = InitialRotation;

        Controller.Move(Velocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (Cooldown == false)
        {
            Debug.Log("Jump");
            Cooldown = true;
            Velocity.y = 0;
            Velocity.y = Mathf.Sqrt(60);
            if (BirdAnimator != null)
            {
                BirdAnimator.SetBool("Fly", true);
            }

            StartCoroutine(CooldownRefresh());
        }
    }

    private void SetupScoreUI()
    {
        if (ScoreText == null)
        {
            return;
        }

        RectTransform scoreRect = ScoreText.rectTransform;

        GameObject panelObject = new GameObject("ScorePanel", typeof(RectTransform), typeof(Image));
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.SetParent(scoreRect.parent, false);
        panelRect.SetSiblingIndex(scoreRect.GetSiblingIndex());
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(20f, -20f);
        panelRect.sizeDelta = new Vector2(140f, 64f);

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = ScoreBackgroundColor;

        scoreRect.SetParent(panelRect, false);
        scoreRect.anchorMin = Vector2.zero;
        scoreRect.anchorMax = Vector2.one;
        scoreRect.pivot = new Vector2(0.5f, 0.5f);
        scoreRect.offsetMin = new Vector2(12f, 8f);
        scoreRect.offsetMax = new Vector2(-12f, -8f);
        scoreRect.anchoredPosition = Vector2.zero;

        ScoreText.alignment = TextAlignmentOptions.Center;
        ScoreText.enableAutoSizing = true;
        ScoreText.fontSizeMin = 24;
        ScoreText.fontSizeMax = 48;
        ScoreText.fontStyle = FontStyles.Bold;
        ScoreText.color = ScoreTextColor;

        if (ScoreFont != null)
        {
            ScoreText.font = ScoreFont;
        }

        Shadow shadow = ScoreText.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = ScoreText.gameObject.AddComponent<Shadow>();
        }

        shadow.effectColor = new Color(0f, 0f, 0f, 0.75f);
        shadow.effectDistance = new Vector2(2f, -2f);
    }

    private IEnumerator CooldownRefresh()
    {
        yield return new WaitForSeconds(0.3f);
        Cooldown = false;
        if (BirdAnimator != null)
        {
            BirdAnimator.SetBool("Fly", false);
        }
    }

    private void HandleDeath()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        Cooldown = true;

        if (BirdAnimator != null)
        {
            BirdAnimator.SetBool("Fly", false);
        }

        if (GameOverUI == null)
        {
            GameOverUI = FindFirstObjectByType<GameOverUI>();
        }

        if (GameOverUI != null)
        {
            GameOverUI.ShowGameOver(Score);
            return;
        }

        // Fallback pour eviter un blocage si le panel n'est pas configure.
        RestartGame();
    }

    private bool IsGroundCollider(Collider colliderToCheck)
    {
        if (colliderToCheck == null)
        {
            return false;
        }

        bool inGroundLayer = (GroundLayers.value & (1 << colliderToCheck.gameObject.layer)) != 0;
        if (inGroundLayer)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(GroundTag))
        {
            return false;
        }

        try
        {
            return colliderToCheck.CompareTag(GroundTag);
        }
        catch (UnityException)
        {
            if (!InvalidGroundTagWarned)
            {
                Debug.LogWarning("GroundTag n'existe pas. Assigne un tag valide ou utilise GroundLayers dans BirdMovement.");
                InvalidGroundTagWarned = true;
            }

            return false;
        }
    }

    private bool IsGroundHit(ControllerColliderHit hit)
    {
        if (hit == null || hit.collider == null)
        {
            return false;
        }

        if (IsGroundCollider(hit.collider))
        {
            return true;
        }

        return hit.normal.y > 0.5f;
    }

    public void RestartGame()
    {
        if (IsRestarting)
        {
            return;
        }

        IsRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
