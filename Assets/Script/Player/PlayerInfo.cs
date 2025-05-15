using System.Collections;
using TMPro;
using UnityEditor.Localization.Plugins.Google.Columns;
using UnityEditor.Localization.Plugins.XLIFF.V20;
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
    public Collider2D playerCollider;
    public LayerMask damageAbles;
    public bool isInfinite;
    private Coroutine _infinityCoroutine;

    [Header("플레이어 스테이터스")]
    public Slider hpBar;
    public Slider staminaBar;
    public TextMeshProUGUI hpRange;
    public TextMeshProUGUI staminaRange;
    public Image defenceBar;

    [System.NonSerialized] private float playerMaxHp = 30f;
    public float PlayerMaxHp
    {
        get => playerMaxHp;
        set
        {
            hpBar.maxValue= value;
            playerMaxHp = value;
        }
    }
    private float _playerCurHp;
    private Coroutine _playerHpCoroutine;
    public float PlayerCurHp
    {
        get => _playerCurHp;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value > playerMaxHp)
            {
                playerMaxHp = value;
            }
            _playerCurHp = value;
            hpRange.text = value.ToString() + "/" + playerMaxHp.ToString();
            hpBar.value = _playerCurHp;
            
        }
    }
    [System.NonSerialized] public float playerMaxStamina = 30f;
    private float _playerCurStamina;
    private Coroutine _playerStaminaCoroutine;
    private Coroutine _playerWaitStaminaCoroutine;
    public float PlayerCurStamina
    {
        get => _playerCurStamina;
        set
        {
            float prev = _playerCurStamina;

            if (value < 0) 
            {
                value = 0;
            }

            if (value > playerMaxStamina)
            {
                value = playerMaxStamina;
            }


            _playerCurStamina = value;

            staminaRange.text = $"{(int)_playerCurStamina}/{playerMaxStamina}";
            staminaBar.value = _playerCurStamina;

            if (_playerCurStamina < playerMaxStamina && value < prev)
            {
                if (_playerStaminaCoroutine != null)
                {
                    StopCoroutine(_playerStaminaCoroutine);
                    _playerStaminaCoroutine = null;
                }
                if (_playerWaitStaminaCoroutine != null)
                {
                    StopCoroutine(_playerWaitStaminaCoroutine);
                    _playerWaitStaminaCoroutine = null;
                }
                _playerWaitStaminaCoroutine = StartCoroutine(StaminaWaitFlow());
            }
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
    [System.NonSerialized] public float playerBasicAttackDamage = 0f;
    private float _playerAttackDamage;
    public float PlayerAttackDamage
    {
        get => _playerAttackDamage;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _playerAttackDamage= value;
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
        _playerSkillCooldown = playerBasicSkillCooldown;
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

    public void SetInfiniteTime(float time)
    {
        if (_infinityCoroutine != null)
        {
            StopCoroutine(_infinityCoroutine);
            RemoveDamageAbleLayers();
        }
        _infinityCoroutine = StartCoroutine(InfiniteTimeFlow(time));
    }

    private IEnumerator InfiniteTimeFlow(float time)
    {
        ApplyDamageAbleLayers();
        isInfinite = true;
        yield return new WaitForSeconds(time);
        isInfinite = false;
        RemoveDamageAbleLayers();
    }

    private void ApplyDamageAbleLayers()
    {
        playerCollider.excludeLayers |= damageAbles;
    }

    private void RemoveDamageAbleLayers()
    {
        playerCollider.excludeLayers &= ~damageAbles;
    }

    private IEnumerator StaminaChargeFlow()
    {
        while (playerMaxStamina>PlayerCurStamina)
        {
            PlayerCurStamina += Time.deltaTime * 10f;
            yield return null;
        }
    }
    private IEnumerator StaminaWaitFlow()
    {
        yield return new WaitForSeconds(1f);
        _playerStaminaCoroutine = StartCoroutine(StaminaChargeFlow());
    }

}
