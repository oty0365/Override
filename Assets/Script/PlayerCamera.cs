using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public enum CameraState
{
    MathFallow,
    CinematicFallow,
    Shake,
    Zoom,
    GetDamage,

}

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    [Header("카메라 구성")]
    public Camera playerCam;

    [Header("타겟 & 움직임")]
    public GameObject target;
    [Range(0.1f, 10f)] public float fallowSpeed;

    [Header("바운더리")]
    public CameraBoundArea camBound;

    [Header("흔들림")]
    [Range(0f, 200f)] public float shakingPower;
    public float shakeTime;
    public float shakeDamping;

    [Header("줌")]
    public float cameraSize;
    public float cameraZoomSpeed;
    public float currentZoomSize;

    [Header("기본값")]
    [SerializeField] private float originFallowSpeed = 2.2f;
    [SerializeField] private float originZoomSpeed = 4f;
    [SerializeField] private float originShakeTime = 1f;
    [SerializeField] private float originDamping = 0.5f;
    [SerializeField] private float originShakingPower = 12f;
    [SerializeField] private float originCameraSize = 4.5f;

    [Header("화면")]
    [SerializeField] private Volume volume;
    private UnityEngine.Rendering.Universal.Vignette _vignette;
    private UnityEngine.Rendering.Universal.ChromaticAberration _aberration;
    [NonSerialized] public UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;
    private float _fadeSpeed;
    private float _fadeAmount;


    private Coroutine _previousFallowCoroutine;
    private Coroutine _previousZoomCoroutine;
    private Coroutine _previousShakeCoroutine;
    private Coroutine _previousGetDamageCoroutine;
    private Coroutine _previousLowHpCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentZoomSize = 4.5f;
        if (volume.profile.TryGet(out UnityEngine.Rendering.Universal.Vignette vignette))
        {
            _vignette = vignette;
        }
        if (volume.profile.TryGet(out UnityEngine.Rendering.Universal.ColorAdjustments colorAdj))
        {
            colorAdjustments = colorAdj;
        }
        if (volume.profile.TryGet(out UnityEngine.Rendering.Universal.ChromaticAberration colorAber))
        {
            _aberration = colorAber;
        }
        cameraSize = originCameraSize;
        fallowSpeed = originFallowSpeed;
        cameraZoomSpeed = originZoomSpeed;
        shakingPower = originShakingPower;

        StartState(CameraState.CinematicFallow);
    }
    public void SetAbHpUi(int i)
    {
        if (_aberration == null)
        {
            return;
        }
        if (i != _aberration.intensity.value)
        {
            if (_previousLowHpCoroutine != null)
            {
                StopCoroutine(_previousLowHpCoroutine);
            }
            _previousLowHpCoroutine =  StartCoroutine(LowHpFlow(i));
            _aberration.intensity.value = i;
        }
    }
    IEnumerator LowHpFlow(float value)
    {
        while(Mathf.Abs(value-_aberration.intensity.value)>0.01f)
        {
            _aberration.intensity.value=Mathf.Lerp(_aberration.intensity.value, value,Time.deltaTime);
            yield return null;
        }
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

    public void SetZoom(float size, float speed)
    {
        currentZoomSize = cameraSize;
        cameraSize = size;
        cameraZoomSpeed = speed;
        StartState(CameraState.Zoom);
    }

    public void SetShake(float time, float amount, float damping)
    {
        shakeTime = time;
        shakingPower = amount;
        shakeDamping = damping;
        StartState(CameraState.Shake);
    }

    public void SetDamge(float speed, float amount)
    {
        _fadeSpeed = speed;
        _fadeAmount = amount;
        StartState(CameraState.GetDamage);
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
                    if (_previousZoomCoroutine == null)
                    {
                        _previousZoomCoroutine = StartCoroutine(ZoomFlow());
                    }
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
            case CameraState.GetDamage:
                {
                    if (_previousGetDamageCoroutine != null)
                    {
                        StopCoroutine(_previousGetDamageCoroutine);
                    }

                    _previousGetDamageCoroutine = StartCoroutine(RedBlinkFlow(_fadeSpeed,_fadeAmount));
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
                    _previousZoomCoroutine = null;
                    cameraZoomSpeed = originZoomSpeed;
                    cameraSize = originCameraSize;
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
            case CameraState.GetDamage:
                {
                    if (_previousGetDamageCoroutine != null)
                    {
                        StopCoroutine(_previousGetDamageCoroutine);
                    }
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
                if (target == null)
                {
                    target = PlayerInfo.Instance.gameObject;
                }
                targetPos = Vector2.Lerp(
                    playerCam.transform.position,
                    (Vector2)target?.transform.position,
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
        while (Mathf.Abs(playerCam.orthographicSize - cameraSize) > 0.02f)
        {
            playerCam.orthographicSize = Mathf.Lerp(playerCam.orthographicSize,cameraSize,Time.deltaTime * cameraZoomSpeed);
            yield return null;
        }

        playerCam.orthographicSize = cameraSize;
        _previousZoomCoroutine = null;
    }

    private IEnumerator RedBlinkFlow(float fadeSpeed,float redBlinkRange)
    {
        for(var i = 0f; i <= redBlinkRange; i += Time.deltaTime * fadeSpeed)
        {
            _vignette.intensity.value = i;
            yield return null;
        }
        for (var i = redBlinkRange; i >= 0; i -= Time.deltaTime * fadeSpeed)
        {
            _vignette.intensity.value = i;
            yield return null;
        }

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

            float offsetX = isTooWide ? 0f : UnityEngine.Random.Range(-1f, 1f) * strength * 0.1f;
            float offsetY = isTooTall ? 0f : UnityEngine.Random.Range(-1f, 1f) * strength * 0.1f;

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
