using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : Card {

    public int m_Width;
    private List<SpriteRenderer> m_VisionIndicators;
    
    [SerializeField] private Effect m_ProjectileEffect;
    [SerializeField] private Effect m_ExplodeEffect;

    void LateUpdate() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector2Int targetPosition = new Vector2Int((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
        bool m_MouseOverTargetableSquare = m_TargetablePositions != null && m_TargetablePositions.Contains(targetPosition);

        BoardUI.DrawVisionRowUI(m_Width, Board.FindInstance(), targetPosition, ref m_VisionIndicators, m_MouseOverTargetableSquare && m_Active);

    }

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {

        int animOffset = 10;
        float animDuration = 3f;

        Vector2Int startpos = new Vector2Int(-animOffset, target.y);
        Vector2Int endpos = new Vector2Int(board.Width + animOffset, target.y);

        Effect newEffect = m_ProjectileEffect.Create(startpos, animDuration);
        newEffect.MoveTo(endpos, animDuration);

        int row = target.y;
        for (int n = -m_Width; n <= m_Width; n++) {
            if (row + n >= 0 && row + n < board.Height) {
                for (int i = 0; i < board.Width + 1; i++) {

                    Vector2Int _actualTarget = new Vector2Int(i, row + n);
                    if (_actualTarget != origin) {

                        float animSpeed = (animOffset * 2 + board.Width) / animDuration;
                        float timeToGetToZero = animOffset / animSpeed;
                        float delay = timeToGetToZero + (i - 1) / animSpeed;
                        StartCoroutine(IEDelayedEffectAnim(delay, _actualTarget, board));

                        Piece piece = board.GetAt<Piece>(_actualTarget);
                        if (piece != null) {
                            piece.TakeDamage(m_Value, delay);

                            Character character = piece.GetComponent<Character>();
                            if (character != null && m_StatusEffect != Status.None && m_Duration > 0) {
                                character.ApplyStatus(m_StatusEffect, m_Duration);
                            }
                        }

                    }
                }
            }
        }

        return true;
    }

    private IEnumerator IEDelayedEffectAnim(float delay, Vector2Int target, Board board) {
        yield return new WaitForSeconds(delay);
        Effect newEffect = m_ExplodeEffect.Create(target, board.TurnDelay);
    }

}
