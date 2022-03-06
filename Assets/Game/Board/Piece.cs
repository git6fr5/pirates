/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Piece : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Board.
    protected Board m_Board;
    // Position.
    protected Vector2Int m_Position;
    public Vector2Int Position => m_Position;
    // Color.
    [SerializeField] protected Color m_DebugColor = Color.white;
    public Color DebugColor => m_DebugColor;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Ew.
    void Awake() {
        Snap();
    }

    void Update() {
        Think();
        float deltaTime = Time.deltaTime;
        Move(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public Piece Create(Board board, int x, int y) {
        Piece newPiece = Instantiate(gameObject, transform.position, Quaternion.identity, board.transform).GetComponent<Piece>();
        newPiece.SetBoard(board);
        newPiece.SetPosition(new Vector2Int(x, y));
        newPiece.gameObject.SetActive(true);
        return newPiece;
    }

    public void SetBoard(Board board) {
        m_Board = board;
    }

    public void SetPosition(Vector2Int position, bool snapToPosition = false) {
        m_Position = position;
        if (snapToPosition) {
            transform.position = (Vector3)(Vector2)position;
        }
    }

    #endregion

    /* --- Region --- */
    #region Actions

    public void Snap() {
        transform.position = (Vector3)(Vector2)m_Position;
    }

    public void Move(float deltaTime) {
        if (m_Board == null) {
            return;
        }

        Vector2 displacement = (Vector2)m_Position - (Vector2)transform.position;
        if (displacement.sqrMagnitude > 0.05f * 0.05f) {
            transform.position += (Vector3)(displacement.normalized) / m_Board.TurnDelay * deltaTime;
        }
    }

    protected virtual void Think() {
    }

    #endregion

    /* --- Debug --- */
    #region Debug

    void OnDrawGizmos() {
        Draw();
    }

    protected virtual void Draw() {
        Gizmos.color = m_DebugColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    #endregion

}
