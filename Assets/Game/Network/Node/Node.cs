using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node {



    /* --- Variables --- */
    #region Variables

    // Position.
    private Vector2Int m_Position;
    public Vector2Int Position => m_Position;
    // Marking.
    private bool m_Marked;
    public bool Marked => m_Marked;
    // Connections.
    private List<NodeLink> m_Links;
    public List<NodeLink> Links => m_Links;

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public Node(int x, int y) {
        m_Position = new Vector2Int(x, y);
        m_Links = new List<NodeLink>();
    }

    #endregion

    #region Marking

    public void Mark() {
        m_Marked = true;
    }

    public void Unmark() {
        m_Marked = false;
    }

    #endregion

    #region Linking

    public static void Link(Node nodeA, Node nodeB) {
        Vector2 direction = (Vector2)(nodeB.Position - nodeA.Position);
        NodeLink connection = VectorToLink(direction);
        nodeA.AddLink(connection);
    }

    public void AddLink(NodeLink connection) {
        if (connection == NodeLink.None) {
            return;
        }
        if (!m_Links.Contains(connection)) {
            m_Links.Add(connection);
        }
    }

    public void ResetLinks() {
        m_Links = new List<NodeLink>();
    }

    public static Vector2 LinkToVector(NodeLink connection) {
        switch (connection) {
            case NodeLink.Right:
                return Vector2.right;
            case NodeLink.Up:
                return Vector2.up;
            case NodeLink.Left:
                return Vector2.left;
            case NodeLink.Down:
                return Vector2.down;
            default:
                return Vector2.zero;
        }
    }

    public static NodeLink VectorToLink(Vector2 vector) {
        if (vector == Vector2.right) {
            return NodeLink.Right;
        }
        else if (vector == Vector2.up) {
            return NodeLink.Up;
        }
        else if (vector == Vector2.left) {
            return NodeLink.Left;
        }
        else if (vector == Vector2.down) {
            return NodeLink.Down;
        }
        return NodeLink.None;
    }

    #endregion

}
