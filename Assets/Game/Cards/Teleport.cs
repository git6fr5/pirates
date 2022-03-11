using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Card {

    [SerializeField] private Effect m_TeleportEffect;

    private List<SpriteRenderer> m_VisionIndicators;

    void LateUpdate() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector2Int targetPosition = new Vector2Int((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
        bool m_MouseOverTargetableSquare = m_TargetablePositions != null && m_TargetablePositions.Contains(targetPosition);


        Board board = Board.FindInstance();
        List<Vector2Int> targets = new List<Vector2Int>();
        Player player = board.Get<Player>();

        int a = -m_Range;
        int b = m_Range + 1;
        int c = m_Range + 1;
        Vector2Int offset = player.Position;
        if (m_TargetType == TargetType.Global) {
            a = 0;
            b = board.Height;
            c = board.Width;
            offset = Vector2Int.zero;
        }

        for (int i = a; i < b; i++) {
            for (int j = a; j < c; j++) {
                Vector2Int position = new Vector2Int(j, i) + offset;
                if (board.Check(position)) {
                    Piece piece = board.GetAt<Piece>(position);
                    if (piece == null || piece.GetComponent<Spike>() != null) {
                        targets.Add(position);
                    }
                }
            }
        }

        if (player != null) {
            BoardUI.DrawVisionCharactersUI(targets.ToArray(), board, player.Position, ref m_VisionIndicators, m_MouseOverTargetableSquare && m_Active);
        }

    }

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Piece piece = board.GetAt<Piece>(target);
        if (piece == null || piece.GetComponent<Spike>() != null) {

            Effect newEffect = m_TeleportEffect.Create(origin);
            Effect newEffect1 = m_TeleportEffect.Create(target);

            Piece movePiece = board.GetAt<Piece>(origin);
            movePiece.SetPosition(target);
            movePiece.Snap();

            Debug.Log("Nothing at targetted location.");
            return true; // Should cards use charges if they don't do anything?
        }

        return false;
    }

}
