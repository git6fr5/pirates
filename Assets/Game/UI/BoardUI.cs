using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour {

    public static Sprite SquareIndicator;
    public Sprite m_SquareIndicator;

    public static Sprite HeartIndicator;
    public Sprite m_HeartIndicator;

    void Start() {
        SquareIndicator = m_SquareIndicator;
        HeartIndicator = m_HeartIndicator;
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

    public static void DrawHealthUI(int hearts, Vector2Int origin, ref List<SpriteRenderer> healthIndicators, bool redraw = true) {
        Reset(ref healthIndicators);
        if (redraw) {
            DrawHealth(hearts, origin, ref healthIndicators);
        }
    }

    public static void DrawTargets(Card card, Board board, Vector2Int origin, ref List<SpriteRenderer> targetIndicators) {
        List<Vector2Int> targettablePositions = card.GetTargetablePositions(board, origin);
        for (int i = 0; i < targettablePositions.Count; i++) {
            DrawSquare(targettablePositions[i], new Color(1f, 0f, 0f, 0f), 0.25f, ref targetIndicators);
        }
    }

    public static void DrawVision(int visionDistance, Board board, Vector2Int origin, ref List<SpriteRenderer> visionIndicators) {
        for (int i = -visionDistance; i < visionDistance + 1; i++) {
            for (int j = -visionDistance; j < visionDistance + 1; j++) {
                Vector2Int position = origin + new Vector2Int(j, i);
                bool validPosition = position.x >= 0 && position.y < board.Width && position.y >= 0 && position.y < board.Height;
                if (validPosition) {
                    DrawSquare(position, new Color(1f, 1f, 0f, 0f), 0.25f, ref visionIndicators);
                }
            }
        }
    }

    public static void DrawHealth(int hearts, Vector2Int origin, ref List<SpriteRenderer> healthIndicators) {
        for (int i = 0; i < hearts; i++) {
            Vector3 position = (Vector3)(Vector2)origin + Vector3.up + Vector3.right * ((float)i - (float)hearts / 2f);
            DrawSprite(HeartIndicator, position, new Color(1f, 1f, 0f, 0f), 0.5f, ref healthIndicators);
        }
    }

    public static void DrawSquare(Vector2Int position, Color color, float opacity, ref List<SpriteRenderer> spriteRenderers) {
        SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        indicator.sortingOrder = 1;
        indicator.transform.position = (Vector3)(Vector2)position;
        indicator.color = new Color(color.r, color.g, color.b, opacity);
        indicator.sprite = SquareIndicator;
        spriteRenderers.Add(indicator);
        Destroy(indicator.gameObject, 0.05f);
    }

    public static void DrawSprite(Sprite sprite, Vector3 position, Color color, float opacity, ref List<SpriteRenderer> spriteRenderers) {
        SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        indicator.sortingOrder = 2;
        indicator.transform.position = position;
        indicator.color = new Color(color.r, color.g, color.b, opacity);
        indicator.sprite = sprite;
        spriteRenderers.Add(indicator);
        spriteRenderers.Add(indicator);
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
