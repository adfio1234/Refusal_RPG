using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Hub,
    Combat,
    Shop,
    Treasure,
    Boss
}

public static class SelectedRoomData
{
    public static RoomType SelectedRoomType = RoomType.Hub;

    public static int CurrentDepth = 0;
    public static int CurrentNodeIndex = 0;

    public static void ResetRun()
    {
        SelectedRoomType = RoomType.Hub;
        CurrentDepth = 0;
        CurrentNodeIndex = 0;
        MapRunData.Clear();
    }

    public static void SelectRoom(RoomType roomType)
    {
        SelectedRoomType = roomType;
    }

    public static void SetCurrentNode(int depth, int nodeIndex)
    {
        CurrentDepth = depth;
        CurrentNodeIndex = nodeIndex;
    }
}

public class MapNodeInfo
{
    public int depth;
    public int index;
    public RoomType roomType;
    public Vector2 anchoredPosition;
    public List<int> nextNodeIndices = new List<int>();
}

public static class MapRunData
{
    public static List<List<MapNodeInfo>> Columns = new List<List<MapNodeInfo>>();

    public static bool HasMap => Columns != null && Columns.Count > 0;

    public static void Clear()
    {
        Columns.Clear();
    }

    public static void Generate(
        int routeLength,
        int minNodesPerColumn,
        int maxNodesPerColumn,
        float startX,
        float xSpacing,
        float ySpacing,
        float randomYJitter
    )
    {
        Columns.Clear();

        // 0번 열: 시작 지점
        List<MapNodeInfo> startColumn = new List<MapNodeInfo>();

        startColumn.Add(new MapNodeInfo
        {
            depth = 0,
            index = 0,
            roomType = RoomType.Hub,
            anchoredPosition = new Vector2(startX, 0f)
        });

        Columns.Add(startColumn);

        // 1 ~ routeLength 열 생성
        for (int depth = 1; depth <= routeLength; depth++)
        {
            bool isBossColumn = depth == routeLength;

            int nodeCount = isBossColumn
                ? 1
                : Random.Range(minNodesPerColumn, maxNodesPerColumn + 1);

            List<MapNodeInfo> column = new List<MapNodeInfo>();

            for (int i = 0; i < nodeCount; i++)
            {
                float centerOffset = (nodeCount - 1) * 0.5f;
                float y = (centerOffset - i) * ySpacing;
                y += Random.Range(-randomYJitter, randomYJitter);

                column.Add(new MapNodeInfo
                {
                    depth = depth,
                    index = i,
                    roomType = isBossColumn ? RoomType.Boss : RoomType.Combat,
                    anchoredPosition = new Vector2(startX + depth * xSpacing, y)
                });
            }

            Columns.Add(column);
        }

        GenerateConnections();
    }

    private static void GenerateConnections()
    {
        for (int depth = 0; depth < Columns.Count - 1; depth++)
        {
            List<MapNodeInfo> currentColumn = Columns[depth];
            List<MapNodeInfo> nextColumn = Columns[depth + 1];

            int currentCount = currentColumn.Count;
            int nextCount = nextColumn.Count;

            foreach (MapNodeInfo node in currentColumn)
            {
                node.nextNodeIndices.Clear();
            }

            // 다음 열 노드들을 현재 열 노드들에게 위→아래 순서로 배분
            for (int nextIndex = 0; nextIndex < nextCount; nextIndex++)
            {
                int ownerIndex = GetMappedIndex(nextIndex, nextCount, currentCount);

                if (!currentColumn[ownerIndex].nextNodeIndices.Contains(nextIndex))
                {
                    currentColumn[ownerIndex].nextNodeIndices.Add(nextIndex);
                }
            }

            // 현재 열의 모든 노드가 최소 1개 연결을 갖게 보정
            for (int currentIndex = 0; currentIndex < currentCount; currentIndex++)
            {
                MapNodeInfo currentNode = currentColumn[currentIndex];

                if (currentNode.nextNodeIndices.Count == 0)
                {
                    int targetIndex = GetMappedIndex(currentIndex, currentCount, nextCount);

                    if (!currentNode.nextNodeIndices.Contains(targetIndex))
                    {
                        currentNode.nextNodeIndices.Add(targetIndex);
                    }
                }

                currentNode.nextNodeIndices.Sort();
            }
        }
    }

    private static int GetMappedIndex(int index, int fromCount, int toCount)
    {
        if (toCount <= 1)
            return 0;

        if (fromCount <= 1)
            return 0;

        float ratio = index / (float)(fromCount - 1);
        int mappedIndex = Mathf.RoundToInt(ratio * (toCount - 1));

        return Mathf.Clamp(mappedIndex, 0, toCount - 1);
    }
}