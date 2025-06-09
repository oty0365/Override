using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.Google.Columns;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

public interface Buff
{
    public void UseBuff();
}

public class PlayerInfo : HalfSingleMono<PlayerInfo>
{

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
    public Slider codeBar;
    public TextMeshProUGUI hpRange;
    public TextMeshProUGUI codeRange;
    public TextMeshProUGUI staminaRange;
    public TextMeshProUGUI coinText;
    public Image defenceBar;

    private int _playerCoin;
    public int PlayerCoin
    {
        get => _playerCoin;
        set
        {
            if (value < 0)
            {
                _playerCoin = 0;
            }
            else
            {
                _playerCoin = value;
            }
                coinText.text = _playerCoin.ToString();
        }
    }
    [System.NonSerialized] private float playerMaxCodePower = 30f;
    public float PlayerMaxCodePower
    {
        get => playerMaxCodePower;
        set
        {
            codeBar.maxValue = value;
            playerMaxHp = value;
        }
    }
    private float _playerCurCodePower;
    public float PlayerCurCodePower
    {
        get => _playerCurCodePower;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value > playerMaxCodePower)
            {
                _playerCurCodePower = playerMaxHp;
            }
            else
            {
                _playerCurCodePower = value;
            }
            codeRange.text = _playerCurCodePower.ToString() + "/" + playerMaxCodePower.ToString();
            codeBar.value = _playerCurCodePower;

        }
    }

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
                _playerCurHp = playerMaxHp;
            }
            else
            {
                _playerCurHp = value;
            }
            hpRange.text = _playerCurHp.ToString() + "/" + playerMaxHp.ToString();
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
        PlayerCoin = 0;
        hpBar.maxValue = playerMaxHp;
        codeBar.maxValue = playerMaxCodePower;
        staminaBar.maxValue = playerMaxStamina;
        defenceBar.fillAmount = PlayerDefence / 100f;
        PlayerCurHp = playerMaxHp;
        PlayerCurCodePower = 0f;
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
            SkillManager.Instance.BreakSkillCooldown(2);

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
    public Stack<Buff> shiledBuff = new();
    void Start()
    {
        InitializeStatus();

    }
    /*private void Update()
    {
        Debug.Log(shiledBuff.Count);
    }*/

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
