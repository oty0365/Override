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
    public void CheckMoveDist()
    {
        if (Vector2.Distance(currentTargetPos, (Vector2)pathFinder.target.transform.position) > 3f)
        {
            WayPoints = pathFinder.PathFinding();
            currentTargetPos = pathFinder.target.transform.position;
            if (WayPoints == null)
            {
                _currentMove = StartCoroutine(Move(0));
            }

        }
    }
    public void Stop()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        StopCoroutine(_currentMove);
        WayPoints = null;
    }
    public IEnumerator Move(int mode)
    {
        if(mode == 1)
        {
            if (_wayPoints == null || _wayPoints.Count == 0)
                yield break;

            foreach (var i in WayPoints)
            {
                while (Vector2.Distance(gameObject.transform.position, i) > 0.01f)
                {
                    enemy.rb2D.MovePosition(Vector2.MoveTowards(gameObject.transform.position, i, Time.deltaTime * enemy.monsterData.moveSpeed));
                    yield return null;
                }
            }
        }
        else
        {
            while (Vector2.Distance(gameObject.transform.position, pathFinder.target.transform.position) > 0.01f)
            {
                enemy.rb2D.MovePosition(Vector2.MoveTowards(gameObject.transform.position, pathFinder.target.transform.position, Time.deltaTime * enemy.monsterData.moveSpeed));
                yield return null;
            }
        }
    }
    public override void Initialize()
    {
        pathFinder.target = enemy.target;
    }


}