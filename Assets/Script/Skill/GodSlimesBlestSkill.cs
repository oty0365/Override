using UnityEngine;

public class GodSlimesBleastSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    private APoolingObject aPoolingObject;
    public APoolingObject currentShield;
    private PlayerInfo playerInfo;
    private bool isInitialized = false;

    public void Start()
    {
        InitializeSkill();
    }

    private void InitializeSkill()
    {
        try
        {
            // null üũ �߰�
            if (instantinateModule.attackObj == null)
            {
                Debug.LogError($"InstantinateModule �Ǵ� attackObj�� null�Դϴ�. GameObject: {gameObject.name}");
                return;
            }

            aPoolingObject = instantinateModule.attackObj.GetComponent<APoolingObject>();
            if (aPoolingObject == null)
            {
                Debug.LogError($"APoolingObject ������Ʈ�� ã�� �� �����ϴ�. GameObject: {instantinateModule.attackObj.name}");
                return;
            }

            // PlayerInfo �̱��� üũ
            if (PlayerInfo.Instance == null)
            {
                Debug.LogError("PlayerInfo.Instance�� null�Դϴ�.");
                return;
            }

            playerInfo = PlayerInfo.Instance;
            UpdateSkill();
            isInitialized = true;

            Debug.Log($"GodSlimesBleastSkill �ʱ�ȭ �Ϸ�: {gameObject.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GodSlimesBleastSkill �ʱ�ȭ �� ����: {e.Message}");
        }
    }

    public void Update()
    {
        // �ʱ�ȭ�� �Ϸ���� �ʾ����� �ٽ� �õ�
        if (!isInitialized)
        {
            InitializeSkill();
            return;
        }

        // ��� �ʼ� ������Ʈ üũ
        if (!CanUseSkill()) return;

        // �Է� üũ
        if (Input.GetKeyDown(_currentKey))
        {
            Debug.Log("��ų Ű �Է� ������");
            UseSkill();
        }
    }

    private bool CanUseSkill()
    {
        // ���� �ǵ尡 ������ ��� �Ұ�
        if (currentShield != null)
        {
            return false;
        }

        // SkillManager üũ
        if (SkillManager.Instance == null)
        {
            Debug.LogWarning("SkillManager.Instance�� null�Դϴ�.");
            return false;
        }

        // ��ų ��� ���� ���� üũ
        if (!SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            return false;
        }

        // PlayerInfo üũ
        if (playerInfo == null || playerInfo.gameObject == null)
        {
            Debug.LogWarning("PlayerInfo �Ǵ� PlayerInfo.gameObject�� null�Դϴ�.");
            return false;
        }

        return true;
    }

    public override void UseSkill()
    {
        try
        {
            Debug.Log("GodSlimesBleast ��ų ��� �õ�");

            // ObjectPooler üũ
            if (ObjectPooler.Instance == null)
            {
                Debug.LogError("ObjectPooler.Instance�� null�Դϴ�.");
                return;
            }

            if (aPoolingObject == null)
            {
                Debug.LogError("aPoolingObject�� null�Դϴ�.");
                return;
            }

            // ������Ʈ ����
            var spawnPos = playerInfo.gameObject.transform.position;
            var o = ObjectPooler.Instance.Get(aPoolingObject, spawnPos, Vector3.zero);

            if (o == null)
            {
                Debug.LogError("ObjectPooler���� ������Ʈ�� ������ �� �����ϴ�.");
                return;
            }

            // GodSlimesBleast ������Ʈ ����
            var bleastComponent = o.GetComponent<GodSlimesBleast>();
            if (bleastComponent != null)
            {
                bleastComponent.manager = this;
            }
            else
            {
                Debug.LogWarning("������ ������Ʈ���� GodSlimesBleast ������Ʈ�� ã�� �� �����ϴ�.");
            }

            // ���� �ǵ� ����
            currentShield = o.GetComponent<APoolingObject>();

            // ��ٿ� ����
            SkillManager.Instance.StartSkillCooldown(skillForm);

            Debug.Log("GodSlimesBleast ��ų ��� �Ϸ�");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UseSkill ���� �� ����: {e.Message}");
        }
    }

    // �ܺο��� �ǵ� ���� �� ȣ���� �� �ִ� �޼���
    public void ClearCurrentShield()
    {
        if (currentShield != null)
        {
            ObjectPooler.Instance.Return(currentShield);
            currentShield = null;
            Debug.Log("���� �ǵ尡 ���ŵǾ����ϴ�.");
        }
    }

    private void OnDestroy()
    {
        ClearCurrentShield();
    }

    // ����׿� - Inspector���� Ȯ�� ����
    private void OnValidate()
    {
        if (instantinateModule.attackObj == null)
        {
            Debug.LogWarning($"InstantinateModule�� �Ҵ���� �ʾҽ��ϴ�. GameObject: {gameObject.name}");
        }
    }
}