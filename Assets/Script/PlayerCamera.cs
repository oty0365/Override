using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum CameraState
{
    MathFallow,
    CinematicFallow,
    Shake,
    Zoom,
}

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    [Header("카메라 구성")]
    public Camera playerCam;
    public PixelPerfectCamera ppc;

    [Header("타겟 & 움직임")]
    public GameObject target;
    [Range(0.1f, 10f)] public float fallowSpeed;

    [Header("바운더리")]
    public CameraBoundArea camBound;

    [Header("흔들림")]
    [Range(0f, 200f)] public float shakingPower;
    public float shakeTime;
    public float shakeDamping;

    [Header("줌 & 해상도")]
    public float cameraSize;
    public int ppcSize;
    public float cameraZoomSpeed;
    [Range(0, 45)] public int pixelPerfectSize;

    [Header("기본값")]
    [SerializeField] private float originFallowSpeed = 2.2f;
    [SerializeField] private float originZoomSpeed = 4f;
    [SerializeField] private float originShakeTime = 1f;
    [SerializeField] private float originDamping = 0.5f;
    [SerializeField] private float originShakingPower = 12f;
    [SerializeField] private float originCameraSize = 4.5f;
    [SerializeField] private int originPpcSize = 20;

    private Coroutine _previousFallowCoroutine;
    private Coroutine _previousZoomCoroutine;
    private Coroutine _previousShakeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cameraSize = originCameraSize;
        ppcSize = originPpcSize;
        pixelPerfectSize = originPpcSize;
        fallowSpeed = originFallowSpeed;
        cameraZoomSpeed = originZoomSpeed;
        shakingPower = originShakingPower;

        StartState(CameraState.CinematicFallow);
    }

    private void OnValidate()
    {
        int min = 0;
        int max = 45;
        int step = 5;

        int nearest = Mathf.RoundToInt((pixelPerfectSize - min) / (float)step) * step + min;
        pixelPerfectSize = Mathf.Clamp(nearest, min, max);
    }

    public void SetFallow(int mode, float speed)
    {
        fallowSpeed = speed;

        if (mode == 0)
        {
            StartState(CameraState.MathFallow);
        }
        else
        {
            StartState(CameraState.CinematicFallow);
        }
    }

    public void SetZoom(int size, float speed)
    {
        cameraSize = originCameraSize * originPpcSize / size;
        ppcSize = size;
        cameraZoomSpeed = speed;
        ppc.enabled = false;
        StartState(CameraState.Zoom);
    }

    public void SetShake(float time, float amount, float damping)
    {
        shakeTime = time;
        shakingPower = amount;
        shakeDamping = damping;
        StartState(CameraState.Shake);
    }

    public void StartState(CameraState cmState)
    {
        switch (cmState)
        {
            case CameraState.MathFallow:
            case CameraState.CinematicFallow:
                {
                    if (_previousFallowCoroutine != null)
                    {
                        StopCoroutine(_previousFallowCoroutine);
                    }

                    int mode = (cmState == CameraState.MathFallow) ? 0 : 1;
                    _previousFallowCoroutine = StartCoroutine(FallowFlow(mode));
                    break;
                }

            case CameraState.Zoom:
                {
                    if (_previousZoomCoroutine != null)
                    {
                        StopCoroutine(_previousZoomCoroutine);
                    }

                    _previousZoomCoroutine = StartCoroutine(ZoomFlow());
                    break;
                }

            case CameraState.Shake:
                {
                    if (_previousShakeCoroutine != null)
                    {
                        StopCoroutine(_previousShakeCoroutine);
                    }

                    _previousShakeCoroutine = StartCoroutine(ShakeFlow());
                    break;
                }
        }
    }

    public void StopState(CameraState cmState)
    {
        switch (cmState)
        {
            case CameraState.MathFallow:
            case CameraState.CinematicFallow:
                {
                    if (_previousFallowCoroutine != null)
                    {
                        StopCoroutine(_previousFallowCoroutine);
                    }

                    fallowSpeed = originFallowSpeed;
                    break;
                }

            case CameraState.Zoom:
                {
                    if (_previousZoomCoroutine != null)
                    {
                        StopCoroutine(_previousZoomCoroutine);
                    }

                    cameraZoomSpeed = originZoomSpeed;
                    SetPpc();
                    break;
                }

            case CameraState.Shake:
                {
                    if (_previousShakeCoroutine != null)
                    {
                        StopCoroutine(_previousShakeCoroutine);
                    }

                    shakingPower = originShakingPower;
                    shakeDamping = originDamping;
                    shakeTime = originShakeTime;
                    break;
                }
        }
    }

    private IEnumerator FallowFlow(int mode)
    {
        while (true)
        {
            Vector2 targetPos;

            if (mode == 0)
            {
                targetPos = Vector2.MoveTowards(
                    playerCam.transform.position,
                    target.transform.position,
                    fallowSpeed * Time.deltaTime
                );
            }
            else
            {
                targetPos = Vector2.Lerp(
                    playerCam.transform.position,
                    target.transform.position,
                    fallowSpeed * Time.deltaTime
                );
            }

            float halfHeight = playerCam.orthographicSize;
            float halfWidth = halfHeight * playerCam.aspect;

            float minX = camBound.transform.position.x - camBound.size.x / 2 + halfWidth;
            float maxX = camBound.transform.position.x + camBound.size.x / 2 - halfWidth;
            float minY = camBound.transform.position.y - camBound.size.y / 2 + halfHeight;
            float maxY = camBound.transform.position.y + camBound.size.y / 2 - halfHeight;

            float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

            playerCam.transform.position = new Vector3(clampedX, clampedY, -10);
            yield return null;
        }
    }

    private IEnumerator ZoomFlow()
    {
        while (Mathf.Abs(playerCam.orthographicSize - cameraSize) > 0.05f)
        {
            playerCam.orthographicSize = Mathf.Lerp(
                playerCam.orthographicSize,
                cameraSize,
                Time.deltaTime * cameraZoomSpeed
            );
            yield return null;
        }

        playerCam.orthographicSize = cameraSize;
        SetPpc();
    }

    private void SetPpc()
    {
        ppc.assetsPPU = ppcSize;
        ppc.enabled = true;
    }

    private IEnumerator ShakeFlow()
    {
        Vector3 originalPos = playerCam.transform.position;
        float elapsed = 0f;

        float halfHeight = playerCam.orthographicSize;
        float halfWidth = halfHeight * playerCam.aspect;

        float boundHalfWidth = camBound.size.x / 2;
        float boundHalfHeight = camBound.size.y / 2;

        float minX = camBound.transform.position.x - boundHalfWidth + halfWidth;
        float maxX = camBound.transform.position.x + boundHalfWidth - halfWidth;
        float minY = camBound.transform.position.y - boundHalfHeight + halfHeight;
        float maxY = camBound.transform.position.y + boundHalfHeight - halfHeight;

        bool isTooWide = halfWidth * 2f > camBound.size.x;
        bool isTooTall = halfHeight * 2f > camBound.size.y;

        while (elapsed < shakeTime)
        {
            float dampingFactor = 1f - (elapsed / shakeTime);
            float strength = shakingPower * dampingFactor * shakeDamping;

            float offsetX = 0f;
            float offsetY = 0f;

            if (!isTooWide)
            {
                offsetX = Random.Range(-1f, 1f) * strength * 0.1f;
            }

            if (!isTooTall)
            {
                offsetY = Random.Range(-1f, 1f) * strength * 0.1f;
            }

            Vector3 targetPos = originalPos + new Vector3(offsetX, offsetY, 0f);

            float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

            playerCam.transform.position = new Vector3(clampedX, clampedY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCam.transform.position = new Vector3(
            Mathf.Clamp(originalPos.x, minX, maxX),
            Mathf.Clamp(originalPos.y, minY, maxY),
            originalPos.z
        );
    }
}
