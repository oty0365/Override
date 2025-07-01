using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderModule : Module
{
    public PathFinder pathFinder;
    private List<Vector2Int> _wayPoints;
    private Coroutine _currentMove;
    private Vector2 currentTargetPos;

    public List<Vector2Int> WayPoints
    {
        get => _wayPoints;
        set
        {
            if (_wayPoints != value)
            {
                _wayPoints = value;
                if (_currentMove != null)
                {
                    StopCoroutine(_currentMove);
                    _currentMove = null;
                }
                if (_wayPoints != null && _wayPoints.Count > 0)
                {
                    _currentMove = StartCoroutine(Move(1));
                }
            }
        }
    }

    public void SetMove()
    {
        WayPoints = pathFinder.PathFinding();
        if (WayPoints == null)
        {
            _currentMove = StartCoroutine(Move(0));
        }
    }

    public bool CheckMoveDist()
    {
        if (Vector2.Distance(currentTargetPos, (Vector2)pathFinder.target.transform.position) > 1f)
        {
            WayPoints = pathFinder.PathFinding();
            currentTargetPos = pathFinder.target.transform.position;
            if (WayPoints == null)
            {
                return false;
            }
        }
        return true;
    }

    public void Stop()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        if (_currentMove != null)
        {
            StopCoroutine(_currentMove);
            _currentMove = null;
        }
        WayPoints = null;
    }

    private int FindClosestWaypointIndex(Vector2 currentPos, List<Vector2Int> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
            return 0;

        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector2 waypointPos = new Vector2(waypoints[i].x + 0.5f, waypoints[i].y + 0.5f);
            float distance = Vector2.Distance(currentPos, waypointPos);

            if (distance > 2f && distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        if (minDistance == float.MaxValue)
        {
            return 0;
        }

        return closestIndex;
    }

    public IEnumerator Move(int mode)
    {
        if (mode == 1)
        {
            if (_wayPoints == null || _wayPoints.Count == 0)
                yield break;

            Vector2 currentPos = gameObject.transform.position;
            int closestIndex = FindClosestWaypointIndex(currentPos, _wayPoints);
            int startIndex = closestIndex;

            for (int i = startIndex; i < _wayPoints.Count; i++)
            {
                var targetPos = new Vector2(_wayPoints[i].x + 0.5f, _wayPoints[i].y + 0.5f);
                if (Vector2.Distance(gameObject.transform.position, targetPos) <= 0.3f)
                    continue;

                while (Vector2.Distance(gameObject.transform.position, targetPos) > 0.1f)
                {
                    Vector2 currentPosition = gameObject.transform.position;
                    Vector2 moveDirection = (targetPos - currentPosition).normalized;
                    float moveDistance = enemy.monsterData.moveSpeed * Time.fixedDeltaTime;

                    if (Vector2.Distance(currentPosition, targetPos) <= moveDistance)
                    {
                        enemy.rb2D.MovePosition(targetPos);
                        break;
                    }
                    else
                    {
                        Vector2 newPosition = currentPosition + moveDirection * moveDistance;
                        enemy.rb2D.MovePosition(newPosition);
                    }

                    yield return new WaitForFixedUpdate(); 
                }
            }
            Vector2 finalTarget = new Vector2(
                Mathf.Floor(pathFinder.target.transform.position.x) + 0.5f,
                Mathf.Floor(pathFinder.target.transform.position.y) + 0.5f
            );

            while (Vector2.Distance(gameObject.transform.position, finalTarget) > 0.1f)
            {
                Vector2 currentPosition = gameObject.transform.position;
                Vector2 moveDirection = (finalTarget - currentPosition).normalized;
                float moveDistance = enemy.monsterData.moveSpeed * Time.fixedDeltaTime;

                if (Vector2.Distance(currentPosition, finalTarget) <= moveDistance)
                {
                    enemy.rb2D.MovePosition(finalTarget);
                    break;
                }
                else
                {
                    Vector2 newPosition = currentPosition + moveDirection * moveDistance;
                    enemy.rb2D.MovePosition(newPosition);
                }

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            // 직접 타겟으로 이동
            Vector2 finalTarget = new Vector2(
                Mathf.Floor(pathFinder.target.transform.position.x) + 0.5f,
                Mathf.Floor(pathFinder.target.transform.position.y) + 0.5f
            );

            while (Vector2.Distance(gameObject.transform.position, finalTarget) > 0.1f)
            {
                Vector2 currentPosition = gameObject.transform.position;
                Vector2 moveDirection = (finalTarget - currentPosition).normalized;
                float moveDistance = enemy.monsterData.moveSpeed * Time.fixedDeltaTime;

                if (Vector2.Distance(currentPosition, finalTarget) <= moveDistance)
                {
                    enemy.rb2D.MovePosition(finalTarget);
                    break;
                }
                else
                {
                    Vector2 newPosition = currentPosition + moveDirection * moveDistance;
                    enemy.rb2D.MovePosition(newPosition);
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }

    public override void Initialize()
    {
        pathFinder.target = enemy.target;
    }
}