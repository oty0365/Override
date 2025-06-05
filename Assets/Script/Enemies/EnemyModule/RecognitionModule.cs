using UnityEngine;

public class RecognitionModule : Module
{
    public GameObject target;
    public LayerMask layerMask;
    public float debugRange = 5f; // 기즈모 표시용 범위

    private bool lastRecognitionResult = false;
    private RaycastHit2D lastHit;
    private Vector2 lastDirection;

    public bool Recognize(float range)
    {
        if (target == null) return false;

        RaycastHit2D hit;
        Vector2 dir = target.transform.position - gameObject.transform.position;
        lastDirection = dir.normalized;

        hit = Physics2D.Raycast(gameObject.transform.position, dir, range, layerMask);
        lastHit = hit;

        if (hit.collider == null)
        {
            lastRecognitionResult = false;
            return false;
        }

        if (hit.collider.CompareTag(target.tag))
        {
            lastRecognitionResult = true;
            return true;
        }

        lastRecognitionResult = false;
        return false;
    }
    public Vector2 SolveDirection(Vector2 target,Vector2 owner)
    {
        return (target - owner).normalized;
    } 

    public override void Initialize()
    {
        target = enemy.target;
    }

    void OnDrawGizmos()
    {
        if (target == null) return;

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 direction = (targetPos - startPos).normalized;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(startPos, direction * debugRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.3f);

        if (lastRecognitionResult)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        if (lastHit.collider != null)
        {
            Gizmos.DrawLine(startPos, lastHit.point);
            Gizmos.DrawWireSphere(lastHit.point, 0.2f);
        }
        else
        {
            Gizmos.DrawLine(startPos, startPos + direction * debugRange);
        }

        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        Gizmos.DrawSphere(startPos, debugRange);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.transform.position;
        float distance = Vector3.Distance(startPos, targetPos);

        Gizmos.color = Color.white;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startPos, targetPos);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(startPos, debugRange);
    }
}