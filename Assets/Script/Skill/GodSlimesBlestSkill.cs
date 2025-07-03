using UnityEngine;
using System.Collections;

public class GodSlimesBleastSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    private APoolingObject aPoolingObject;
    public APoolingObject currentShield;
    private PlayerInfo playerInfo;
    private bool isInitialized = false;
    private Coroutine initializationCoroutine;

    public void Start()
    {
        if (initializationCoroutine != null)
        {
            StopCoroutine(initializationCoroutine);
        }
        initializationCoroutine = StartCoroutine(InitializeSkillCoroutine());
    }

    private IEnumerator InitializeSkillCoroutine()
    {
        while (PlayerInfo.Instance == null || SkillManager.Instance == null || ObjectPooler.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);
        InitializeSkill();
        initializationCoroutine = null;
    }

    private void InitializeSkill()
    {
        if (instantinateModule.attackObj == null)
        {
            return;
        }

        aPoolingObject = instantinateModule.attackObj.GetComponent<APoolingObject>();
        if (aPoolingObject == null)
        {
            return;
        }

        playerInfo = PlayerInfo.Instance;
        UpdateSkill();
        isInitialized = true;
    }

    public void Update()
    {
        if (!isInitialized) return;

        if (!CanUseSkill()) return;

        if (Input.GetKeyDown(_currentKey))
        {
            UseSkill();
        }
    }

    private bool CanUseSkill()
    {
        if (currentShield != null && currentShield.gameObject.activeInHierarchy)
        {
            return false;
        }

        if (SkillManager.Instance == null)
        {
            return false;
        }

        if (!SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            return false;
        }

        if (playerInfo == null || playerInfo.gameObject == null)
        {
            return false;
        }

        return true;
    }

    public override void UseSkill()
    {
        if (currentShield != null)
        {
            ClearCurrentShield();
        }

        if (ObjectPooler.Instance == null || aPoolingObject == null)
        {
            return;
        }

        var spawnPos = playerInfo.gameObject.transform.position;
        var o = ObjectPooler.Instance.Get(aPoolingObject, spawnPos, Vector3.zero);

        if (o == null)
        {
            return;
        }

        o.gameObject.SetActive(true);
        o.transform.position = spawnPos;

        var bleastComponent = o.GetComponent<GodSlimesBleast>();
        if (bleastComponent != null)
        {
            bleastComponent.manager = this;
            bleastComponent.enabled = true;
        }

        var renderers = o.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }

        var colliders = o.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }

        currentShield = o.GetComponent<APoolingObject>();
        SkillManager.Instance.StartSkillCooldown(skillForm);
        StartCoroutine(MonitorShieldStatus());
    }

    private IEnumerator MonitorShieldStatus()
    {
        while (currentShield != null && currentShield.gameObject.activeInHierarchy)
        {
            if (!currentShield.gameObject.activeInHierarchy)
            {
                currentShield = null;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ClearCurrentShield()
    {
        if (currentShield != null)
        {
            currentShield.gameObject.SetActive(false);
            ObjectPooler.Instance.Return(currentShield);
            currentShield = null;
        }
    }

    private void OnDestroy()
    {
        if (initializationCoroutine != null)
        {
            StopCoroutine(initializationCoroutine);
        }
        StopAllCoroutines();
        ClearCurrentShield();
    }

    private void OnDisable()
    {
        if (initializationCoroutine != null)
        {
            StopCoroutine(initializationCoroutine);
        }
        StopAllCoroutines();
    }
}