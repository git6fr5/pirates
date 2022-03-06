using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : Card {

    public int m_Radius;
    private List<SpriteRenderer> m_VisionIndicators;

    void LateUpdate() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector2Int targetPosition = new Vector2Int((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
        bool m_MouseOverTargetableSquare = m_TargetablePositions != null && m_TargetablePositions.Contains(targetPosition);

        BoardUI.DrawVisionUI(m_Radius, Board.FindInstance(), targetPosition, ref m_VisionIndicators, m_MouseOverTargetableSquare && m_Active);

    }

    public override bool Effect(Board board, Vector2Int target) {
        List<Vector2Int> areaOfEffect = new List<Vector2Int>();
        areaOfEffect = board.AllWithinRadius(target, m_Radius, ref areaOfEffect);

        for (int i = 0; i < areaOfEffect.Count; i++) {
            base.Effect(board, areaOfEffect[i]);
            AOEEffect(board, areaOfEffect[i]);
        }        
        return true;
    }

    public virtual bool AOEEffect(Board board, Vector2Int target) {
        return true;
    }

}
