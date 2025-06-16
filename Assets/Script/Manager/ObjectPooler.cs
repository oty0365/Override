using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectType
{
    PlayerAfterImage,
    Particle,
    SoulBullet,
    Monster,
    ThrownDagger,
    CrunchBite,
    BlazeOfEvil,
    HitByLayzer,
    StaminaPoint,
    Dummy,
    Code,
    ForceFromPeople,
    SwordOfVados,
    RedBlade,
    VoidSword,
    BloodKatana,
    GreatSword,
    Shovle,
    WeaponSwip,
    IceRage,
    JellyBall,
    SlimesBleast,
    ThrownSlik,
    RiseOfRadiance,
    GoblinKnight,
    MonsterBite,
    Crow,
    MonsterDagger,
    GreenGoblin,
    CurGoblinKnight,
    CurCrow,
    CurGreenGoblin,
    SkeletonArch,
    CurSkeletonArch,
    Arrow,
    GoblinBeastRider,
    SFXObj,
}

public abstract class APoolingObject:MonoBehaviour
{

    public PoolObjectType objectType;

    public void Death()
    {
        OnDeathInit();
        ObjectPooler.Instance.Return(this);
    }
    public abstract void OnBirth();
    public abstract void OnDeathInit();
}

public class ObjectPooler : HalfSingleMono<ObjectPooler>
{
    [SerializeField] private GameObject retruningParentObj;
    public GameObject currentReturningParentObj;
    private Dictionary<PoolObjectType, Queue<APoolingObject>> objectPoolList;

    protected override void Awake()
    {
        base.Awake();
        InitPoollist();

    }

    public void ReBakeObjectPooler()
    {
        objectPoolList.Clear();
    }

    public GameObject Get(APoolingObject prefab, Vector2 position, Vector3 rotation)
    {
        PoolObjectType key = prefab.objectType;
        if (!objectPoolList.ContainsKey(key))
        {
            objectPoolList[key] = new Queue<APoolingObject>();
        }
        APoolingObject obj;
        if (objectPoolList[key].Count == 0)
        {
            obj = Instantiate(prefab, position, Quaternion.Euler(rotation));
            obj.objectType = key;
            obj.OnBirth();

        }
        else
        {
            obj = objectPoolList[key].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.Euler(rotation);
            obj.gameObject.SetActive(true);
            obj.OnBirth();
        }

        obj.transform.SetParent(currentReturningParentObj.transform, worldPositionStays: false);

        if (obj != null) obj.gameObject.SetActive(true);
        return obj.gameObject;
    }

    public GameObject Get(APoolingObject prefab, Vector2 position, Vector3 rotation, Vector2 size)
    {
        PoolObjectType key = prefab.objectType;

        if (!objectPoolList.ContainsKey(key))
            objectPoolList[key] = new Queue<APoolingObject>();

        APoolingObject obj;

        if (objectPoolList[key].Count == 0)
        {
            obj = Instantiate(prefab, position, Quaternion.Euler(rotation));
            obj.transform.localScale = new Vector3(size.x, size.y);
            obj.objectType = key;
            obj.OnBirth();
        }
        else
        {
            obj = objectPoolList[key].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.Euler(rotation);
            obj.transform.localScale = new Vector3(size.x, size.y);
            obj.gameObject.SetActive(true);
            obj.OnBirth();
        }

        obj.transform.SetParent(currentReturningParentObj.transform, worldPositionStays: false);

        return obj.gameObject;
    }
    public GameObject Get(APoolingObject prefab, Transform parent)
    {
        PoolObjectType key = prefab.objectType;

        if (!objectPoolList.ContainsKey(key))
            objectPoolList[key] = new Queue<APoolingObject>();

        APoolingObject obj;

        if (objectPoolList[key].Count == 0)
        {
            obj = Instantiate(prefab, parent);
            obj.objectType = key;
            obj.OnBirth();
        }
        else
        {
            obj = objectPoolList[key].Dequeue();
            obj.transform.parent = parent;
            obj.gameObject.SetActive(true);
            obj.OnBirth();
        }

        return obj.gameObject;
    }

    public void Return(APoolingObject obj)
    {
        PoolObjectType key = obj.objectType;

        if (!objectPoolList.ContainsKey(key))
            objectPoolList[key] = new Queue<APoolingObject>();

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(currentReturningParentObj.transform, worldPositionStays: false);
        objectPoolList[key].Enqueue(obj);
    }
    public void InitPoollist()
    {
        Destroy(currentReturningParentObj);
        currentReturningParentObj = Instantiate(retruningParentObj);
        objectPoolList = new Dictionary<PoolObjectType, Queue<APoolingObject>>();
    }

}

