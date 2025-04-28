using UnityEngine;

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
                var o = Instantiate(_currentOverridingObject,gameObject.transform.position,Quaternion.identity);
                var over = o.GetComponent<Overrideables>();
                currentCharacterData = over.characterData;
                currentSkillData = over.characterSkillData;
                //이후 설정 변경
            }
            Debug.Log(_currentOverridingObject);
            _currentOverridingObject = value;
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
