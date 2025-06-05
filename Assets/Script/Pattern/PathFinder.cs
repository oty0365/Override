using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    // 경로를 저장할 리스트 (기즈모에서 사용)
    private List<Vector2Int> currentPath = new List<Vector2Int>();

    public List<Vector2Int> PathFinding()
    {
        Tilemap tilemap = TileMapManager.Instance.tilemap;
        if (tilemap == null)
        {
            return null;
        }

        Vector3Int cellStart = tilemap.WorldToCell(gameObject.transform.position);
        Vector3Int cellTarget = tilemap.WorldToCell(target.transform.position);
        Vector2Int curPos = new Vector2Int(cellStart.x, cellStart.y);
        StartPos = curPos;
        Vector2Int targetPos = new Vector2Int(cellTarget.x, cellTarget.y);

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
            if (!openDict.ContainsKey(curPos)) continue;

            var curNode = openDict[curPos];
            openDict.Remove(curPos);
            closedDict[curPos] = curNode;
            var mapInfos = TileMapManager.Instance.mapInfos;

            if (curPos == targetPos) break;

            for (int i = 0; i < 8; i++)
            {
                var newPos = new Vector2Int(curPos.x + _eightX[i], curPos.y + _eightY[i]);
                var newCellPos = new Vector3Int(newPos.x, newPos.y, 0);

                if (!mapInfos.ContainsKey(newCellPos) || !mapInfos[newCellPos].ableToGo) continue;
                if (closedDict.ContainsKey(newPos)) continue;

                bool isDiagonal = _eightX[i] != 0 && _eightY[i] != 0;
                if (isDiagonal)
                {
                    Vector3Int adj1 = new Vector3Int(curPos.x + _eightX[i], curPos.y, 0);
                    Vector3Int adj2 = new Vector3Int(curPos.x, curPos.y + _eightY[i], 0);
                    if (!mapInfos.ContainsKey(adj1) || !mapInfos[adj1].ableToGo || !mapInfos.ContainsKey(adj2) || !mapInfos[adj2].ableToGo)
                        continue;
                }

                float moveCost = isDiagonal ? 1.4f : 1.0f;
                int g = curNode.gCost + Mathf.RoundToInt(moveCost * 10);
                int h = (int)Vector2Int.Distance(newPos, targetPos);
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

            // 현재 경로를 업데이트 (기즈모에서 사용)
            currentPath = path;

            /*Debug.Log("경로 출력:");
            foreach (var p in path)
            {
                Debug.Log($"Path Point: {p}");
            }*/

            closedList.Clear();
            foreach (var pos in path)
            {
                closedList.Add(new PassedPoint { pos = pos });
            }
            return path;
        }

        // 경로를 찾지 못한 경우 빈 리스트로 설정
        currentPath.Clear();
        return null;
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Tilemap tilemap = TileMapManager.Instance?.tilemap;
        if (tilemap == null) return;

        // 경로 선 그리기
        Gizmos.color = Color.green;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 worldPos1 = tilemap.GetCellCenterWorld(new Vector3Int(currentPath[i].x, currentPath[i].y, 0));
            Vector3 worldPos2 = tilemap.GetCellCenterWorld(new Vector3Int(currentPath[i + 1].x, currentPath[i + 1].y, 0));
            Gizmos.DrawLine(worldPos1, worldPos2);
        }

        // 경로 포인트 그리기
        Gizmos.color = Color.yellow;
        foreach (var point in currentPath)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(new Vector3Int(point.x, point.y, 0));
            Gizmos.DrawWireSphere(worldPos, 0.1f);
        }

        // 시작점과 목표점 강조
        if (currentPath.Count > 0)
        {
            Gizmos.color = Color.blue;
            Vector3 startWorldPos = tilemap.GetCellCenterWorld(new Vector3Int(currentPath[0].x, currentPath[0].y, 0));
            Gizmos.DrawSphere(startWorldPos, 0.15f);
            Gizmos.color = Color.red;
            Vector3 targetWorldPos = tilemap.GetCellCenterWorld(new Vector3Int(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y, 0));
            Gizmos.DrawSphere(targetWorldPos, 0.15f);
        }
    }

    /*private void OnDrawGizmosSelected()
    {

        if (closedList == null || closedList.Count == 0) return;

        Tilemap tilemap = TileMapManager.Instance?.tilemap;
        if (tilemap == null) return;
        Gizmos.color = Color.gray;
        foreach (var node in closedList)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(new Vector3Int(node.pos.x, node.pos.y, 0));
            Gizmos.DrawWireCube(worldPos, Vector3.one * 0.2f);
        }
    }*/
}