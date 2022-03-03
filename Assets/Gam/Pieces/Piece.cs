using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    [SerializeField] protected Section m_Section;

    [SerializeField] protected Vector2Int m_Position;
    public Vector2Int Position => m_Position;

    [SerializeField] private Color m_PieceColor = Color.white;

    public Piece Create(Section section, int x, int y) {
        Piece newPiece = Instantiate(gameObject, transform.position, Quaternion.identity, section.transform).GetComponent<Piece>();
        newPiece.m_Section = section;
        newPiece.m_Position = new Vector2Int(x, y);
        newPiece.transform.position = (Vector3)(Vector2)m_Position;
        newPiece.gameObject.SetActive(true);
        return newPiece;
    }

    public void SetPosition(Vector2Int position) {
        m_Position = position;
        transform.position = (Vector3)(Vector2)m_Position;
    }

    protected virtual void OnDrawGizmos() {
        Gizmos.color = m_PieceColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

}
