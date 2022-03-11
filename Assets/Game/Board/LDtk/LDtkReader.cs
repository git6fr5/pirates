using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

using LDtkLevel = LDtkUnity.Level;
using PieceData = Board.PieceData;

public class LDtkReader : MonoBehaviour {

    public class LDtkTileData {

        /* --- Properties --- */
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public int index;

        /* --- Constructor --- */
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
        }

    }

    public static string PieceLayer = "Layer_1";

    public string m_LevelName;

    [SerializeField] private LDtkComponentProject m_LDtkData;
    [HideInInspector] private LdtkJson m_JSON;
    [SerializeField] private Environment m_Environment;
    [SerializeField] private List<PieceData> m_PieceData;

    public int m_Easy;
    public int m_Mid;
    public int m_Hard;


    public PieceData[] Get(int depth, int difficulty) {

        m_PieceData = new List<PieceData>();
        Vector2Int quadrant = new Vector2Int(0, 0);

        int[][] qs;
        int index;
        GetDifficulty(depth, difficulty, quadrant, out qs, out index);
        print(difficulty);

        index = qs[0][0] + (m_Environment.Jumble(depth, quadrant, qs[0][1]) % qs[0][1]);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        quadrant = new Vector2Int(1, 0);
        index = qs[1][0] + (m_Environment.Jumble(depth, quadrant, qs[1][1]) % qs[1][1]);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        quadrant = new Vector2Int(0, 1);
        index = qs[2][0] + (m_Environment.Jumble(depth, quadrant, qs[2][1]) % qs[2][1]);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        quadrant = new Vector2Int(1, 1);
        index = qs[3][0] + (m_Environment.Jumble(depth, quadrant, qs[3][1]) % qs[3][1]);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        return m_PieceData.ToArray();
    }

    private void GetDifficulty(int depth, int difficulty, Vector2Int quadrant, out int[][] quadrants, out int index) {
        
        int[] easy = new int[] { 0, m_Easy };
        int[] mid = new int[] { m_Easy + 1, m_Mid };
        int[] hard = new int[] { m_Mid + 1, m_Hard };

        quadrants = new int[][] { easy, easy, easy, easy };
        index = 0;
        if (difficulty == 1) { // 3 easy, 1 medium
            index = m_Environment.Jumble(depth, quadrant, 3) % 3;
            quadrants[index] = mid;
        }
        if (difficulty == 2) { // 2 easy, 2 medium
            index = m_Environment.Jumble(depth, quadrant, 3) % 3;
            quadrants[index] = mid;
            index += (1 + m_Environment.Jumble(depth, quadrant, 2) % 2);
            quadrants[index] = mid;
        }
        if (difficulty == 3) { 
            // BOSS
            // return m_PieceData.ToArray();
        }
        if (difficulty == 4) { // 1 easy, 3 medium
            index = m_Environment.Jumble(depth, quadrant, 3) % 3;
            for (int i = 0; i < quadrants.Length; i++) {
                if (i != index) {
                    quadrants[i] = mid;
                }
            }
        }
        if (difficulty == 5) { // 4 medium
            for (int i = 0; i < quadrants.Length; i++) {
                quadrants[i] = mid;
            }
        }
        if (difficulty == 6) {
            // BOSS
            // return m_PieceData.ToArray();
        }
        if (difficulty == 7) { // 1 hard, 3 medium
            for (int i = 0; i < quadrants.Length; i++) {
                quadrants[i] = mid;
            }
            index = m_Environment.Jumble(depth, quadrant, 3) % 3;
            quadrants[index] = hard;
        }
        if (difficulty == 8) { // 2 hard, 2 medium
            for (int i = 0; i < quadrants.Length; i++) {
                quadrants[i] = mid;
            }
            index = m_Environment.Jumble(depth, quadrant, 3) % 3;
            quadrants[index] = hard;
            index += (1 + m_Environment.Jumble(depth, quadrant, 2) % 2);
            quadrants[index] = hard;
        }
        if (difficulty > 8) { // 3 hard, 1 medium
            for (int i = 0; i < quadrants.Length; i++) {
                quadrants[i] = mid;
            }
            index = m_Environment.Jumble(depth, quadrant, 3) % 3;
            for (int i = 0; i < quadrants.Length; i++) {
                if (i != index) {
                    quadrants[i] = hard;
                }
            }
        }
        if (difficulty == 10) {
            // Boss.
            // return m_PieceData.ToArray();
        }
    }

    public void OpenLevelByName(string levelName, Vector2Int quadrant) {
        LDtkLevel ldtkLevel = GetLevelByName(m_LDtkData, levelName);
        OpenLevel(ldtkLevel, quadrant);
    }

    protected LDtkLevel GetLevelByName(LDtkComponentProject lDtkData, string levelName) {
        LdtkJson json = lDtkData.FromJson();
        for (int i = 0; i < json.Levels.Length; i++) {
            if (json.Levels[i].Identifier == levelName) {
                return json.Levels[i];
            }
        }
        Debug.Log("Could not find room");
        return null;
    }

    protected void OpenLevel(LDtkLevel ldtkLevel, Vector2Int quadrant) {
        if (ldtkLevel != null) {
            List<LDtkTileData> data = LoadLayer(ldtkLevel, PieceLayer, 16, quadrant);
            AddPieces(data);
        }
    }

    protected List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, Vector2Int quadrant) {
        List<LDtkTileData> layerData = new List<LDtkTileData>();

        Board board = Board.FindInstance();
        Vector2Int offset = new Vector2Int(quadrant.x * board.Width / 2, quadrant.y * board.Height / 2);

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            for (int index = 0; index < layer.GridTiles.Length; index++) {
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / gridSize + offset;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / gridSize;

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(vectorID, gridPosition, index);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    private LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    private List<PieceData> AddPieces(List<LDtkTileData> data) {
        for (int i = 0; i < data.Count; i++) {
            Piece piece = GetPieceByVectorID(data[i].vectorID);
            Vector2Int position = data[i].gridPosition;
            if (piece != null) {
                m_PieceData.Add(new PieceData(piece, position));
            }

        }
        return m_PieceData;
    }

    private Piece GetPieceByVectorID(Vector2Int vectorID) {
        if (vectorID == new Vector2Int(0, 0)) {
            return Random.Range(0f, 1f) < 0.5f ? m_Environment.Bush0 : m_Environment.Bush1;
        }
        if (vectorID == new Vector2Int(1, 0)) {
            return m_Environment.Bush2;
        }
        if (vectorID == new Vector2Int(2, 0)) {
            return m_Environment.Bush3;
        }
        if (vectorID == new Vector2Int(0, 1)) {
            return m_Environment.Wall;
        }
        if (vectorID == new Vector2Int(1, 1)) {
            return m_Environment.spike;
        }
        if (vectorID == new Vector2Int(2, 1)) {
            return m_Environment.plant;
        }
        if (vectorID.y == 2) {
            if (vectorID.x == 0) {
                return m_Environment.CommonTreasureChest;
            }
            if (vectorID.x == 1) {
                return m_Environment.RareTreasureChest;
            }
            if (vectorID.x == 2) {
                return m_Environment.LegendaryTreasureChest;
            }
        }
        if (vectorID == new Vector2Int(0, 3)) {
            return m_Environment.RareTreasureChest;
        }
        if (vectorID.y == 4) {
            if (vectorID.x == 0) {
                return m_Environment.EasyEnemy;
            }
            if (vectorID.x == 1) {
                return m_Environment.MidEnemy;
            }
            if (vectorID.x == 2) {
                return m_Environment.HardEnemy;
            }
        }
        return null;
    }

}
