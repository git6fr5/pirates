using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    public int m_Seed;

    // Walls.
    [SerializeField] private Piece[] m_Walls;
    public Piece Wall => GetRandomWall();
    
    // Bushes.
    [SerializeField] private Piece[] m_Bushes;
    public Piece Bush => GetRandomBush();

    // Treasure Chests.
    [SerializeField] private Piece[] m_TreasureChests;
    public Piece TreasureChest => m_TreasureChests[0];

    // Enemies.
    [SerializeField] private Enemy[] m_Enemies;

    [HideInInspector] private List<Piece> m_EasyEnemies;
    public Piece EasyEnemy => GetRandomEnemy(m_EasyEnemies);

    [HideInInspector] private List<Piece> m_MidEnemies;
    [HideInInspector] private List<Piece> m_HardEnemies;
    public Piece MidEnemy => GetRandomEnemy(m_MidEnemies);
    public Piece HardEnemy => GetRandomEnemy(m_HardEnemies);

    /* --- Unity --- */
    #region Unity

    void Awake() {
        SortEnemies();
    }

    #endregion

    /* --- Sorting --- */
    #region Sorting

    public int Jumble(int depth, Vector2Int quadrant, int modulo) {
        int quad = 2 * quadrant.y + quadrant.x;
        int index = (m_Seed * 7919 + depth * 7907 + quad * 7901) % modulo;
        return index;
    }

    private void SortEnemies() {
        m_EasyEnemies = new List<Piece>();
        m_MidEnemies = new List<Piece>();
        m_HardEnemies = new List<Piece>();

        for (int i = 0; i < m_Enemies.Length; i++) {
            switch (m_Enemies[i].EnemyDifficulty) {
                case Difficulty.Easy:
                    m_EasyEnemies.Add(m_Enemies[i]);
                    break;
                case Difficulty.Mid:
                    m_MidEnemies.Add(m_Enemies[i]);
                    break;
                case Difficulty.Hard:
                    m_HardEnemies.Add(m_Enemies[i]);
                    break;
            }
        }
    }

    #endregion

    /* --- Pieces --- */
    #region Pieces

    private Piece GetRandomWall() {
        if (m_Walls == null || m_Walls.Length == 0) {
            return null;
        }
        int index = Random.Range(0, m_Walls.Length);
        return m_Walls[index];
    }

    private Piece GetRandomBush() {
        if (m_Bushes == null || m_Bushes.Length == 0) {
            return null;
        }
        int index = Random.Range(0, m_Bushes.Length);
        return m_Bushes[index];
    }

    public Piece GetRandomEnemy(List<Piece> enemies) {
        if (enemies.Count == 0) {
            return null;
        }
        int index = Random.Range(0, enemies.Count);
        return enemies[index];
    }

    #endregion

}
