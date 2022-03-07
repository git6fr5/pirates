/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NetworkDebugger : MonoBehaviour {

    public int m_Seed;

    [SerializeField] private NetworkSettings m_Settings;
    private Network m_Network;

    [SerializeField] private Node m_CurrNode;

    [SerializeField] private Board m_Board;
    [SerializeField] private Environment m_Environment;

    public float m_NodeRadius;

    public bool reset;
    public bool generate;

    void Start() {
        // Random.seed = m_Seed; // hmmm.
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

    }

    private void Reset() {
        m_Network = new Network(m_Settings, false);
    }

    private void Generate() {
        m_Network = new Network(m_Settings, true);
        m_CurrNode = m_Network.Entrance;
        ResetBoard();
    }

    #region Board Generation

    public void Move(Vector2Int direction) {
        Vector2Int target = m_CurrNode.Position + direction;
        bool verticalBounds = target.y >= 0 && target.y < m_Network.Array.Length;
        bool horizontalBounds = target.x >= 0 && target.x < m_Network.Array[0].Length;
        if (verticalBounds && horizontalBounds) {
            m_CurrNode = m_Network.Array[target.y][target.x];

            m_Environment.SetPlayer(m_Board.Get<Player>());
            m_Environment.m_PlayerStartPosition = m_Board.Get<Player>().Position - new Vector2Int((m_Board.Width - 1) * direction.x, (m_Board.Height - 1)* direction.y);
            ResetBoard();
        }
    }

    private void ResetBoard() {
        int depth = m_CurrNode.Position.y * m_Settings.Height + m_CurrNode.Position.x;
        m_Board.SetDepth(depth);

        m_Board.Reset();
        m_Board.Init();
        List<NodeLink> links = GetAllLinks();
        m_Board.AddExits(links);
    }

    private List<NodeLink> GetAllLinks() {
        List<NodeLink> links = new List<NodeLink>();

        for (int i = 0; i < m_CurrNode.Links.Count; i++) {
            links.Add(m_CurrNode.Links[i]);
        }

        int height = m_Network.Array.Length;
        int width = m_Network.Array[0].Length;
        CheckForLinkInAdjacentNode(height, width, m_CurrNode.Position, new Vector2Int(1, 0), ref links);
        CheckForLinkInAdjacentNode(height, width, m_CurrNode.Position, new Vector2Int(-1, 0), ref links);
        CheckForLinkInAdjacentNode(height, width, m_CurrNode.Position, new Vector2Int(0, 1), ref links);
        CheckForLinkInAdjacentNode(height, width, m_CurrNode.Position, new Vector2Int(0, -1), ref links);
        return links;
    }

    private void CheckForLinkInAdjacentNode(int height, int width, Vector2Int position, Vector2Int direction, ref List<NodeLink> links) {
        Vector2Int target = position + direction;

        NodeLink fromLink = Node.VectorIntToLink(-direction);
        NodeLink toLink = Node.VectorIntToLink(direction);

        if (target.x >= 0 && target.x < width && target.y >= 0 && target.y < height) {
            if (m_Network.Array[target.y][target.x].Links.Contains(fromLink)) {
                if (!links.Contains(toLink)) {
                    links.Add(toLink);
                }
            }
        }
    }

    #endregion

    #region Debug

    void OnDrawGizmos() {
        if (m_Network?.Array == null) { return; }
        Node[][] array = m_Network.Array;

        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array[i].Length; j++) {
                // Circle.
                Vector3 position = (Vector3)(Vector2)array[i][j].Position + transform.localPosition;
                Gizmos.color = GetColor(array[i][j]);
                Gizmos.DrawWireSphere(position, m_NodeRadius);
                // Lines.
                Gizmos.color = Color.blue;
                foreach (NodeLink link in array[i][j].Links) {
                    Gizmos.DrawLine(position, position + (Vector3)Node.LinkToVector(link));
                }

            }
        }
    }

    private Color GetColor(Node node) {

        if (node == m_CurrNode) {
            return Color.white;
        }

        // Primary Path.
        if (m_Network.PrimaryPath != null && m_Network.PrimaryPath.Contains(node)) {
            return Color.red;
        }

        // Secondary Path.
        if (m_Network.SecondaryPaths != null) {
            for (int i = 0; i < m_Network.SecondaryPaths.Length; i++) {
                if (m_Network.SecondaryPaths[i].Contains(node)) {
                    return Color.green;
                }
            }
        }

        // Tertiary Path.
        if (m_Network.TertiaryPaths != null) {
            for (int i = 0; i < m_Network.TertiaryPaths.Count; i++) {
                if (m_Network.TertiaryPaths[i].Contains(node)) {
                    return Color.blue;
                }
            }
        }

        // Marking.
        if (node.Marked) {
            return Color.cyan;
        }

        // Default.
        return Color.white;
    }

    #endregion



}
