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
    [Tooltip("������ ���� ũ�� (Ÿ�� ����)")]
    public int monsterWidth = 1;
    [Tooltip("������ ���� ũ�� (Ÿ�� ����)")]
    public int monsterHeight = 1;
    [Tooltip("���� �߽��������� ������")]
    public Vector2Int centerOffset = Vector2Int.zero;

    [Header("Player Size Settings")]
    [Tooltip("�÷��̾��� ���� ũ�� (Ÿ�� ����)")]
    public int playerWidth = 1;
    [Tooltip("�÷��̾��� ���� ũ�� (Ÿ�� ����)")]
    public int playerHeight = 1;
    [Tooltip("�÷��̾� �߽��������� ������")]
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
            Debug.LogWarning("���Ͱ� ���� ��ġ�� ���� �ʽ��ϴ�!");
            return null;
        }

        if (!CanPlayerFitAt(targetPos))
        {
            Debug.LogWarning("�÷��̾ ��ǥ ��ġ�� ���� �ʽ��ϴ�!");
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

            /*Debug.Log("��� ���:");
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

        Debug.LogWarning("��θ� ã�� �� �����ϴ�!");
        return null;
    }

    // ����׿�: ���Ϳ� �÷��̾ �����ϴ� ���� �ð�ȭ
    /*private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // ���� ���� ǥ�� (������)
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

            // �÷��̾� ���� ǥ�� (�Ķ���)
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

                // �÷��̾� �߽��� ǥ��
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0), 0.2f);
            }

            // ���� �߽��� ǥ��
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(monsterPos.x + 0.5f, monsterPos.y + 0.5f, 0), 0.2f);
        }
    }*/
}