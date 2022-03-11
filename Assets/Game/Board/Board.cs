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

    public static int Frames;
    public static float Ticks;

    // Generation.
    public LDtkReader m_LDtkReader;
    public PieceData[] m_PieceData;
    public List<Vector2Int> m_Exits;
    public bool m_Reset;
    public int m_Depth;

    // Settings.
    public int m_Width;
    public int Width => m_Width;
    public int m_Height;
    public int Height => m_Height;

    // Pieces.
    public List<Piece> m_Pieces;
    public List<Piece> Pieces => m_Pieces;
    [SerializeField] private Player m_PlayerBase;
    [SerializeField, ReadOnly] private Player m_Player;
    [SerializeField] private Vector2Int m_PlayerStartPosition;
    public Enemy[] m_Enemies;

    // Loop.
    public Coroutine m_GameLoop;
    public int m_TurnNumber;
    public int m_MaxTurnNumber;
    public int m_RoundNumber;
    public float m_TurnDelay;
    public float TurnDelay => m_TurnDelay;

    // Tilemap.
    public Tilemap m_Tilemap;
    public Tilemap m_Exitmap;
    public TileBase m_BackgroundTile;
    public TileBase m_ExitTile;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Awake() {
        m_Player = (Player)m_PlayerBase.Create(this, m_PlayerStartPosition.x, m_PlayerStartPosition.y);
        m_Player.SetPosition(m_PlayerStartPosition, true);
    }

    void Update() {
        Frames += 1;
        Ticks += Time.deltaTime;

    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Reset() {

        SoundController.PlaySound(SoundController.BoardReset, 1);

        // Stop the game loop.
        if (m_GameLoop != null) {
            StopCoroutine(m_GameLoop);
        }

        // Reset the pieces.
        for (int i = 0; i < m_Pieces.Count; i++) {
            if (m_Pieces[i] != null && m_Pieces[i] != m_Player) {
                Destroy(m_Pieces[i].gameObject);
            }
        }

        // Reset the exits.
        for (int i = 0; i < m_Exits.Count; i++) {
            m_Exitmap.SetTile(new Vector3Int(m_Exits[i].x, m_Exits[i].y, 0), null);
        }

        // Reset the tilemap.
        for (int i = -1; i < m_Height + 1; i++) {
            for (int j = -1; j < m_Width + 1; j++) {
                // m_Tilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
        }

        if (m_Player != null) {
            m_Player.gameObject.SetActive(false);
        }
    }

    public void GenerateMap() {
        // Draw the background.
        for (int i = -1; i < m_Height + 1; i++) {
            for (int j = -1; j < m_Width + 1; j++) {
                m_Tilemap.SetTile(new Vector3Int(j, i, 0), m_BackgroundTile);
            }
        }
    }

    public void Generate(int depth, List<NodeLink> nodeLinks, Vector2Int playerDirection, int diffiulty) {

        // Set the depth.
        m_Depth = depth;

        // Add the player.
        Vector2Int newPosition = m_Player.Position - new Vector2Int((m_Width - 1) * playerDirection.x, (m_Height - 1) * playerDirection.y);
        m_Player.SetPosition(newPosition, false);
        m_Player.gameObject.SetActive(true);
        m_Player.transform.position = (Vector3)(Vector2)m_Player.Position + (17.5f) * Vector3.up;
        m_Pieces = new List<Piece>();
        m_Pieces.Add(m_Player);

        // Generate all the other pieces.
        m_PieceData = m_LDtkReader.Get(m_Depth, diffiulty);
        for (int i = 0; i < m_PieceData.Length; i++) {
            Piece newPiece = m_PieceData[i].m_Piece.Create(this, m_PieceData[i].m_Position.x, m_PieceData[i].m_Position.y);
            newPiece.transform.position = (Vector3)(Vector2)m_PieceData[i].m_Position + (18f + i * 0.3f) * Vector3.up;
            // newPiece.SetPosition(m_PieceData[i].m_Position, true);
            m_Pieces.Add(newPiece);
            MakeSounds(18f + i * 0.3f, i % 2);
        }


        // Start the game loop.
        m_Enemies = GetAll<Enemy>();
        m_MaxTurnNumber = m_Enemies.Length;
        m_GameLoop = StartCoroutine(IEGameLoop());

        // Get the exits.
        m_Exits = new List<Vector2Int>();
        for (int i = 0; i < nodeLinks.Count; i++) {
            Vector2 direction = Node.LinkToVector(nodeLinks[i]);
            direction.x = direction.x == 1 ? m_Width : (direction.x == -1 ? -1 : 0);
            direction.y = direction.y == 1 ? m_Height : (direction.y == -1 ? -1 : 0);
            if (direction.x == 0) {
                for (int j = 0; j < 3; j++) {
                    m_Exits.Add(new Vector2Int((int)Mathf.Ceil((float)(m_Width - 1) / 2f), (int)direction.y + (int)Mathf.Sign(direction.y) * j));
                    m_Exits.Add(new Vector2Int((int)Mathf.Floor((float)(m_Width - 1) / 2f), (int)direction.y + (int)Mathf.Sign(direction.y) * j));
                }
            }
            else if (direction.y == 0) {
                for (int j = 0; j < 3; j++) {
                    m_Exits.Add(new Vector2Int((int)direction.x + (int)Mathf.Sign(direction.x) * j, (int)Mathf.Ceil((float)(m_Height - 1) / 2f)));
                    m_Exits.Add(new Vector2Int((int)direction.x + (int)Mathf.Sign(direction.x) * j, (int)Mathf.Floor((float)(m_Height - 1) / 2f)));
                }
            }
        }

        // Draw the exits.
        for (int i = 0; i < m_Exits.Count; i++) {
            m_Exitmap.SetTile(new Vector3Int(m_Exits[i].x, m_Exits[i].y, 0), m_ExitTile);
        }
    }

    private void MakeSounds(float fallDistance, int AorB) {
        float fallSpeed = 1f / TurnDelay;
        float soundDelay = (fallDistance - 1f) / fallSpeed;
        StartCoroutine(IEDelayedSound(soundDelay, AorB));
    }

    private IEnumerator IEDelayedSound(float soundDelay, int AorB) {
        yield return new WaitForSeconds(soundDelay);
        SoundController.PlaySound(SoundController.PieceLandSound, AorB);
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
            if (m_Pieces[i] != null && m_Pieces[i].GetComponent<T>() != null) {
                newList.Add(m_Pieces[i].GetComponent<T>());
            }
        }
        return newList.ToArray();
    }

    public Vector2Int[] GetAllLocations<T>() {
        List<Vector2Int> newList = new List<Vector2Int>();
        for (int i = 0; i < m_Pieces.Count; i++) {
            if (m_Pieces[i] != null && m_Pieces[i].GetComponent<T>() != null) {
                newList.Add(m_Pieces[i].Position);
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

    /* --- Game Loop --- */
    #region Game Loop

    private IEnumerator IEGameLoop() {
        while (true) {

            // Wait for the players turn to end.
            if (m_Player != null) {
                m_Player.NewTurn();
            }
            yield return new WaitUntil(() => m_Player == null || (m_Player != null && m_Player.CompletedTurn));

            SoundController.PlaySound(m_RoundNumber % 2 == 0 ? SoundController.Walking1 : SoundController.Walking2);

            // Run through the enemies turns.
            for (int i = 0; i < m_Enemies.Length; i++) {
                m_TurnNumber = i;
                if (m_Enemies[i] != null) {
                    m_Enemies[i].NewTurn();
                }
                yield return new WaitUntil(() => m_Enemies[i] == null || (m_Enemies[i] != null && m_Enemies[i].CompletedTurn));
                if (m_Enemies[i] != null) {
                    yield return new WaitForSeconds(m_TurnDelay / 10f);
                }
            }

            // Move to the next round.
            yield return new WaitForSeconds(m_TurnDelay / 2f);
            m_RoundNumber += 1;

            Spike[] spikes = GetAll<Spike>();
            for (int i = 0; i < spikes.Length; i++) {
                spikes[i].Swap();
            }

        }
    }

    public void AddPiece(Piece piece, Vector2Int location) {
        Piece newPiece = piece.Create(this, location.x, location.y);
        newPiece.transform.position = (Vector3)(Vector2)location + 18f * Vector3.up;
        m_Pieces.Add(newPiece);
    }

    #endregion

    /* --- Actions --- */
    #region Actions

    public bool Move(Piece piece, Vector2Int direction) {
        if (CheckMove(piece.Position, direction)) {
            piece.SetPosition(piece.Position + direction);
            return true;
        }
        return false;
    }

    public bool CheckMove(Vector2Int origin, Vector2Int direction) {

        Vector2Int target = origin + direction;
        if (m_Exits.Contains(target) && GetAt<Player>(origin) != null) {
            Map.MapMove(direction);
        }

        bool horizontalBoundCheck = target.x >= 0 && target.x < m_Width;
        bool verticalBoundCheck = target.y >= 0 && target.y < m_Height;
        if (!horizontalBoundCheck || !verticalBoundCheck) {
            return false;
        }

        bool collisionCheck = m_Pieces.Find(piece => piece.Position == target) != null;
        collisionCheck = collisionCheck && GetAt<Spike>(target) == null;
        if (!collisionCheck) {
            return true;
        }
        else {
            return false;
        }

    }

    public bool CheckTarget(Vector2Int origin, Vector2Int direction) {

        Vector2Int target = origin + direction;
        bool horizontalBoundCheck = target.x >= 0 && target.x < m_Width;
        bool verticalBoundCheck = target.y >= 0 && target.y < m_Height;
        if (!horizontalBoundCheck || !verticalBoundCheck) {
            return false;
        }
        return true;
    }

    #endregion

    /* --- Targetting --- */
    #region Targetting

    public List<Vector2Int> MeleeTargetting(Vector2Int origin, int depth, ref List<Vector2Int> positions) {
        
        List<Vector2Int> directions = new List<Vector2Int>() {
            Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down
        };

        for (int i = 0; i < directions.Count; i++) {
            if (CheckTarget(origin, directions[i])) {
                positions.Add(origin + directions[i]);
            }
        }
        return positions;
    }

    public List<Vector2Int> DirectionalTargetting(Vector2Int origin, int depth, ref List<Vector2Int> positions) {

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
                    if (GetAt<Piece>(origin + i * directions[j]) != null && GetAt<Spike>(origin + i * directions[j]) == null) {
                        brokenPaths.Add(j);
                    }
                }
            }
        }

        return positions;
    }

    public List<Vector2Int> ThrowTargetting(Vector2Int origin, int range, ref List<Vector2Int> positions) {
        for (int i = -range; i <= range; i++) {
            for (int j = -range; j <= range; j++) {
                if (CheckTarget(origin, new Vector2Int(j, i))) {
                    positions.Add(origin + new Vector2Int(j, i));
                }
            }
        }
        return positions;
    }

    public List<Vector2Int> AOETargetting(Vector2Int origin, int range, ref List<Vector2Int> positions) {
        for (int i = -range; i <= range; i++) {
            for (int j = -range; j <= range; j++) {
                if (CheckTarget(origin, new Vector2Int(j, i))) {
                    positions.Add(origin + new Vector2Int(j, i));
                }
            }
        }
        return positions;
    }

    public List<Vector2Int> GlobalTargetting(ref List<Vector2Int> positions) {
        for (int i = 0; i < m_Height; i++) {
            for (int j = 0; j < m_Width; j++) {
                positions.Add(new Vector2Int(j, i));
            }
        }
        return positions;
    }

    #endregion

    /* --- Enemy Pathing --- */
    #region Enemy Pathing

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

    #endregion

    /* --- Debug --- */
    #region Debug

    void OnDrawGizmos() {
        for (int i = 0; i < m_Height; i++) {
            for (int j = 0; j < m_Width; j++) {
                Gizmos.DrawWireCube(new Vector3(j, i, 0), new Vector3(1, 1, 1));
            }
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < m_Exits.Count; i++) {
            Gizmos.DrawWireCube((Vector3)(Vector2)m_Exits[i], new Vector3(1.25f, 1.25f, 1));
        }
    }

    #endregion

}
