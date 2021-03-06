using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : Card {

    public int m_Radius;
    private List<SpriteRenderer> m_VisionIndicators;

    [SerializeField] private Effect m_ProjectileEffect;
    [SerializeField] private Effect m_ExplodeEffect;

    void LateUpdate() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector2Int targetPosition = new Vector2Int((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
        bool m_MouseOverTargetableSquare = m_TargetablePositions != null && m_TargetablePositions.Contains(targetPosition);

        BoardUI.DrawVisionUI(m_Radius, Board.FindInstance(), targetPosition, ref m_VisionIndicators, m_MouseOverTargetableSquare && m_Active);

    }

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        List<Vector2Int> areaOfEffect = new List<Vector2Int>();
        areaOfEffect = board.AOETargetting(target, m_Radius, ref areaOfEffect);

        Effect newEffect = m_ProjectileEffect.Create(origin, board.TurnDelay);
        newEffect.MoveTo(target, board.TurnDelay);

        float distance = (origin - target).magnitude;
        float maxDistance = Mathf.Sqrt(2) * m_Range;
        float actualSpeed = maxDistance / board.TurnDelay;
        float timeInterval = distance / actualSpeed;

        for (int i = 0; i < areaOfEffect.Count; i++) {
            base.Effect(board, origin, areaOfEffect[i]);
            AOEEffect(board, areaOfEffect[i], timeInterval);
        }        
        return true;
    }

    public virtual bool AOEEffect(Board board, Vector2Int target, float delay) {
        return true;
    }

}
