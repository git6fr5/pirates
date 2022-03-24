using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : Card {

    public int m_Radius;
    private List<SpriteRenderer> m_VisionIndicators;

    [SerializeField] private Effect m_ProjectileEffect;
    [SerializeField] private Effect m_ExplodeEffect;

    public AudioClip explodeSound;

    public bool notTargetEnemies;

    void LateUpdate() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector2Int targetPosition = new Vector2Int((int)Mathf.Floor(mousePosition.x), (int)Mathf.Floor(mousePosition.y));
        bool m_MouseOverTargetableSquare = m_TargetablePositions != null && m_TargetablePositions.Contains(targetPosition);

        BoardUI.DrawVisionUI(m_Radius, Board.FindInstance(), targetPosition, ref m_VisionIndicators, m_MouseOverTargetableSquare && m_Active);

    }

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        List<Vector2Int> areaOfEffect = new List<Vector2Int>();
        areaOfEffect = board.AOETargetting(target, m_Radius, ref areaOfEffect);

        float delay = 0f;

        if (m_ProjectileEffect != null) {
            Effect newEffect = m_ProjectileEffect.Create(origin, 3f * board.TurnDelay);
            newEffect.FallTo(target, 3f * board.TurnDelay);

            float distance = (origin - target).magnitude;
            // float maxDistance = Mathf.Sqrt(2) * (overrideRange > 0 ? overrideRange : m_Range);
            float actualSpeed = distance / (3f * board.TurnDelay);
            delay = distance / actualSpeed;
        }

        for (int i = 0; i < areaOfEffect.Count; i++) {
            AOEEffect(board, areaOfEffect[i], delay, i);
        }        
        return true;
    }

    public int overrideRange = -1;

    public virtual bool AOEEffect(Board board, Vector2Int target, float delay, int i) {
        if (Charges == 1) {
            m_ExplodeEffect.transform.SetParent(DelayedEffectBugFixer.Instance.transform);
            DelayedEffectBugFixer.Instance.DelayEffectAnim(m_ExplodeEffect, explodeSound, delay, target, board, i);
        }
        else {
            StartCoroutine(IEDelayedEffectAnim(delay, target, board, i));
        }

        Piece piece = board.GetAt<Piece>(target);
        Debug.Log("Doing damage");

        

        if (piece != null) {

            if (notTargetEnemies && (piece.GetComponent<Enemy>() || piece.GetComponent<Parrot>())) {
                return true;
            }
            else if (!notTargetEnemies && piece.GetComponent<Player>()) {
                return true;
            }

            piece.TakeDamage(m_Value, delay);

            Character character = piece.GetComponent<Character>();
            if (character != null && m_StatusEffect != Status.None && m_Duration > 0) {
                character.ApplyStatus(m_StatusEffect, m_Duration);
            }

            float duration = board.TurnDelay;
            piece.StartShake(delay, duration);

        }

        return true;
    }

    private IEnumerator IEDelayedEffectAnim(float delay, Vector2Int target, Board board, int i) {
        yield return new WaitForSeconds(delay);
        Effect newEffect = m_ExplodeEffect.Create(target, 3f * board.TurnDelay);
        SoundController.PlaySound(explodeSound, i % 2);
    }

}
