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
        
    }
    void Update()
    {
        
    }
}
