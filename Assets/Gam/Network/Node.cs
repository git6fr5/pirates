using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node {

    private Vector2Int m_Position;
    public Vector2Int Position => m_Position;

    private List<Connection> m_Connections;
    public List<Connection> Connections => m_Connections;

    private bool m_Marked;
    public bool Marked => m_Marked;


    public Node(int x, int y) {
        m_Position = new Vector2Int(x, y);
        m_Connections = new List<Connection>();
    }

    public void Mark() {
        m_Marked = true;
    }

    public void Unmark() {
        m_Marked = false;
    }

    public void AddConnection(Connection connection) {
        if (connection == Connection.None) {
            return;
        }
        if (!m_Connections.Contains(connection)) {
            m_Connections.Add(connection);
        }
    }

    public void ResetConnections() {
        m_Connections = new List<Connection>();
    }

}
