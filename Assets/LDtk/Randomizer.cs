using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour {

    public int m_Seed;

    [SerializeField] private Player m_Player;
    public Player MainPlayer => m_Player;

    [SerializeField] private Piece[] m_WallPieces;
    public Piece WallPiece => GetRandomWallPiece();

    [SerializeField] private Piece[] m_TreasureChests;
    public Piece TreasureChest => m_TreasureChests[0];

    [SerializeField] private Piece[] m_Enemies;
    public Piece EasyEnemy => GetRandomEnemy(0);

    void Start() {

    }

    public int Jumble(int depth, Vector2Int quadrant, int modulo) {
        int quad = 2 * quadrant.y + quadrant.x;
        int index = (m_Seed * 7919 + depth * 7907 + quad * 7901) % modulo;
        print(index);
        return index;
    }

    private Piece GetRandomWallPiece() {
        int index = Random.Range(0, m_WallPieces.Length);
        return m_WallPieces[index];
    }

    private Piece GetRandomEnemy(int difficulty) {
        int index = Random.Range(0, m_Enemies.Length);
        return m_Enemies[index];
    }

}
