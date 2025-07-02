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
            // null 체크 추가
            if (instantinateModule.attackObj == null)
            {
                Debug.LogError($"InstantinateModule 또는 attackObj가 null입니다. GameObject: {gameObject.name}");
                return;
            }

            aPoolingObject = instantinateModule.attackObj.GetComponent<APoolingObject>();
            if (aPoolingObject == null)
            {
                Debug.LogError($"APoolingObject 컴포넌트를 찾을 수 없습니다. GameObject: {instantinateModule.attackObj.name}");
                return;
            }

            // PlayerInfo 싱글톤 체크
            if (PlayerInfo.Instance == null)
            {
                Debug.LogError("PlayerInfo.Instance가 null입니다.");
                return;
            }

            playerInfo = PlayerInfo.Instance;
            UpdateSkill();
            isInitialized = true;

            Debug.Log($"GodSlimesBleastSkill 초기화 완료: {gameObject.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GodSlimesBleastSkill 초기화 중 오류: {e.Message}");
        }
    }

    public void Update()
    {
        // 초기화가 완료되지 않았으면 다시 시도
        if (!isInitialized)
        {
            InitializeSkill();
            return;
        }

        // 모든 필수 컴포넌트 체크
        if (!CanUseSkill()) return;

        // 입력 체크
        if (Input.GetKeyDown(_currentKey))
        {
            Debug.Log("스킬 키 입력 감지됨");
            UseSkill();
        }
    }

    private bool CanUseSkill()
    {
        // 현재 실드가 있으면 사용 불가
        if (currentShield != null)
        {
            return false;
        }

        // SkillManager 체크
        if (SkillManager.Instance == null)
        {
            Debug.LogWarning("SkillManager.Instance가 null입니다.");
            return false;
        }

        // 스킬 사용 가능 여부 체크
        if (!SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            return false;
        }

        // PlayerInfo 체크
        if (playerInfo == null || playerInfo.gameObject == null)
        {
            Debug.LogWarning("PlayerInfo 또는 PlayerInfo.gameObject가 null입니다.");
            return false;
        }

        return true;
    }

    public override void UseSkill()
    {
        try
        {
            Debug.Log("GodSlimesBleast 스킬 사용 시도");

            // ObjectPooler 체크
            if (ObjectPooler.Instance == null)
            {
                Debug.LogError("ObjectPooler.Instance가 null입니다.");
                return;
            }

            if (aPoolingObject == null)
            {
                Debug.LogError("aPoolingObject가 null입니다.");
                return;
            }

            // 오브젝트 생성
            var spawnPos = playerInfo.gameObject.transform.position;
            var o = ObjectPooler.Instance.Get(aPoolingObject, spawnPos, Vector3.zero);

            if (o == null)
            {
                Debug.LogError("ObjectPooler에서 오브젝트를 가져올 수 없습니다.");
                return;
            }

            // GodSlimesBleast 컴포넌트 설정
            var bleastComponent = o.GetComponent<GodSlimesBleast>();
            if (bleastComponent != null)
            {
                bleastComponent.manager = this;
            }
            else
            {
                Debug.LogWarning("생성된 오브젝트에서 GodSlimesBleast 컴포넌트를 찾을 수 없습니다.");
            }

            // 현재 실드 설정
            currentShield = o.GetComponent<APoolingObject>();

            // 쿨다운 시작
            SkillManager.Instance.StartSkillCooldown(skillForm);

            Debug.Log("GodSlimesBleast 스킬 사용 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UseSkill 실행 중 오류: {e.Message}");
        }
    }

    // 외부에서 실드 제거 시 호출할 수 있는 메서드
    public void ClearCurrentShield()
    {
        if (currentShield != null)
        {
            ObjectPooler.Instance.Return(currentShield);
            currentShield = null;
            Debug.Log("현재 실드가 제거되었습니다.");
        }
    }

    private void OnDestroy()
    {
        ClearCurrentShield();
    }

    // 디버그용 - Inspector에서 확인 가능
    private void OnValidate()
    {
        if (instantinateModule.attackObj == null)
        {
            Debug.LogWarning($"InstantinateModule이 할당되지 않았습니다. GameObject: {gameObject.name}");
        }
    }
}