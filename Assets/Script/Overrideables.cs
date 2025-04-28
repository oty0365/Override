using UnityEngine;

public class Overrideables : AInteractable
{
    [SerializeField] private OverrideablesData data;
    [SerializeField] private ParticleSystem prt;
    private SpriteRenderer _sr;

    void Start()
    {

        _sr = GetComponent<SpriteRenderer>();
        var main = prt.main;
        switch (data.rarity)
        {
            case Rarity.Common:
                main.startColor = Color.white;
                break;
            case Rarity.Rare:
                main.startColor = Color.green;
                break;
            case Rarity.Epic:
                main.startColor = Color.blue;
                break;
            case Rarity.Legendary:
                main.startColor = Color.yellow;
                break;
            case Rarity.Unknown:
                main.startColor = Color.red;
                break;
        }
        

    }

    void Update()
    {
        
    }
    public override void OnInteract()
    {
        PlayerAnimator.Instance.Override(data.animationClips,gameObject.transform,data.prefabObject);
        Destroy(gameObject);
    }
}
