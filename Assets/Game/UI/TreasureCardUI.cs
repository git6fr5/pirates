using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCardUI : CardUI {

    private int m_Offset;

    public void SetIndex(int index) {
        m_Offset = index;
    }

    protected override void Activate() {
        Board board = Board.FindInstance();
        Player player = board.Get<Player>();
        bool success = player.AddCard(m_Card);
        if (success) {
            Destroy(gameObject);
        }
    }

    protected override void Deactivate() {

    }

    protected override void DrawTarget() {


    }

    protected override void SetScale(float deltaTime) {

        float targetScale = m_MouseOver ? 1.5f : 1f; // Mathf.Max(0f, 1f - distance);

        m_Scale = m_Scale != targetScale ? m_Scale + Mathf.Sign(targetScale - m_Scale) * m_ScaleSpeed * deltaTime : m_Scale;
        if (Mathf.Abs(targetScale - m_Scale) < 0.05f) {
            m_Scale = targetScale;
        }

        transform.localScale = m_Scale * new Vector3(1f, 1f, 1f);

        //if (m_Scale <= 0f) {
        //    m_CardTargetType.transform.SetParent(null);
        //    m_CardTargetType.gameObject.SetActive(true);
        //    m_CardTargetType.transform.localScale = new Vector3(1f, 1f, 1f);
        //}
        //else {
        //    // m_CardTargetType.gameObject.SetActive(false);
        //    m_CardTargetType.transform.SetParent(transform);
        //    m_CardTargetType.transform.localScale = new Vector3(1f, 1f, 1f);
        //}

    }

    protected override void SetPosition(float deltaTime) {

        //if (m_Active) {
        //    Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    transform.position = mousePosition;
        //}
        //else {
        //    transform.position = (Vector3)m_Origin;
        //}


        //float targetY = 0f;
        //if (m_MouseOver && !m_Active) {
        //    targetY = 3f;
        //}

        //float currY = m_SpriteRenderer.material.GetVector("_Offset").y;
        //if (currY > targetY + 0.05f || currY < targetY - 0.05f) {
        //    currY += Mathf.Sign(targetY - currY) * 3f * m_ScaleSpeed * deltaTime;
        //}
        //else {
        //    currY = targetY;
        //}

        //foreach (KeyValuePair<Transform, Vector3> item in m_LocalOrigins) {
        //    item.Key.localPosition = item.Value + Vector3.up * currY;
        //}

        //m_SpriteRenderer.material.SetVector("_Offset", new Vector4(0f, currY, 0f, 0f));

        //if (m_Active) {
        //    m_CardTargetType.transform.position = transform.position;
        //}

        float y = 0.25f * Mathf.Sin( Mathf.PI * (Board.Ticks + m_Offset / 3f));
        m_SpriteRenderer.material.SetVector("_Offset", new Vector4(0f, y, 0f, 0f));

        foreach (KeyValuePair<Transform, Vector3> item in m_LocalOrigins) {
            item.Key.localPosition = item.Value + Vector3.up * y;
        }

    }

}
