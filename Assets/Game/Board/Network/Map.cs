/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Map : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Network.
    [SerializeField] private int m_Seed;
    [SerializeField] private NetworkSettings m_Settings;
    [HideInInspector] private Network m_Network;

    // Board.
    [SerializeField] private Node m_CurrNode;
    [SerializeField] private Board m_Board;
    [SerializeField] private Environment m_Environment;

    // Resetting.
    [SerializeField] private Background m_Background;
    public bool generate = false;

    public int m_DifficultyOffset = 0;


    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        // Generate();
    }

    void Update() {
        if (generate) {
            Generate();
            generate = false;
        }
    }

    #endregion

    /* --- Board Generation --- */
    #region Board Generation

    private void Generate() {
        m_Network = new Network(m_Settings, true);
        m_CurrNode = m_Network.Entrance;
        GenerateBoard(new Vector2Int(0, 0));
    }

    public static void MapMove(Vector2Int direction) {
        Map instance = (Map)GameObject.FindObjectOfType(typeof(Map));
        instance.Move(direction);
    }

    public void Move(Vector2Int direction) {
        Vector2Int target = m_CurrNode.Position + direction;
        m_CurrNode = m_Network.GetNodeAt(target);
        GenerateBoard(direction);
    }

    private void GenerateBoard(Vector2Int direction) {
        if (m_CurrNode == null) { return; }

        List<NodeLink> links = m_Network.GetAllLinks(m_CurrNode);
        int depth = m_CurrNode.Position.y * m_Settings.Height + m_CurrNode.Position.x;
        int difficulty = m_CurrNode.Position.x + m_DifficultyOffset;

        m_Board.Reset();
        m_Background.Close();

        StartCoroutine(IEDelayedGeneration(depth, links, direction, difficulty));
    }

    private IEnumerator IEDelayedGeneration(int depth, List<NodeLink> links, Vector2Int direction, int difficulty) {
        yield return new WaitForSeconds(m_Background.GetCloseDelay() + 0.2f);
        
        SoundController.PlaySound(SoundController.BoardGenerate, 1);
        m_Board.GenerateMap();
        m_Background.Open();

        yield return new WaitForSeconds(m_Background.GetOpenDelay());

        m_Board.Generate(depth, links, direction, difficulty);

        Time.timeScale = 2f;
        yield return new WaitForSeconds(2.5f);
        Time.timeScale = 1.5f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1.25f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;

    }

    #endregion

    /* --- Debug --- */
    #region Debug

    void OnDrawGizmos() {
        if (m_Network?.Array == null) { return; }
        Node[][] array = m_Network.Array;

        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array[i].Length; j++) {

                // Circle.
                Vector3 position = (Vector3)(Vector2)array[i][j].Position + transform.localPosition;
                Gizmos.color = GetColor(array[i][j]);
                Gizmos.DrawWireSphere(position, 0.35f);

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
