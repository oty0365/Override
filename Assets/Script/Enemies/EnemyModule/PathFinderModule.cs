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
            // Ÿ�� �������� ��� (x, y ��� 0.5f �߰�)
            Vector2 waypointPos = new Vector2(waypoints[i].x + 0.5f, waypoints[i].y + 0.5f);
            float distance = Vector2.Distance(currentPos, waypointPos);

            if (distance > 2f && distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        // ��� ������ �ʹ� ������ ù ��° ���� ��ȯ
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

            // ���� ��ġ���� ���� ����� ��������Ʈ ã��
            Vector2 currentPos = gameObject.transform.position;
            int closestIndex = FindClosestWaypointIndex(currentPos, _wayPoints);
            int startIndex = closestIndex;

            // �ش� �������� �����ؼ� ������ �̵�
            for (int i = startIndex; i < _wayPoints.Count; i++)
            {
                // Ÿ�� �������� �̵� (x, y ��� 0.5f �߰�)
                var targetPos = new Vector2(_wayPoints[i].x + 0.5f, _wayPoints[i].y + 0.5f);

                // ���� ��ġ�� �̹� ��������Ʈ�� ����� ������ �ǳʶٱ�
                if (Vector2.Distance(gameObject.transform.position, targetPos) <= 0.3f)
                    continue;

                while (Vector2.Distance(gameObject.transform.position, targetPos) > 0.1f)
                {
                    enemy.rb2D.MovePosition(Vector2.MoveTowards(gameObject.transform.position, targetPos, Time.deltaTime * enemy.monsterData.moveSpeed));
                    yield return null;
                }
            }

            // ���� ��ǥ������ Ÿ�� �������� �̵�
            Vector2 finalTarget = new Vector2(
                Mathf.Floor(pathFinder.target.transform.position.x) + 0.5f,
                Mathf.Floor(pathFinder.target.transform.position.y) + 0.5f
            );

            while (Vector2.Distance(gameObject.transform.position, finalTarget) > 0.1f)
            {
                enemy.rb2D.MovePosition(Vector2.MoveTowards(gameObject.transform.position, finalTarget, Time.deltaTime * enemy.monsterData.moveSpeed));
                yield return null;
            }
        }
        else
        {
            // ���� Ÿ������ �̵��� ���� Ÿ�� �������� �̵�
            Vector2 finalTarget = new Vector2(
                Mathf.Floor(pathFinder.target.transform.position.x) + 0.5f,
                Mathf.Floor(pathFinder.target.transform.position.y) + 0.5f
            );

            while (Vector2.Distance(gameObject.transform.position, finalTarget) > 0.1f)
            {
                enemy.rb2D.MovePosition(Vector2.MoveTowards(gameObject.transform.position, finalTarget, Time.deltaTime * enemy.monsterData.moveSpeed));
                yield return null;
            }
        }
    }

    public override void Initialize()
    {
        pathFinder.target = enemy.target;
    }
}