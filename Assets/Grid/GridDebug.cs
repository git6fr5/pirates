using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebug : MonoBehaviour {

    private Grid m_Grid;
    public int m_Width;
    public int m_Height;

    public bool mouseInput;
    private Vector2 m_MousePosition;
    public float m_NodeRadius;

    private Node[] m_PathControl;

    private List<Node> m_PrimaryPath;
    private List<Node>[] m_SecondaryPaths;
    private List<List<Node>> m_TertiaryPaths;

    public int m_SecondaryBranchDepth;
    public int m_TertiaryBranchDepth;
    public bool m_OverlapSecondaryBranches;
    public bool m_OverlapTertiaryBranches;

    public bool debugAdjacency;
    public bool debugPath;

    public bool reset;
    public bool generate;
    public bool generatePrimary;
    public bool generateSecondary;
    public bool generateTertiary;



    void Start() {
        m_Grid = new Grid(m_Width, m_Height);
        m_PathControl = new Node[2];
        m_PrimaryPath = new List<Node>();
        m_SecondaryPaths = new List<Node>[0];
        m_TertiaryPaths = new List<List<Node>>();
    }

    public void SetGrid(Grid grid) {
        m_Grid = grid;
        m_PathControl[0] = m_Grid.Entrance;
        m_PathControl[1] = m_Grid.Exit;
        m_PrimaryPath = m_Grid.PrimaryPath;
        m_SecondaryPaths = m_Grid.SecondaryPaths;
        m_TertiaryPaths = m_Grid.TertiaryPaths;
        m_Grid.Connect();
        reset = false;
        generate = false;
        generatePrimary = false;
        generateSecondary = false;
        generateTertiary = false;
    }

    void Update() {
        if (reset) {
            Reset();
            reset = false;
        }

        if (generate) {
            Generate();
            generate = false;
        }

        if (generatePrimary) {
            GeneratePrimary();
            generatePrimary = false;
        }

        if (generateSecondary) {
            GenerateSecondary();
            generateSecondary = false;
        }

        if (generateTertiary) {
            GenerateTertiary();
            generateTertiary = false;
        }

        if (mouseInput) {
            MarkNodes();
            DrawPath();
            ConnectPath();
        }
    }

    private void Reset() {
        m_Grid = new Grid(m_Width, m_Height);
        m_PathControl = new Node[2];
        m_PrimaryPath = new List<Node>();
        m_SecondaryPaths = new List<Node>[0];
    }

    private void Generate() {
        Reset();
        m_Grid.Generate(m_SecondaryBranchDepth, m_OverlapSecondaryBranches, m_TertiaryBranchDepth, m_OverlapTertiaryBranches);
        m_PathControl[0] = m_Grid.Entrance;
        m_PathControl[1] = m_Grid.Exit;
        m_PrimaryPath = m_Grid.PrimaryPath;
        m_SecondaryPaths = m_Grid.SecondaryPaths;
        m_TertiaryPaths = m_Grid.TertiaryPaths;
        m_Grid.Connect();
    }

    private void GeneratePrimary() {
        m_Grid.GeneratePrimary();
        m_PathControl[0] = m_Grid.Entrance;
        m_PathControl[1] = m_Grid.Exit;
        m_PrimaryPath = m_Grid.PrimaryPath;
        m_Grid.Connect();
    }

    private void GenerateSecondary() {
        m_Grid.GenerateSecondary(m_SecondaryBranchDepth, m_OverlapSecondaryBranches);
        m_SecondaryPaths = m_Grid.SecondaryPaths;
        m_Grid.Connect();
    }

    private void GenerateTertiary() {
        m_Grid.GenerateTertiary(m_TertiaryBranchDepth, m_OverlapTertiaryBranches);
        m_TertiaryPaths = m_Grid.TertiaryPaths;
        m_Grid.Connect();
    }

    private void MarkNodes() {
        m_MousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool addToPath = Input.GetMouseButtonDown(0);
        Node[][] array = m_Grid.Array;
        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array[i].Length; j++) {
                if (IsMouseOver(array[i][j]) && addToPath) {
                    m_PathControl[0] = m_PathControl[1];
                    m_PathControl[1] = array[i][j];
                }
            }
        }
    }

    private void DrawPath() {
        bool drawPath = Input.GetKeyDown(KeyCode.Space);
        if (drawPath) {
            m_PrimaryPath = m_Grid.ManhattanPath(m_PathControl[0], m_PathControl[1]);
        }
    }

    private void ConnectPath() {
        bool connectPath = Input.GetKeyDown(KeyCode.C);
        if (connectPath) {
            m_Grid.Connect();
        }
    }

    void OnDrawGizmos() {
        if (m_Grid == null) { return; }

        Node[][] array = m_Grid.Array;
        if (array == null) { return; }

        float nodeRadius = 0f;
        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array[i].Length; j++) {
                Vector3 position = (Vector3)(Vector2)array[i][j].Position;

                Gizmos.color = GetColor(array[i][j], ref nodeRadius);
                Gizmos.DrawWireSphere(position, nodeRadius);

                Gizmos.color = Color.blue;
                foreach (Connection connection in array[i][j].Connections) {
                    Gizmos.DrawLine(position, position + (Vector3)Grid.GetConnectionVector(connection));
                }

            }
        }
    }

    private Color GetColor(Node node, ref float radius) {
        radius = m_NodeRadius;

        if (IsMouseOver(node)) {
            radius = m_NodeRadius * 2f;
            return Color.white;
        }
        
        if (node == m_PathControl[0] || node == m_PathControl[1]) {
            return Color.yellow;
        }
        
        if (m_Grid != null) {
            if (debugAdjacency) {
                bool isAdjacent = m_PathControl[0] != null && m_Grid.GetAdjacentNodes(m_PathControl[0]).Contains(node);
                isAdjacent = isAdjacent ? true : m_PathControl[1] != null && m_Grid.GetAdjacentNodes(m_PathControl[1]).Contains(node);
                if (isAdjacent) {
                    return Color.magenta;
                }
            }
            else if (debugPath) {
                if (m_PrimaryPath.Contains(node)) {
                    return Color.red;
                }
                if (m_SecondaryPaths != null) {
                    for (int i = 0; i < m_SecondaryPaths.Length; i++) {
                        if (m_SecondaryPaths[i].Contains(node)) {
                            return Color.green;
                        }
                    }
                }
                if (m_TertiaryPaths != null) {
                    for (int i = 0; i < m_TertiaryPaths.Count; i++) {
                        if (m_TertiaryPaths[i].Contains(node)) {
                            return Color.blue;
                        }
                    }
                }
            }
        }
        
        if (node.Marked) {
            return Color.cyan;
        }

        return Color.white;
    }

    private bool IsMouseOver(Node node) {
        return (m_MousePosition - (Vector2)node.Position).magnitude < m_NodeRadius;
    }
}
