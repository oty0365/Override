using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

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

    [Header("ī�޶� ��ü")]
    public Camera playerCam;
    [Header("�i�ư� ���")]
    public GameObject target;
    [Header("ī�޶� �̵� �ӵ�")]
    public float fallowSpeed;
    [Header("ī�޶� �ٿ���� ����")]
    public CameraBoundArea camBound;
    [Range(0f, 200f), Header("ī�޶� ��鸲 ��")]
    public float shakingPower;
    [Header("ī�޶� ��鸲 �ð�")]
    public float shakeTime;
    public float shakeDamping;
    [Range(0.1f, 20),Header("ī�޶��� ũ��")]
    public float cameraSize;

    private void OnValidate()
    {
        if (playerCam != null)
        {
            StartState(CameraState.Zoom);
        }

    }

    [Header("ī�޶� �� �ӵ�")]
    public float cameraZoomSpeed;

    [Header("�⺻��")]
    [SerializeField] private float originFallowSpeed = 2.2f;
    [SerializeField] private float originZoomSpeed = 4f;
    [SerializeField] private float originShakeTime = 1f;
    [SerializeField] private float originDamping = 0.5f;
    [SerializeField] private float originShakingPower = 12f;
    [SerializeField] private float originCamSize = 4.5f;


    private Coroutine _previousFallowCoroutine;
    private Coroutine _previousZoomCoroutine;
    private Coroutine _previousShakeCoroutine;
    private Coroutine _previousQuickMoveCoroutine;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cameraSize = originCamSize;
        fallowSpeed = originFallowSpeed;
        cameraZoomSpeed = originZoomSpeed;
        shakingPower = originShakingPower;

        StartState(CameraState.CinematicFallow);
    }
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetShake(0.3f, 10, 0.2f);
        }
    }*/

    public void SetFallow(int mode,float speed)
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

    public void SetZoom(float speed)
    {
        cameraZoomSpeed = speed;
        StartState(CameraState.Zoom);
    }

    public void SetShake(float time,float amount,float damping) 
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
                if (_previousFallowCoroutine != null)
                {
                    StopCoroutine(_previousFallowCoroutine);
                }
                _previousFallowCoroutine = StartCoroutine(FallowFlow(0));
                break;
            case CameraState.CinematicFallow:
                if (_previousFallowCoroutine != null)
                {
                    StopCoroutine(_previousFallowCoroutine);
                }
                _previousFallowCoroutine = StartCoroutine(FallowFlow(1));
                break;
            case CameraState.Zoom:
                if (_previousZoomCoroutine != null)
                {
                    StopCoroutine(_previousZoomCoroutine);
                }
                _previousZoomCoroutine = StartCoroutine(ZoomFlow());
                break;
            case CameraState.Shake:
                if (_previousShakeCoroutine != null)
                {
                    StopCoroutine(_previousShakeCoroutine);
                }
                _previousShakeCoroutine = StartCoroutine(ShakeFlow());
                break;
        }
    }
    public void StopState(CameraState cmState)
    {
        switch (cmState)
        {
            case CameraState.MathFallow:
                if (_previousFallowCoroutine != null)
                {
                    fallowSpeed = originFallowSpeed;
                    StopCoroutine(_previousFallowCoroutine);
                }
                break;
            case CameraState.CinematicFallow:
                if (_previousFallowCoroutine != null)
                {
                    fallowSpeed = originFallowSpeed;
                    StopCoroutine(_previousFallowCoroutine);
                }
                break;
            case CameraState.Zoom:
                if (_previousZoomCoroutine != null)
                {
                    cameraZoomSpeed = originZoomSpeed;
                    StopCoroutine(_previousZoomCoroutine);
                }
                break;
            case CameraState.Shake:
                if (_previousShakeCoroutine != null)
                {
                    shakingPower = originShakingPower;
                    shakeDamping = originDamping;
                    shakeTime = originShakeTime;
                    StopCoroutine(_previousShakeCoroutine);
                }
                break;
        }
    }

    private IEnumerator FallowFlow(int mode)
    {
        while (true)
        {
            Vector2 newPos;
            if(mode == 0)
            {
                newPos = Vector2.MoveTowards(playerCam.transform.position, target.transform.position, fallowSpeed * Time.deltaTime);
            }
            else
            {
                newPos = Vector2.Lerp(playerCam.transform.position, target.transform.position, fallowSpeed * Time.deltaTime);
            }

            float halfHeight = playerCam.orthographicSize;
            float halfWidth = halfHeight * playerCam.aspect;

            float minX = camBound.transform.position.x - camBound.size.x / 2 + halfWidth;
            float maxX = camBound.transform.position.x + camBound.size.x / 2 - halfWidth;
            float minY = camBound.transform.position.y - camBound.size.y / 2 + halfHeight;
            float maxY = camBound.transform.position.y + camBound.size.y / 2 - halfHeight;

            float clampedX = Mathf.Clamp(newPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(newPos.y, minY, maxY);

            playerCam.transform.position = new Vector3(clampedX, clampedY, -10);
            yield return null;
        }
    }

    private IEnumerator ZoomFlow()
    {
        while (Mathf.Abs(playerCam.orthographicSize-cameraSize)>0.05f)
        {
            playerCam.orthographicSize = Mathf.Lerp(playerCam.orthographicSize, cameraSize,Time.deltaTime*cameraZoomSpeed);
            yield return null;
        }
        playerCam.orthographicSize = cameraSize;

    }

    private IEnumerator ShakeFlow()
    {
        Vector3 originalPos = playerCam.transform.position;
        float elapsed = 0f;

        float halfHeight = playerCam.orthographicSize;
        float halfWidth = halfHeight * playerCam.aspect;

        float minX = camBound.transform.position.x - camBound.size.x / 2 + halfWidth;
        float maxX = camBound.transform.position.x + camBound.size.x / 2 - halfWidth;
        float minY = camBound.transform.position.y - camBound.size.y / 2 + halfHeight;
        float maxY = camBound.transform.position.y + camBound.size.y / 2 - halfHeight;

        while (elapsed < shakeTime)
        {
            float dampingFactor = 1f - (elapsed / shakeTime);
            float strength = shakingPower * dampingFactor * shakeDamping;

            float offsetX = Random.Range(-1f, 1f) * strength * 0.1f;
            float offsetY = Random.Range(-1f, 1f) * strength * 0.1f;

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
            originalPos.z);
    }

}
