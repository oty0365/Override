using UnityEngine;

public class RecognitionModule : Module
{
    public GameObject target;
    public LayerMask layerMask;
    public bool Recognize(float range)
    {
        RaycastHit2D hit;
        Vector2 dir = target.transform.position-gameObject.transform.position;
        hit = Physics2D.Raycast(gameObject.transform.position, dir,range,layerMask);
        Debug.Log(hit.collider);
        if (hit.collider == null)
        {
            return false;
        }
        if(hit.collider.CompareTag(target.tag))
        {
            return true;
        }
        return false;
    }
    public override void Initialize()
    {
        target = enemy.target;
    }

}
