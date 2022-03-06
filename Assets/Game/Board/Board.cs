using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Board : MonoBehaviour {

    /* --- Data --- */
    #region Data

    [System.Serializable]
    public class PieceData {
        public Piece m_Piece;
        public Vector2Int m_Position;

        public PieceData(Piece piece, Vector2Int position) {
            m_Piece = piece;
            m_Position = position;
        }
    }

    #endregion

    /* --- Variables --- */
    #region Variables

    // Generation.
    public bool m_Reset;
    public int m_Depth;

    // Settings.
    public int m_Width;
    public int Width => m_Width;
    public int m_Height;
    public int Height => m_Height;

    // Pieces.
    public Coroutine m_GameLoop;
    public LDtkReader m_LDtkReader;
    public PieceData[] m_PieceData;
    public List<Piece> m_Pieces;
    public List<Piece> Pieces => m_Pieces;

    // Loop.
    public Character[] m_Characters;
    public int m_TurnNumber;
    public int m_MaxTurnNumber;
    public int m_RoundNumber;
    public float m_TurnDelay;
    public float TurnDelay => m_TurnDelay;

    // UI.
    public Tilemap m_Background;
    public TileBase m_BackgroundTile;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    void Update() {
        if (m_Reset) {
            Reset();
            Init();
            m_Reset = false;
        }
    }

    public void Reset() {
        StopCoroutine(m_GameLoop);
        for (int i = 0; i < m_Pieces.Count; i++) {
            Destroy(m_Pieces[i].gameObject);
        }
        for (int i = 0; i < m_Height; i++) {
            for (int j = 0; j < m_Width; j++) {
                m_Background.SetTile(new Vector3Int(j, i, 0), null);
            }
        }
    }

    public void Init() {
        // Pieces.
        m_PieceData = m_LDtkReader.Get(m_Depth);
        m_Pieces = new List<Piece>();
        for (int i = 0; i < m_PieceData.Length; i++) {
            Piece newPiece = m_PieceData[i].m_Piece.Create(this, m_PieceData[i].m_Position.x, m_PieceData[i].m_Position.y);
            newPiece.SetPosition(m_PieceData[i].m_Position, true);
            m_Pieces.Add(newPiece);
        }

        // Loop.
        m_Characters = GetAll<Character>();
        m_MaxTurnNumber = m_Characters.Length;
        m_GameLoop = StartCoroutine(IEGameLoop());

        // Background.
        for (int i = 0; i < m_Height; i++) {
            for (int j = 0; j < m_Width; j++) {
                m_Background.SetTile(new Vector3Int(j, i, 0), m_BackgroundTile);
            }
        }
    }

    #endregion

    /* --- Generics --- */
    #region Generics

    public static Board FindInstance() {
        return (Board)GameObject.FindObjectOfType(typeof(Board));
    }

    public T[] GetAll<T>() {
        List<T> newList = new List<T>();
        for (int i = 0; i < m_Pieces.Count; i++) {
            if (m_Pieces[i].GetComponent<T>() != null) {
                newList.Add(m_Pieces[i].GetComponent<T>());
            }
        }
        return newList.ToArray();
    }

    public T Get<T>() {
        Piece piece = m_Pieces.Find(piece => piece != null && piece.GetComponent<T>() != null);
        if (piece != null) {
            return piece.GetComponent<T>();
        }
        return default(T);
    }

    public T GetAt<T>(Vector2Int position) {
        Piece piece = m_Pieces.Find(piece => piece != null && piece.Position == position);
        if (piece != null) {
            return piece.GetComponent<T>();
        }
        return default(T);
    }

    #endregion

    private IEnumerator IEGameLoop() {
        while (true) {
            for (int i = 0; i < m_Characters.Length; i++) {
                m_TurnNumber = i;
                m_Characters[i].NewTurn();
                yield return new WaitUntil(() => m_Characters[i] == null || (m_Characters[i] != null && m_Characters[i].CompletedTurn));
                if (m_Characters[i] != null && !m_Characters[i].IsStatic) {
                    // yield return new WaitForSeconds(m_TurnDelay);
                    // yield return new WaitForSeconds(m_TurnDelay / 10f);
                }
            }
            // yield return new WaitForSeconds(m_TurnDelay / 2f);
            m_RoundNumber += 1;
        }
    }

    public bool CheckMove(Vector2Int origin, Vector2Int direction) {

        Vector2Int target = origin + direction;
        bool horizontalBoundCheck = target.x >= 0 && target.x < m_Width;
        bool verticalBoundCheck = target.y >= 0 && target.y < m_Height;
        if (!horizontalBoundCheck || !verticalBoundCheck) {
            Debug.Log("Trying to move out of bounds.");
            return false;
        }

        bool collisionCheck = m_Pieces.Find(piece => piece.Position == target) != null;
        if (!collisionCheck) {
            return true;
        }
        else {
            Debug.Log("Trying to move to an occupied square.");
            return false;
        }

    }

    public bool CheckTarget(Vector2Int origin, Vector2Int direction) {

        Vector2Int target = origin + direction;
        bool horizontalBoundCheck = target.x >= 0 && target.x < m_Width;
        bool verticalBoundCheck = target.y >= 0 && target.y < m_Height;
        if (!horizontalBoundCheck || !verticalBoundCheck) {
            Debug.Log("Trying to move out of bounds.");
            return false;
        }
        return true;
    }

    public bool Move(Piece piece, Vector2Int direction) {
        if (CheckMove(piece.Position, direction)) {
            piece.SetPosition(piece.Position + direction);
            return true;
        }
        return false;
    }

    public List<Vector2Int> AdjacentPositions(Vector2Int origin, int depth, ref List<Vector2Int> positions) {
        
        List<Vector2Int> directions = new List<Vector2Int>() {
            Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down
        };

        for (int i = 0; i < directions.Count; i++) {
            if (CheckTarget(origin, directions[i])) {
                positions.Add(origin + directions[i]);
            }
        }

        //if (depth > 0) {
        //    int newDepth = depth - 1;
        //    for (int i = 0; i < adjacentPositions.Count; i++) {
        //        AdjacentPositions(adjacentPositions[i], newDepth, ref adjacentPositions);
        //    }
        //}

        return positions;
    }

    public List<Vector2Int> OctaDirectional(Vector2Int origin, int depth, ref List<Vector2Int> positions) {

        List<Vector2Int> directions = new List<Vector2Int>() {
            Vector2Int.right, Vector2Int.right + Vector2Int.up,
            Vector2Int.up, Vector2Int.up + Vector2Int.left,
            Vector2Int.left, Vector2Int.left + Vector2Int.down,
            Vector2Int.down, Vector2Int.down + Vector2Int.right
        };

        List<int> brokenPaths = new List<int>();
        for (int i = 1; i <= depth; i++) {
            for (int j = 0; j < directions.Count; j++) {
                if (!brokenPaths.Contains(j) && CheckTarget(origin, i * directions[j])) {
                    positions.Add(origin + i * directions[j]);
                    if (GetAt<Piece>(origin + i * directions[j])) {
                        brokenPaths.Add(j);
                    }
                }
            }
        }

        return positions;
    }

    public List<Vector2Int> AllWithinRadius(Vector2Int origin, int range, ref List<Vector2Int> positions) {
        for (int i = -range; i <= range; i++) {
            for (int j = -range; j <= range; j++) {
                if (CheckTarget(origin, new Vector2Int(j, i))) {
                    positions.Add(origin + new Vector2Int(j, i));
                }
            }
        }
        return positions;
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
            List<Vector2Int> allMinMovements = new List<Vector2Int>();
            for (int i = 0; i < possibleMovements.Count; i++) {
                float tempDistance = GetManhattanDistance(possibleMovements[i], endPosition);
                if (tempDistance == minDistance) {
                    allMinMovements.Add(possibleMovements[i]);
                }
                if (tempDistance < minDistance) {
                    position = possibleMovements[i];
                    allMinMovements = new List<Vector2Int>();
                    allMinMovements.Add(position);
                    minDistance = tempDistance;
                }
            }

            if (allMinMovements.Count > 1) {
                position = allMinMovements[Random.Range(0, allMinMovements.Count)];
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
        // Vector3 offset = new Vector3(0.5f, 0.5f, 0f);
        for (int i = 0; i < m_Height; i++) {
            for (int j = 0; j < m_Width; j++) {
                Gizmos.DrawWireCube(new Vector3(j, i, 0), new Vector3(1, 1, 1));
            }
        }
    }

}
