using UnityEngine;

public class CameraScoreSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera CameraA;
    public Camera CameraB;

    [Header("Decor Switch")]
    public Transform DecorRoot;
    public bool UseCustomDecorXForEachCamera;
    public float DecorXForCameraA;
    public float DecorXForCameraB;
    public float MirrorPivotX = 0f;

    [Header("Rules")]
    public int PointsPerSwitch = 10;
    public bool StartWithCameraA = true;

    private int LastMilestone = -1;
    private float InitialDecorX;
    private bool HasInitialDecorX;

    private void Start()
    {
        CaptureInitialDecorX();
        ApplyForScore(0);
    }

    public void OnScoreChanged(int score)
    {
        ApplyForScore(score);
    }

    private void ApplyForScore(int score)
    {
        if (PointsPerSwitch <= 0)
        {
            PointsPerSwitch = 10;
        }

        int milestone = Mathf.FloorToInt(score / (float)PointsPerSwitch);
        if (milestone == LastMilestone)
        {
            return;
        }

        LastMilestone = milestone;

        bool useCameraA = milestone % 2 == 0;
        if (!StartWithCameraA)
        {
            useCameraA = !useCameraA;
        }

        SetCameraActive(CameraA, useCameraA);
        SetCameraActive(CameraB, !useCameraA);
        ApplyDecorSide(useCameraA);
    }

    private void SetCameraActive(Camera cameraToToggle, bool isActive)
    {
        if (cameraToToggle == null)
        {
            return;
        }

        cameraToToggle.enabled = isActive;

        AudioListener listener = cameraToToggle.GetComponent<AudioListener>();
        if (listener != null)
        {
            listener.enabled = isActive;
        }
    }

    private void CaptureInitialDecorX()
    {
        if (DecorRoot == null || HasInitialDecorX)
        {
            return;
        }

        InitialDecorX = DecorRoot.position.x;
        HasInitialDecorX = true;
    }

    private void ApplyDecorSide(bool useCameraA)
    {
        if (DecorRoot == null)
        {
            return;
        }

        CaptureInitialDecorX();

        float targetX;
        if (UseCustomDecorXForEachCamera)
        {
            targetX = useCameraA ? DecorXForCameraA : DecorXForCameraB;
        }
        else
        {
            float cameraAX = InitialDecorX;
            float cameraBX = 2f * MirrorPivotX - InitialDecorX;
            targetX = useCameraA ? cameraAX : cameraBX;
        }

        Vector3 decorPosition = DecorRoot.position;
        decorPosition.x = targetX;
        DecorRoot.position = decorPosition;
    }
}
