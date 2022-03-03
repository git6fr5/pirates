using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid {

    [System.Serializable]
    public class GridSettings {
        public int GridWidth;
        public int GridHeight;
        public int SecondaryBranchDepth;
        public int TertiaryBranchDepth;
        public bool OverlapSecondaryBranches;
        public bool OverlapTertiaryBranches;
    }

    private Node[][] m_Array;
    public Node[][] Array => m_Array;

    private Node m_Entrance;
    public Node Entrance => m_Entrance;

    private Node m_Exit;
    public Node Exit => m_Exit;

    private List<Node> m_PrimaryPath;
    public List<Node> PrimaryPath => m_PrimaryPath;

    private List<Node>[] m_SecondaryPaths;
    public List<Node>[] SecondaryPaths => m_SecondaryPaths;

    private List<List<Node>> m_TertiaryPaths;
    public List<List<Node>> TertiaryPaths => m_TertiaryPaths;


    public Grid(int width, int height) {
        m_Array = new Node[height][];
        for (int i = 0; i < height; i++) {
            m_Array[i] = new Node[width];
            for (int j = 0; j < width; j++) {
                m_Array[i][j] = new Node(j, i);
            }
        }
    }

    public Grid(GridSettings gridSettings) {
        m_Array = new Node[gridSettings.GridHeight][];
        for (int i = 0; i < gridSettings.GridHeight; i++) {
            m_Array[i] = new Node[gridSettings.GridWidth];
            for (int j = 0; j < gridSettings.GridWidth; j++) {
                m_Array[i][j] = new Node(j, i);
            }
        }
        Generate(gridSettings);
    }

    public void Generate(GridSettings gridSettings) {
        GeneratePrimary();
        GenerateSecondary(gridSettings.SecondaryBranchDepth, gridSettings.OverlapSecondaryBranches);
        GenerateTertiary(gridSettings.TertiaryBranchDepth, gridSettings.OverlapTertiaryBranches);
    }

    public void Generate(int secondaryBranchDepth, bool overlappingSecondaryBranches,
                         int tertiaryBranchDepth, bool overlappingTertiaryBranches) {
        GeneratePrimary();
        GenerateSecondary(secondaryBranchDepth, overlappingSecondaryBranches);
        GenerateTertiary(tertiaryBranchDepth, overlappingTertiaryBranches);
    }

    public void GeneratePrimary() {
        int height = m_Array.Length;
        int width = m_Array[0].Length;

        m_Entrance = m_Array[(int)Mathf.Ceil(height / 2)][0];
        m_Exit = m_Array[(int)Mathf.Ceil(height / 2)][width - 1];

        List<Node> pathControls = new List<Node>();
        pathControls.Add(m_Entrance);
        for (int i = 1; i < width - 1; i++) {
            pathControls.Add(m_Array[Random.Range(1, height - 1)][i]);
        }
        pathControls.Add(m_Exit);

        List<Node>[] paths = new List<Node>[pathControls.Count - 1];
        for (int i = 1; i < pathControls.Count; i++) {
            paths[i - 1] = ManhattanPath(pathControls[i], pathControls[i - 1]);

        }

        m_PrimaryPath = new List<Node>();
        for (int i = 0; i < paths.Length; i++) {
            for (int j = 0; j < paths[i].Count; j++) {
                m_PrimaryPath.Add(paths[i][j]);
            }
        }

    }

    public void GenerateSecondary(int branchDepth, bool overlappingBranches) {
        if (m_PrimaryPath == null || m_PrimaryPath.Count < 2) {
            return;
        }
        int height = m_Array.Length;
        int width = m_Array[0].Length;

        m_SecondaryPaths = new List<Node>[m_PrimaryPath.Count];

        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                m_Array[i][j].Unmark();
            }
        }
        for (int i = 0; i < m_PrimaryPath.Count; i++) {
            m_PrimaryPath[i].Mark();
        }

        for (int i = 0; i < m_PrimaryPath.Count; i++) {
            List<Node> unmarkedAdjacentNodes = GetUnmarkedAdjacentNodes(m_PrimaryPath[i]);
            if (unmarkedAdjacentNodes.Count != 0) {
                Node randomUnmarkedAdjacentNode = unmarkedAdjacentNodes[Random.Range(0, unmarkedAdjacentNodes.Count)];
                m_SecondaryPaths[i] = ManhattanPath(m_PrimaryPath[i], randomUnmarkedAdjacentNode);
                if (!overlappingBranches) {
                    randomUnmarkedAdjacentNode.Mark();
                }
            }
            else {
                m_SecondaryPaths[i] = ManhattanPath(m_PrimaryPath[i], m_PrimaryPath[i]);
            }
        }

        for (int i = 0; i < m_SecondaryPaths.Length; i++) {
            for (int j = 0; j < m_SecondaryPaths[i].Count; j++) {
                m_SecondaryPaths[i][j].Mark();
            }
        }

        for (int n = 0; n < branchDepth; n++) {

            for (int i = 0; i < m_SecondaryPaths.Length; i++) {
                int pathLength = m_SecondaryPaths[i].Count;
                if (pathLength > 1) {
                    List<Node> unmarkedAdjacentNodes = GetUnmarkedAdjacentNodes(m_SecondaryPaths[i][pathLength - 1]);
                    if (unmarkedAdjacentNodes.Count != 0) {
                        Node randomUnmarkedAdjacentNode = unmarkedAdjacentNodes[Random.Range(0, unmarkedAdjacentNodes.Count)];
                        m_SecondaryPaths[i].Add(randomUnmarkedAdjacentNode);
                        if (!overlappingBranches) {
                            randomUnmarkedAdjacentNode.Mark();
                        }
                    }
                }
            }

            for (int i = 0; i < m_SecondaryPaths.Length; i++) {
                int pathLength = m_SecondaryPaths[i].Count;
                m_SecondaryPaths[i][pathLength - 1].Mark();
            }
        }

    }

    public void GenerateTertiary(int branchDepth, bool overlappingBranches) {
        int depth = 0;
        List<Node> markedNodes = GetAllMarkedNodes();
        if (markedNodes.Count == 0) {
            return;
        }

        m_TertiaryPaths = new List<List<Node>>();
        while (markedNodes.Count < m_Array.Length * m_Array[0].Length && depth < branchDepth) {

            List<Node> markingQueue = new List<Node>();

            for (int i = 0; i < markedNodes.Count; i++) {
                List<Node> unmarkedAdjacentNodes = GetUnmarkedAdjacentNodes(markedNodes[i]);
                for (int j = 0; j < unmarkedAdjacentNodes.Count; j++) {
                    m_TertiaryPaths.Add(ManhattanPath(markedNodes[i], unmarkedAdjacentNodes[j]));
                    if (!overlappingBranches) {
                        unmarkedAdjacentNodes[j].Mark();
                    }
                    else {
                        markingQueue.Add(unmarkedAdjacentNodes[j]);
                    }
                }
            }

            for (int i = 0; i < markingQueue.Count; i++) {
                markingQueue[i].Mark();
            }

            markedNodes = GetAllMarkedNodes();
            depth += 1;

        }

    }

    private List<Node> GetAllMarkedNodes() {
        List<Node> markedNodes = new List<Node>();
        for (int i = 0; i < m_Array.Length; i++) {
            for (int j = 0; j < m_Array[i].Length; j++) {
                if (m_Array[i][j].Marked) {
                    markedNodes.Add(m_Array[i][j]);
                }
            }
        }

        return markedNodes;
    }

    private List<Node> GetUnmarkedAdjacentNodes(Node node) {
        List<Node> adjacentNodes = GetAdjacentNodes(node);
        List<Node> unmarkedAdjacentNodes = new List<Node>();
        for (int j = 0; j < adjacentNodes.Count; j++) {
            if (!adjacentNodes[j].Marked) {
                unmarkedAdjacentNodes.Add(adjacentNodes[j]);
            }
        }
        return unmarkedAdjacentNodes;
    }

    public List<Node> ManhattanPath(Node nodeA, Node nodeB) {
        List<Node> path = new List<Node>();
        path.Add(nodeA);

        Node node = nodeA;
        int distance = 0;
        int maxDistance = m_Array.Length * m_Array[0].Length;
        while (node != nodeB && distance < maxDistance) {
            List<Node> adjacentNodes = GetAdjacentNodes(node);

            float minDistance = maxDistance;
            for (int i = 0; i < adjacentNodes.Count; i++) {
                float tempDistance = GetManhattanDistance(adjacentNodes[i], nodeB);
                if (tempDistance < minDistance) {
                    node = adjacentNodes[i];
                    minDistance = tempDistance;
                }
            }
            distance += 1;
            path.Add(node);
        }

        return path;
    }

    public void Connect() {
        ResetAllConnections();
        ConnectPath(m_PrimaryPath);
        if (m_SecondaryPaths != null) {
            for (int i = 0; i < m_SecondaryPaths.Length; i++) {
                ConnectPath(m_SecondaryPaths[i]);
            }
        }
        if (m_TertiaryPaths != null) {
            for (int i = 0; i < m_TertiaryPaths.Count; i++) {
                ConnectPath(m_TertiaryPaths[i]);
            }
        }
    }

    public void ConnectPath(List<Node> path) {
        if (path == null || path.Count < 2) {
            Debug.Log("Invalid Path");
            return;
        }

        for (int i = 1; i < path.Count; i++) {
            Vector2 direction = (Vector2)(path[i].Position - path[i - 1].Position);
            Connection connection = GetVectorConnection(direction);
            path[i - 1].AddConnection(connection);
        }
    }

    public void ResetAllConnections() {
        int height = m_Array.Length;
        int width = m_Array[0].Length;
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                m_Array[i][j].ResetConnections();
            }
        }
    }

    public List<Node> GetAdjacentNodes(Node nodeA) {

        int height = m_Array.Length;
        int width = m_Array[0].Length;
        Vector2Int position = nodeA.Position;

        List<Node> adjacentNodes = new List<Node>();
        if (position.x >= 1) {
            adjacentNodes.Add(m_Array[position.y][position.x - 1]);
        }
        if (position.x < width - 1) {
            adjacentNodes.Add(m_Array[position.y][position.x + 1]);
        }
        if (position.y >= 1) {
            adjacentNodes.Add(m_Array[position.y - 1][position.x]);
        }
        if (position.y < height - 1) {
            adjacentNodes.Add(m_Array[position.y + 1][position.x]);
        }
        return adjacentNodes;
    }

    public static int GetManhattanDistance(Node nodeA, Node nodeB) {
        int horizontalDistance = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int verticalDistance = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
        return horizontalDistance + verticalDistance;
    }

    public static Vector2 GetConnectionVector(Connection connection) {
        switch (connection) {
            case Connection.Right:
                return Vector2.right;
            case Connection.Up:
                return Vector2.up;
            case Connection.Left:
                return Vector2.left;
            case Connection.Down:
                return Vector2.down;
            default:
                return Vector2.zero;
        }
    }

    public static Connection GetVectorConnection(Vector2 vector) {
        if (vector == Vector2.right) {
            return Connection.Right;
        }
        else if (vector == Vector2.up) {
            return Connection.Up;
        }
        else if (vector == Vector2.left) {
            return Connection.Left;
        }
        else if (vector == Vector2.down) {
            return Connection.Down;
        }
        return Connection.None;
    }

}
