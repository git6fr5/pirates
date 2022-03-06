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

    public GameObject m_Link;
    public List<GameObject> m_Links;

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

            if (m_Links != null) {
                for (int i = 0; i < m_Links.Count; i++) {
                    GameObject linkObject = m_Links[i];
                    Destroy(linkObject);
                }
            }
            
            int depth = m_CurrNode.Position.y * m_Settings.Height + m_CurrNode.Position.x;
            m_Board.SetDepth(depth);

            m_Board.Reset();
            m_Board.Init();

            m_Links = new List<GameObject>();
            for (int i = 0; i < m_CurrNode.Links.Count; i++) {
                GameObject newLink = Instantiate(m_Link, transform.position, Quaternion.identity, null);
                Vector2 v = Node.LinkToVector(m_CurrNode.Links[i]);
                v.x = (v.x + 1f) / 2f;
                v.y = (v.y + 1f) / 2f;
                newLink.transform.position = new Vector3(m_Board.Width * v.x - 0.5f, m_Board.Height * v.y - 0.5f, 0f);
                m_Links.Add(newLink);
            }
            
            generate = false;
        }

    }

    private void Reset() {
        m_Network = new Network(m_Settings, false);
    }

    private void Generate() {
        m_Network = new Network(m_Settings, true);
        m_CurrNode = m_Network.Entrance;
    }

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

}
