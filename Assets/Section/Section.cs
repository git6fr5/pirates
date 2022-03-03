using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Section : MonoBehaviour {

    private Node m_Node;

    public Piece[][] m_Array;
    public Piece[][] Array => m_Array;

    public int m_Width;
    public int m_Height;

    [System.Serializable]
    public class PieceData {
        public Character m_Piece;
        public Vector2Int m_Position;
    }
    public PieceData[] m_PieceData;

    void Start() {
        m_Array = new Piece[m_Height][];
        for (int i = 0; i < m_Height; i++) {
            m_Array[i] = new Piece[m_Width];
        }

        for (int i = 0; i < m_PieceData.Length; i++) {
            m_Array[m_PieceData[i].m_Position.y][m_PieceData[i].m_Position.x] = m_PieceData[i].m_Piece.Create(this, m_PieceData[i].m_Position.x, m_PieceData[i].m_Position.y);
            m_Array[m_PieceData[i].m_Position.y][m_PieceData[i].m_Position.x].SetPosition(m_PieceData[i].m_Position);
        }
    }

    public bool CheckMove(Vector2Int origin, Vector2Int direction) {

        Vector2Int target = origin + direction;
        bool horizontalBoundCheck = target.x >= 0 && target.x < m_Width;
        bool verticalBoundCheck = target.y >= 0 && target.y < m_Height;
        if (!horizontalBoundCheck || !verticalBoundCheck) {
            return false;
        }

        bool collisionCheck = m_Array[target.y][target.x] != null;
        if (!collisionCheck) {
            return true;
        }
        else {
            return false;
        }

    }

    public bool Move(Piece piece, Vector2Int direction) {

        bool validPiece = piece == m_Array[piece.Position.y][piece.Position.x];
        if (!validPiece) {
            return false;
        }

        Vector2Int origin = piece.Position;
        if (CheckMove(origin, direction)) {
            Vector2Int targetPosition = piece.Position + direction;
            m_Array[piece.Position.y][piece.Position.x] = null;
            m_Array[targetPosition.y][targetPosition.x] = piece;
            piece.SetPosition(targetPosition);
            return true;
        }
        return false;
    }

    public bool WithinRadius(Piece pieceA, Piece pieceB, int radius) {
        int horizontalDistance = Mathf.Abs(pieceA.Position.x - pieceB.Position.x);
        int verticalDistance = Mathf.Abs(pieceA.Position.y - pieceB.Position.y);
        return Mathf.Max(horizontalDistance, verticalDistance) <= radius;
    }

    public List<Vector2Int> ManhattanPath(Vector2Int startPosition, Vector2Int endPosition) {
        List<Vector2Int> path = new List<Vector2Int>();

        List<Vector2Int> movements = new List<Vector2Int>() {
            Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down
        };

        int distance = 0;
        int maxDistance = m_Height * m_Width;

        Vector2Int position = startPosition;
        path.Add(position);

        while (position != endPosition && distance < maxDistance) {
            List<Vector2Int> possibleMovements = new List<Vector2Int>();
            for (int i = 0; i < movements.Count; i++) {
                if (CheckMove(position, movements[i])) {
                    possibleMovements.Add(position + movements[i]);
                }
            }

            float minDistance = maxDistance;
            for (int i = 0; i < possibleMovements.Count; i++) {
                float tempDistance = GetManhattanDistance(possibleMovements[i], endPosition);
                if (tempDistance < minDistance) {
                    position = possibleMovements[i];
                    minDistance = tempDistance;
                }
            }
            distance += 1;
            path.Add(position);
        }

        return path;
    }

    public static int GetManhattanDistance(Vector2Int origin, Vector2Int end) {
        int horizontalDistance = Mathf.Abs(origin.x - end.x);
        int verticalDistance = Mathf.Abs(origin.y - end.y);
        return horizontalDistance + verticalDistance;
    }

    void OnDrawGizmos() {
        for (int i = 0; i < m_Height - 1; i++) {
            for (int j = 0; j < m_Width - 1; j++) {
                Gizmos.DrawLine(new Vector3(j, i, 0), new Vector3(j + 1, i, 0));
                Gizmos.DrawLine(new Vector3(j, i, 0), new Vector3(j, i + 1, 0));
                Gizmos.DrawLine(new Vector3(j + 1, i, 0), new Vector3(j + 1, i + 1, 0));
                Gizmos.DrawLine(new Vector3(j, i + 1, 0), new Vector3(j + 1, i + 1, 0));
            }
        }
    }

}
