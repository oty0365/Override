using TMPro;
using UnityEngine;

public abstract class AInteractable:MonoBehaviour
{
    public GameObject interactionKeyPos;
    public bool autoInteraction;
    public abstract void OnInteract();
    public abstract void ExitInteract();
}

public class PlayerInteraction : HalfSingleMono<PlayerInteraction>
{

    [Header("������")]
    public float interactionRange;
    public LayerMask interactionLayer;
    public GameObject interactionKey;
    public bool isInteracting;
    public bool isCharging;

    private Collider2D[] _hits;
    private GameObject _currentObject;
    private GameObject _prevObject;
    private Vector2 _playerPos;
    private AInteractable _currentObejctInteractable;
    private TextMeshPro _interactText;

    private void Start()
    {
        interactionKey.SetActive(false);
        _interactText = interactionKey.GetComponentInChildren<TextMeshPro>();
        _interactText.text = KeyBindingManager.Instance.keyBindings["Interact"].ToString();
    }

    private void FixedUpdate()
    {
        _playerPos = transform.position;

        _hits = Physics2D.OverlapCircleAll(_playerPos, interactionRange, interactionLayer);

        _currentObject = null;
        float closestDist = float.MaxValue;

        foreach (var hit in _hits)
        {
            float dist = Vector2.Distance(hit.transform.position, _playerPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                _currentObject = hit.gameObject;
            }
        }
        if (_currentObject != null)
        {
            _currentObejctInteractable = _currentObject.GetComponent<AInteractable>();

            if (_currentObject == _prevObject)
            {
                if (_currentObejctInteractable.autoInteraction)
                {
                    interactionKey.SetActive(false);
                }
                else
                {
                    if (!isInteracting)
                    {
                        interactionKey.SetActive(true);
                        interactionKey.transform.position = _currentObejctInteractable.interactionKeyPos.transform.position;
                    }
                    else
                    {
                        interactionKey.SetActive(false);
                    }
                }
            }
            else
            {
                if (_prevObject != null)
                {
                    _prevObject.GetComponent<AInteractable>().ExitInteract();
                }

                if (_currentObejctInteractable.autoInteraction)
                {
                    interactionKey.SetActive(false);
                    _currentObejctInteractable.OnInteract();
                }
                else if (!isInteracting)
                {
                    interactionKey.SetActive(true);
                    interactionKey.transform.position = _currentObejctInteractable.interactionKeyPos.transform.position;
                }
            }
        }

        else
        {
            interactionKey.SetActive(false);
            if (_prevObject != null)
            {
                _prevObject.GetComponent<AInteractable>().ExitInteract();
            }
        }
        _prevObject = _currentObject;
    }

    private void Update()
    {
        if (_currentObject != null)
        {
            if(!_currentObejctInteractable.autoInteraction&&Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Interact"]))
            {

                _currentObejctInteractable.OnInteract();
            }

        }
        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Charge"]) && !isInteracting&&PlayerMove.Instance.canInput)
        {
            PlayerCamera.Instance.SetZoom(3, 6);
            isCharging = true;

        }
        if (isCharging)
        {
            var c = SkillManager.Instance.openCodes;
            OnInteractMode(0);
            foreach(var i in c)
            {
                if (i != null)
                {
                    i.FallowPlayer();
                    i.target = gameObject.transform.position;
                }
            }
        }
        if (Input.GetKeyUp(KeyBindingManager.Instance.keyBindings["Charge"]))
        {
            OnInteractMode(1);
            isCharging = false;
            PlayerCamera.Instance.SetZoom(PlayerCamera.Instance.currentZoomSize, 6);
        }
    }

    public void OnInteractMode(int mode)
    {
        PlayerMove.Instance.canInput = mode != 0;
        isInteracting = mode == 0;
    }
}
