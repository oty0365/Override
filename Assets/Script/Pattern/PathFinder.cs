using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassedPoint
{
    public Vector2Int pos;
    public int gCost;
    public int hCost;
    public int fCost;
    public Vector2Int parent;
}

public class PathFinder : MonoBehaviour
{
    public GameObject target;
    public GameObject wayPoint;

    [Header("Monster Size Settings")]
    [Tooltip("몬스터의 가로 크기 (타일 단위)")]
    public int monsterWidth = 1;
    [Tooltip("몬스터의 세로 크기 (타일 단위)")]
    public int monsterHeight = 1;
    [Tooltip("몬스터 중심점에서의 오프셋")]
    public Vector2Int centerOffset = Vector2Int.zero;

    [Header("Player Size Settings")]
    [Tooltip("플레이어의 가로 크기 (타일 단위)")]
    public int playerWidth = 1;
    [Tooltip("플레이어의 세로 크기 (타일 단위)")]
    public int playerHeight = 1;
    [Tooltip("플레이어 중심점에서의 오프셋")]
    public Vector2Int playerCenterOffset = Vector2Int.zero;

    private Vector2Int _startPos;
    public Vector2Int StartPos
    {
        get => _startPos;
        set
        {
            _startPos = value;
        }
    }
    public PriorityQueue<Vector2Int> openQueue = new PriorityQueue<Vector2Int>();
    public List<PassedPoint> closedList = new List<PassedPoint>();

    private int[] _eightX = new int[8] { 0, 1, 1, 0, -1, -1, 1, -1 };
    private int[] _eightY = new int[8] { 1, 0, 1, -1, 0, -1, -1, 1 };

    private bool CanPlayerFitAt(Vector2Int centerPos)
    {
        var mapInfos = TileMapManager.Instance.mapInfos;

        int startX = centerPos.x + playerCenterOffset.x - playerWidth / 2;
        int startY = centerPos.y + playerCenterOffset.y - playerHeight / 2;
        int endX = startX + playerWidth;
        int endY = startY + playerHeight;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (!mapInfos.ContainsKey(tilePos) || !mapInfos[tilePos].ableToGo)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CanMonsterFitAt(Vector2Int centerPos)
    {
        var mapInfos = TileMapManager.Instance.mapInfos;

        int startX = centerPos.x + centerOffset.x - monsterWidth / 2;
        int startY = centerPos.y + centerOffset.y - monsterHeight / 2;
        int endX = startX + monsterWidth;
        int endY = startY + monsterHeight;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (!mapInfos.ContainsKey(tilePos) || !mapInfos[tilePos].ableToGo)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CanMoveDiagonally(Vector2Int from, Vector2Int to)
    {
        if (!CanMonsterFitAt(to))
            return false;

        var mapInfos = TileMapManager.Instance.mapInfos;

        int deltaX = to.x - from.x;
        int deltaY = to.y - from.y;

        Vector2Int side1 = new Vector2Int(from.x + deltaX, from.y);
        Vector2Int side2 = new Vector2Int(from.x, from.y + deltaY);

        if (!CanMonsterFitAt(side1) || !CanMonsterFitAt(side2))
            return false;

        return true;
    }

    public List<Vector2Int> PathFinding()
    {
        var curPos = new Vector2Int((int)Mathf.Round(gameObject.transform.position.x), (int)Mathf.Round(gameObject.transform.position.y));
        StartPos = curPos;
        var targetPos = new Vector2Int((int)Mathf.Round(target.transform.position.x), (int)Mathf.Round(target.transform.position.y));

        if (!CanMonsterFitAt(curPos))
        {
            Debug.LogWarning("몬스터가 시작 위치에 맞지 않습니다!");
            return null;
        }

        if (!CanPlayerFitAt(targetPos))
        {
            Debug.LogWarning("플레이어가 목표 위치에 맞지 않습니다!");
            return null;
        }

        openQueue.Clear();
        closedList.Clear();

        var openDict = new Dictionary<Vector2Int, PassedPoint>();
        var closedDict = new Dictionary<Vector2Int, PassedPoint>();

        var startNode = new PassedPoint
        {
            pos = curPos,
            gCost = 0,
            hCost = (int)Vector2Int.Distance(curPos, targetPos),
            fCost = (int)Vector2Int.Distance(curPos, targetPos),
            parent = curPos
        };

        openQueue.Enqueue(curPos, startNode.fCost);
        openDict[curPos] = startNode;

        while (openQueue.Count > 0)
        {
            curPos = openQueue.Dequeue();
            if (!openDict.ContainsKey(curPos))
            {
                continue;
            }

            var curNode = openDict[curPos];
            openDict.Remove(curPos);
            closedDict[curPos] = curNode;

            if (curPos == targetPos)
            {
                break;
            }

            for (int i = 0; i < 8; i++)
            {
                var newPos = new Vector2Int(curPos.x + _eightX[i], curPos.y + _eightY[i]);
                if (!CanMonsterFitAt(newPos))
                {
                    continue;
                }

                if (closedDict.ContainsKey(newPos))
                {
                    continue;
                }

                bool isDiagonal = _eightX[i] != 0 && _eightY[i] != 0;
                if (isDiagonal && !CanMoveDiagonally(curPos, newPos))
                {
                    continue;
                }

                float moveCost = isDiagonal ? 1.4f : 1.0f;
                int g = curNode.gCost + Mathf.RoundToInt(moveCost * 10);
                int h = (int)Vector2Int.Distance(newPos, targetPos);
                //h += Random.Range(0, 400);
                int f = g + h;

                if (openDict.TryGetValue(newPos, out var existingNode))
                {
                    if (g < existingNode.gCost)
                    {
                        existingNode.gCost = g;
                        existingNode.fCost = f;
                        existingNode.parent = curPos;
                        openQueue.Enqueue(newPos, f);
                    }
                }
                else
                {
                    var newNode = new PassedPoint
                    {
                        pos = newPos,
                        gCost = g,
                        hCost = h,
                        fCost = f,
                        parent = curPos
                    };
                    openQueue.Enqueue(newPos, f);
                    openDict[newPos] = newNode;
                }
            }
        }

        if (closedDict.ContainsKey(targetPos))
        {
            var path = new List<Vector2Int>();
            var trace = targetPos;
            while (trace != StartPos)
            {
                path.Add(trace);
                trace = closedDict[trace].parent;
            }
            path.Add(StartPos);
            path.Reverse();

            /*Debug.Log("경로 출력:");
            foreach (var p in path)
            {
                Instantiate(wayPoint, p, Quaternion.identity);
            }*/

            closedList.Clear();
            foreach (var pos in path)
            {
                closedList.Add(new PassedPoint { pos = pos });
            }
            return path;
        }

        Debug.LogWarning("경로를 찾을 수 없습니다!");
        return null;
    }

    // 디버그용: 몬스터와 플레이어가 차지하는 영역 시각화
    /*private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // 몬스터 영역 표시 (빨간색)
            Vector2Int monsterPos = new Vector2Int(
                (int)Mathf.Round(transform.position.x),
                (int)Mathf.Round(transform.position.y)
            );

            Gizmos.color = Color.red;

            int monsterStartX = monsterPos.x + centerOffset.x - monsterWidth / 2;
            int monsterStartY = monsterPos.y + centerOffset.y - monsterHeight / 2;

            for (int x = 0; x < monsterWidth; x++)
            {
                for (int y = 0; y < monsterHeight; y++)
                {
                    Vector3 tileCenter = new Vector3(monsterStartX + x + 0.5f, monsterStartY + y + 0.5f, 0);
                    Gizmos.DrawWireCube(tileCenter, Vector3.one);
                }
            }

            // 플레이어 영역 표시 (파란색)
            if (target != null)
            {
                Vector2Int playerPos = new Vector2Int(
                    (int)Mathf.Round(target.transform.position.x),
                    (int)Mathf.Round(target.transform.position.y)
                );

                Gizmos.color = Color.blue;

                int playerStartX = playerPos.x + playerCenterOffset.x - playerWidth / 2;
                int playerStartY = playerPos.y + playerCenterOffset.y - playerHeight / 2;

                for (int x = 0; x < playerWidth; x++)
                {
                    for (int y = 0; y < playerHeight; y++)
                    {
                        Vector3 tileCenter = new Vector3(playerStartX + x + 0.5f, playerStartY + y + 0.5f, 0);
                        Gizmos.DrawWireCube(tileCenter, Vector3.one);
                    }
                }

                // 플레이어 중심점 표시
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0), 0.2f);
            }

            // 몬스터 중심점 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(monsterPos.x + 0.5f, monsterPos.y + 0.5f, 0), 0.2f);
        }
    }*/
}