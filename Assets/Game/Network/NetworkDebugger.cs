/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NetworkDebugger : MonoBehaviour {

    [SerializeField] private NetworkSettings m_Settings;
    private Network m_Network;

    public float m_NodeRadius;

    public bool reset;
    public bool generate;

    void Start() {
        
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
    }

    void OnDrawGizmos() {
        if (m_Network?.Array == null) { return; }
        Node[][] array = m_Network.Array;

        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array[i].Length; j++) {
                // Circle.
                Vector3 position = (Vector3)(Vector2)array[i][j].Position;
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
