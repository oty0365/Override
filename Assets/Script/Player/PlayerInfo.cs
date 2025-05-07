using UnityEditor.Localization.Plugins.Google.Columns;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance { get; private set; }

    [Header("플레이어 정보")]
    public WeaponData playerWeaponData;
    public SpriteRenderer weaponCore;
    private GameObject _currentOverridingObject;
    public OverrideablesData currentCharacterData;
    public CharacterSkillData currentSkillData;

    [Header("플레이어 스테이터스")]
    public Slider hpBar;
    public Slider staminaBar;
    public Image defenceBar;

    [System.NonSerialized] public float playerMaxHp = 30f;
    private float _playerCurHp;
    public float PlayerCurHp
    {
        get => _playerCurHp;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _playerCurHp = value;
            hpBar.value = _playerCurHp;
            
        }
    }
    [System.NonSerialized] public float playerMaxStamina = 30f;
    private float _playerCurStamina;
    public float PlayerCurStamina
    {
        get => _playerCurStamina;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _playerCurStamina = value;
            staminaBar.value = _playerCurStamina;
        }
    }
    [System.NonSerialized] public float playerBasicSkillCooldown = 1f;
    private float _playerSkillCooldown;
    public float PlayerSkillCooldown
    {
        get => _playerSkillCooldown;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _playerSkillCooldown = value;
        }
    }
    private float _playerDefence;
    public float PlayerDefence
    {
        get => _playerDefence;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _playerDefence = value;
            defenceBar.fillAmount = _playerDefence / 100f;
        }
    }

    public void InitializeStatus()
    {
        hpBar.maxValue = playerMaxHp;
        staminaBar.maxValue = playerMaxStamina;
        defenceBar.fillAmount = PlayerDefence / 100f;
        PlayerCurHp = playerMaxHp;
        PlayerCurStamina = playerMaxStamina;
    }

    public GameObject CurrentOverridingObject
    {
        get => _currentOverridingObject;
        set
        {
            if (_currentOverridingObject != null)
            {
                Instantiate(_currentOverridingObject,gameObject.transform.position,Quaternion.identity);
            }
            Debug.Log(_currentOverridingObject);
            _currentOverridingObject = value;
            var over = _currentOverridingObject.GetComponent<Overrideables>();
            currentCharacterData = over.characterData;
            currentSkillData = over.characterSkillData;
            SkillManager.Instance.ChangeCharacterSkill();
            SkillManager.Instance.BreakSkillCooldown(0);
            SkillManager.Instance.BreakSkillCooldown(1);

        }
    }
    private WeaponCode _playerWeapon;
    public WeaponCode PlayerWeapon
    {
        get => _playerWeapon;
        set
        {
            _playerWeapon = value;
            playerWeaponData = WeaponManager.Instance.weaponDict[value];
            weaponCore.sprite = playerWeaponData.weaponSprite;
            WeaponCore.Instance.ChangeWeapon(playerWeaponData.weaponPrefab);
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        InitializeStatus();
    }
    void Update()
    {
        
    }
}
