using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : Card {

    [SerializeField] private Effect m_ZapEffect;
    private List<SpriteRenderer> m_VisionIndicators;

    void LateUpdate() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector2Int targetPosition = new Vector2Int((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
        bool m_MouseOverTargetableSquare = m_TargetablePositions != null && m_TargetablePositions.Contains(targetPosition);

        Board board = Board.FindInstance();
        Vector2Int[] targets = board.GetAllLocations<Character>();
        Player player = board.Get<Player>();

        BoardUI.DrawVisionCharactersUI(targets, board, player.Position, ref m_VisionIndicators, m_MouseOverTargetableSquare && m_Active);

    }

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Vector2Int[] targets = board.GetAllLocations<Character>();
        for (int i = 0; i < targets.Length; i++) {

            target = targets[i];
            if (target != origin) {
                Piece piece = board.GetAt<Piece>(target);
                if (piece == null) {
                    Debug.Log("Nothing at targetted location.");
                    return false; // Should cards use charges if they don't do anything?
                }

                Effect newEffect = m_ZapEffect.Create(target);

                Debug.Log("Doing damage");
                piece.TakeDamage(m_Value);

                Character character = piece.GetComponent<Character>();
                if (character != null && m_StatusEffect != Status.None && m_Duration > 0) {
                    character.ApplyStatus(m_StatusEffect, m_Duration);
                }

                float duration = board.TurnDelay;
                piece.StartShake(0f, duration);
            }

        }

        return true;
    }

}
