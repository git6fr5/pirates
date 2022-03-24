using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour {

    public static Sprite SquareIndicator;
    public Sprite m_SquareIndicator;

    public static Sprite HeartIndicator;
    public Sprite m_HeartIndicator;

    public static Sprite DamageIndicator;
    public Sprite m_DamageIndicator;

    public static Sprite DamageIndicator2;
    public Sprite m_DamageIndicator2;

    public static Sprite DamageIndicator3;
    public Sprite m_DamageIndicator3;

    public static Sprite DamageIndicator4;
    public Sprite m_DamageIndicator4;

    public static Material HeartMaterial;
    public Material m_HeartMaterial;

    public static float Ticks = 0f;

    void Start() {
        SquareIndicator = m_SquareIndicator;
        HeartIndicator = m_HeartIndicator;
        DamageIndicator = m_DamageIndicator;
        DamageIndicator2 = m_DamageIndicator2;
        DamageIndicator3 = m_DamageIndicator3;
        DamageIndicator4 = m_DamageIndicator4;
        HeartMaterial = m_HeartMaterial;
    }

    void Update() {

        Ticks += Time.deltaTime;

    }

    public static void DrawTargetUI(Card card, Board board, Vector2Int origin, ref List<SpriteRenderer> targetIndicators, bool redraw = true) {
        Reset(ref targetIndicators);
        if (redraw) {
            DrawTargets(card, board, origin, ref targetIndicators);
        }
    }

    public static void DrawVisionUI(int visionDistance, Board board, Vector2Int origin, ref List<SpriteRenderer> visionIndicators, bool redraw = true) {
        Reset(ref visionIndicators);
        if (redraw) {
            DrawVision(visionDistance, board, origin, ref visionIndicators);
        }
    }

    public static void DrawVisionRowUI(int width, Board board, Vector2Int origin, ref List<SpriteRenderer> visionIndicators, bool redraw = true, bool yesPlayer = true) {
        Reset(ref visionIndicators);
        if (redraw) {
            DrawVisionRow(width, board, origin, ref visionIndicators, yesPlayer);
        }
    }

    public static void DrawVisionCharactersUI(Vector2Int[] targets, Board board, Vector2Int origin, ref List<SpriteRenderer> visionIndicators, bool redraw = true) {
        Reset(ref visionIndicators);
        if (redraw) {
            DrawVisionCharacters(board, targets, origin, ref visionIndicators);
        }
    }

    public static void DrawHealthUI(int hearts, Vector2Int origin, ref List<SpriteRenderer> healthIndicators, bool redraw = true) {
        Reset(ref healthIndicators);
        if (redraw) {
            DrawHealth(hearts, origin, ref healthIndicators);
        }
    }

    public static void DrawDamageUI(int damage, Vector2Int origin, ref List<SpriteRenderer> damageIndicators, bool redraw = true, int AorB = 0) {
        Reset(ref damageIndicators);
        if (redraw) {
            DrawDamage(damage, origin, ref damageIndicators, AorB);
        }
    }

    public static void DrawTargets(Card card, Board board, Vector2Int origin, ref List<SpriteRenderer> targetIndicators) {
        List<Vector2Int> targettablePositions = card.GetTargetablePositions(board, origin);
        for (int i = 0; i < targettablePositions.Count; i++) {
            DrawSquare(targettablePositions[i], new Color(1f, 0f, 0f, 0f), 0.25f, ref targetIndicators, 2);
        }
    }

    public static void DrawVision(int visionDistance, Board board, Vector2Int origin, ref List<SpriteRenderer> visionIndicators) {
        for (int i = -visionDistance; i < visionDistance + 1; i++) {
            for (int j = -visionDistance; j < visionDistance + 1; j++) {
                Vector2Int position = origin + new Vector2Int(j, i);
                bool validPosition = position.x >= 0 && position.y < board.Width && position.y >= 0 && position.y < board.Height;
                if (validPosition) {
                    DrawSquare(position, new Color(1f, 1f, 0f, 0f), 0.25f, ref visionIndicators, 1);
                }
            }
        }
    }

    public static void DrawVisionRow(int width, Board board, Vector2Int origin, ref List<SpriteRenderer> visionIndicators, bool yesPlayer = true) {
        int row = origin.y;
        if (yesPlayer) {
            Player player = board.Get<Player>();
            if (player == null) {
                return;
            }
            for (int n = -width; n <= width; n++) {
                if (row + n >= 0 && row + n < board.Height) {
                    for (int i = 0; i < board.Width; i++) {
                        if (player.Position != new Vector2Int(i, row + n)) {
                            DrawSquare(new Vector2Int(i, row + n), new Color(1f, 1f, 0f, 0f), 0.5f, ref visionIndicators, 1);
                        }
                    }
                }
            }
        }
        else {
            for (int n = -width; n <= width; n++) {
                if (row + n >= 0 && row + n < board.Height) {
                    for (int i = 0; i < board.Width; i++) {
                        DrawSquare(new Vector2Int(i, row + n), new Color(1f, 1f, 0f, 0f), 0.5f, ref visionIndicators, 1);
                    }
                }
            }
        }
        
    }

    public static void DrawVisionCharacters(Board board, Vector2Int[] targets, Vector2Int origin, ref List<SpriteRenderer> visionIndicators) {
        for (int i = 0; i < targets.Length; i++) {
            if (targets[i] != origin) {
                DrawSquare(targets[i], new Color(1f, 1f, 0f, 0f), 0.5f, ref visionIndicators, 1);
            }
        }
    }

    public static void DrawHealth(int hearts, Vector2Int origin, ref List<SpriteRenderer> healthIndicators) {
        for (int i = 0; i < hearts; i++) {
            float bob = 1.2f + 0.05f * Mathf.Sin(Mathf.PI * (Ticks + i / 4f));
            Vector3 offset = Vector3.up * bob + Vector3.right * ((float)i - (float)(hearts - 1f) / 2f) / 1.5f;
            Vector3 position = (Vector3)(Vector2)origin + offset;
            Sprite sprite = DrawSprite(HeartIndicator, position, new Color(1f, 1f, 0f, 0f), 1f, ref healthIndicators, HeartMaterial, 3);
        }
    }

    public static void DrawDamage(int damage, Vector2Int origin, ref List<SpriteRenderer> damageIndicators, int AorB) {
        for (int i = 0; i < damage; i++) {
            float bob = -0.8f + 0.05f * Mathf.Sin(Mathf.PI * (Ticks + i / 4f));
            Vector3 offset = Vector3.up * bob + Vector3.right * ((float)i - (float)(damage - 1f) / 2f) / 1.5f;
            Vector3 position = (Vector3)(Vector2)origin + offset;
            if (AorB == 0) {
                Sprite sprite = DrawSprite(DamageIndicator, position, new Color(1f, 1f, 0f, 0f), 1f, ref damageIndicators, HeartMaterial, 3);
            }
            else if (AorB == 1) {
                Sprite sprite = DrawSprite(DamageIndicator2, position, new Color(1f, 1f, 0f, 0f), 1f, ref damageIndicators, HeartMaterial, 3);
            }
            else if (AorB == 2) {
                Sprite sprite = DrawSprite(DamageIndicator3, position, new Color(1f, 1f, 0f, 0f), 1f, ref damageIndicators, HeartMaterial, 3);
            }
            else if (AorB == 3) {
                Sprite sprite = DrawSprite(DamageIndicator4, position, new Color(1f, 1f, 0f, 0f), 1f, ref damageIndicators, HeartMaterial, 3);
            }
        }
    }

    public static void DrawSquare(Vector2Int position, Color color, float opacity, ref List<SpriteRenderer> spriteRenderers, int order = 1) {
        SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        indicator.sortingOrder = order - 10;
        indicator.sortingLayerName = "Characters";
        indicator.transform.position = (Vector3)(Vector2)position;
        indicator.color = new Color(color.r, color.g, color.b, opacity);
        indicator.sprite = SquareIndicator;
        spriteRenderers.Add(indicator);
        Destroy(indicator.gameObject, 1f/24f);
    }

    public static Sprite DrawSprite(Sprite sprite, Vector3 position, Color color, float opacity, ref List<SpriteRenderer> spriteRenderers, Material material, int order = 1) {
        SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        indicator.sortingOrder = 2;
        indicator.sortingLayerName = "UI";
        indicator.transform.position = position;
        indicator.color = new Color(color.r, color.g, color.b, opacity);
        indicator.sprite = sprite;
        spriteRenderers.Add(indicator);
        if (material != null) {
            indicator.material = material;
        }
        return sprite;
    }

    public static void Reset(ref List<SpriteRenderer> spriteRenderers) {
        if (spriteRenderers != null) {
            for (int i = 0; i < spriteRenderers.Count; i++) {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                if (spriteRenderer != null) {
                    Destroy(spriteRenderer.gameObject);
                }
            }
        }
        spriteRenderers = new List<SpriteRenderer>();
    }

}
