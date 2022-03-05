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

    public static string PieceLayer = "Asset";

    public string m_LevelName;

    [SerializeField] private LDtkComponentProject m_LDtkData;
    [HideInInspector] private LdtkJson m_JSON;
    [SerializeField] private Randomizer m_Randomizer;
    [SerializeField] private List<PieceData> m_PieceData;

    public PieceData[] Get(int depth) {

        m_PieceData = new List<PieceData>();
        PieceData playerPiece = new PieceData(m_Randomizer.MainPlayer, new Vector2Int(0, 0));
        m_PieceData.Add(playerPiece);

        Vector2Int quadrant = new Vector2Int(0, 0);
        int index = m_Randomizer.Jumble(depth, quadrant, 20);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        quadrant = new Vector2Int(1, 0);
        index = m_Randomizer.Jumble(depth, quadrant, 20);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        quadrant = new Vector2Int(0, 1);
        index = m_Randomizer.Jumble(depth, quadrant, 20);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        quadrant = new Vector2Int(1, 1);
        index = m_Randomizer.Jumble(depth, quadrant, 20);
        OpenLevelByName("Level_" + index.ToString(), quadrant);

        return m_PieceData.ToArray();
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
       // Debug.Log("Could not find room");
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
        Vector2Int offset = new Vector2Int(quadrant.x * board.Width / 2 - 1, quadrant.y * board.Height / 2);

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
            // Get the entity based on the environment.

            Piece piece = GetPieceByVectorID(data[i].vectorID);
            Vector2Int position = data[i].gridPosition;
            if (piece != null) {
                m_PieceData.Add(new PieceData(piece, position));
            }

        }
        return m_PieceData;
    }

    private Piece GetPieceByVectorID(Vector2Int vectorID) {
        if (vectorID == new Vector2Int(0, 1)) {
            return m_Randomizer.WallPiece;
        }
        if (vectorID.y == 2) {
            return m_Randomizer.TreasureChest;
        }
        if (vectorID.y == 3) {
            return m_Randomizer.EasyEnemy;
        }
        return null;
    }

}
