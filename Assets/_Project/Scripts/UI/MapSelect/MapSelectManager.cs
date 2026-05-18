using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Map Generate")]
    [SerializeField] private int routeLength = 8;
    [SerializeField] private int minNodesPerColumn = 2;
    [SerializeField] private int maxNodesPerColumn = 3;
    [SerializeField] private float startX = -750f;
    [SerializeField] private float xSpacing = 210f;
    [SerializeField] private float ySpacing = 180f;
    [SerializeField] private float randomYJitter = 35f;

    [Header("Prefabs")]
    [SerializeField] private MapSelectNode nodePrefab;
    [SerializeField] private MapPathLine linePrefab;

    [Header("Parents")]
    [SerializeField] private RectTransform nodesParent;
    [SerializeField] private RectTransform linesParent;

    [Header("Cursor")]
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private Vector2 cursorOffset = new Vector2(-80f, 0f);

    [Header("Input")]
    [SerializeField] private KeyCode confirmKey = KeyCode.X;

    private readonly Dictionary<string, MapSelectNode> spawnedNodes = new Dictionary<string, MapSelectNode>();
    private readonly List<MapSelectNode> selectableNodes = new List<MapSelectNode>();

    private int currentSelectIndex = 0;

    private void Start()
    {
        if (!MapRunData.HasMap)
        {
            MapRunData.Generate(
                routeLength,
                minNodesPerColumn,
                maxNodesPerColumn,
                startX,
                xSpacing,
                ySpacing,
                randomYJitter
            );
        }

        BuildMapUI();
        RefreshCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(confirmKey))
        {
            SelectCurrentNode();
        }
    }

    private void BuildMapUI()
    {
        ClearChildren(nodesParent);
        ClearChildren(linesParent);

        spawnedNodes.Clear();
        selectableNodes.Clear();

        int currentDepth = SelectedRoomData.CurrentDepth;
        int currentNodeIndex = SelectedRoomData.CurrentNodeIndex;

        List<int> nextSelectableIndices = new List<int>();

        if (currentDepth < MapRunData.Columns.Count - 1)
        {
            MapNodeInfo currentNode = MapRunData.Columns[currentDepth][currentNodeIndex];
            nextSelectableIndices = currentNode.nextNodeIndices;
        }

        // 노드 생성
        for (int depth = 0; depth < MapRunData.Columns.Count; depth++)
        {
            List<MapNodeInfo> column = MapRunData.Columns[depth];

            for (int i = 0; i < column.Count; i++)
            {
                MapNodeInfo info = column[i];

                MapSelectNode node = Instantiate(nodePrefab, nodesParent);

                bool completed = depth <= currentDepth;
                bool selectable = depth == currentDepth + 1 && nextSelectableIndices.Contains(i);

                node.Setup(info, selectable, completed);

                string key = GetKey(depth, i);
                spawnedNodes.Add(key, node);

                if (selectable)
                {
                    selectableNodes.Add(node);
                }
            }
        }

        // 선 생성
        for (int depth = 0; depth < MapRunData.Columns.Count - 1; depth++)
        {
            List<MapNodeInfo> column = MapRunData.Columns[depth];

            foreach (MapNodeInfo nodeInfo in column)
            {
                foreach (int nextIndex in nodeInfo.nextNodeIndices)
                {
                    MapSelectNode fromNode = spawnedNodes[GetKey(depth, nodeInfo.index)];
                    MapSelectNode toNode = spawnedNodes[GetKey(depth + 1, nextIndex)];

                    MapPathLine line = Instantiate(linePrefab, linesParent);

                    bool activeLine =
                        depth == currentDepth &&
                        nodeInfo.index == currentNodeIndex &&
                        nextSelectableIndices.Contains(nextIndex);

                    line.Setup(
                        fromNode.GetComponent<RectTransform>(),
                        toNode.GetComponent<RectTransform>(),
                        6f,
                        activeLine
                    );
                }
            }
        }

        currentSelectIndex = 0;
    }

    private void MoveSelection(int direction)
    {
        if (selectableNodes.Count == 0)
            return;

        currentSelectIndex += direction;

        if (currentSelectIndex < 0)
            currentSelectIndex = selectableNodes.Count - 1;
        else if (currentSelectIndex >= selectableNodes.Count)
            currentSelectIndex = 0;

        RefreshCursor();
    }

    private void RefreshCursor()
    {
        if (selectCursor == null)
            return;

        if (selectableNodes.Count == 0)
        {
            selectCursor.gameObject.SetActive(false);
            return;
        }

        selectCursor.gameObject.SetActive(true);

        RectTransform selectedNodeRect = selectableNodes[currentSelectIndex].GetComponent<RectTransform>();
        selectCursor.position = selectedNodeRect.position + (Vector3)cursorOffset;
    }

    private void SelectCurrentNode()
    {
        if (selectableNodes.Count == 0)
            return;

        MapNodeInfo selectedInfo = selectableNodes[currentSelectIndex].NodeInfo;

        SelectedRoomData.SelectRoom(selectedInfo.roomType);
        SelectedRoomData.SetCurrentNode(selectedInfo.depth, selectedInfo.index);

        Debug.Log($"선택한 방: {selectedInfo.roomType}, Depth: {selectedInfo.depth}, Index: {selectedInfo.index}");

        SceneManager.LoadScene(gameSceneName);
    }

    private string GetKey(int depth, int index)
    {
        return $"{depth}_{index}";
    }

    private void ClearChildren(RectTransform parent)
    {
        if (parent == null)
            return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}