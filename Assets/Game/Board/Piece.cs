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

    // Health.
    [SerializeField] protected int m_MaxHearts;
    [SerializeField] protected int m_Hearts;
    public int Hearts => m_Hearts;
    public bool TookDamage = false;

    // Position.
    protected Vector2Int m_Position;
    public Vector2Int Position => m_Position;

    // Color.
    [SerializeField] protected Color m_DebugColor = Color.white;
    public Color DebugColor => m_DebugColor;

    // Shake.
    [HideInInspector] protected bool m_Shake;
    [SerializeField] protected float m_ShakeStrength;
    [SerializeField] protected float m_ShakeDuration;
    [HideInInspector] private float m_ShakeTicks;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Update() {
        // m_TookDamage = false;
        if (m_Shake) {
            m_Shake = WhileShake();
        }
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
            Snap();
        }
    }

    #endregion

    /* --- Region --- */
    #region Actions

    protected virtual void Think() {
    }

    public void Snap() {
        transform.localPosition = (Vector3)(Vector2)m_Position;
    }

    public void Move(float deltaTime) {
        if (m_Board == null) {
            return;
        }

        Vector2 displacement = (Vector2)m_Position - (Vector2)transform.localPosition;
        if (displacement.sqrMagnitude > 0.05f * 0.05f) {
            transform.position += (Vector3)(displacement.normalized) / m_Board.TurnDelay * deltaTime;
        }
    }

    #endregion

    #region Health
    /* --- Health --- */

    public void TakeDamage(int damage, float delay = 0f) {
        m_Hearts -= damage;
        CameraShake.ActivateShake(m_Board.TurnDelay * 2f);

        CheckDeath(delay);
    }

    public void Heal(int health) {
        m_Hearts += health;
        if (m_Hearts > m_MaxHearts) {
            m_Hearts = m_MaxHearts;
        }
    }

    private void CheckDeath(float delay) {
        bool die = m_Hearts <= 0 && this != null;
        StartCoroutine(IEDeathDelay(delay, die));
    }

    private IEnumerator IEDeathDelay(float delay, bool die) {
        yield return new WaitForSeconds(delay);
        TookDamage = true;
        if (die) {
            Die();
        }
    }

    private void Die() {
        if (gameObject != null) {
            Destroy(gameObject);
        }
    }

    #endregion

    /* --- Shaking --- */
    #region Shaking

    public bool WhileShake() {
        transform.position = (Vector3)(Vector2)m_Position;

        m_ShakeTicks += Time.deltaTime;
        if (m_ShakeTicks >= m_ShakeDuration) {
            m_ShakeTicks = 0f;
            return false;
        }
        transform.position += (Vector3)Random.insideUnitCircle * m_ShakeStrength;
        return true;
    }

    public void StartShake(float delay, float duration) {
        if (delay == 0) {
            m_ShakeDuration = duration;
            m_Shake = true;
        }
        else {
            StartCoroutine(IEShakeDelay(delay, duration));
        }
    }

    private IEnumerator IEShakeDelay(float delay, float duration) {
        yield return new WaitForSeconds(delay);
        if (this != null && gameObject != null) {
            m_ShakeDuration = duration;
            m_Shake = true;
        }
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
