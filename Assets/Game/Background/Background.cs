/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Background : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    [SerializeField] private Transform m_Left;
    [SerializeField] private Transform m_Right;
    [SerializeField] private Transform m_Center;
    [SerializeField] private Transform m_Board;

    [SerializeField, ReadOnly] private bool m_Open;
    [SerializeField, ReadOnly] private bool m_Close;
    [SerializeField, ReadOnly] private bool m_Rebound;


    [SerializeField, Range(0f, 100f)] private float m_CloseSpeed;
    [SerializeField, Range(0f, 100f)] private float m_OpenSpeed;
    [SerializeField, Range(0f, 100f)] private float m_ReboundSpeed;
    [SerializeField, Range(0f, 100f)] private float m_ReboundLength;
    

    [SerializeField, Range(0f, 1f)] private float m_Openness;
    [SerializeField, Range(0f, 100f)] private float m_BoardHeight;

    [SerializeField] private Vector3 m_LeftOrigin;
    [SerializeField] private Vector3 m_RightOrigin;
    [SerializeField] private Vector3 m_BoardOrigin;
    [SerializeField] private Vector3 m_CenterScale;


    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {

        m_CenterScale = m_Center.transform.localScale;
        m_LeftOrigin = m_Left.transform.localPosition;
        m_RightOrigin = m_Right.transform.localPosition;
        m_BoardOrigin = m_Board.transform.localPosition;

    }

    void Update() {

        if (m_Open) {
            m_Openness += m_OpenSpeed * Time.deltaTime;
            if (m_Openness >= 1f + m_ReboundLength) {
                m_Rebound = true;
                m_Open = false;
            }
        }

        if (m_Rebound) {
            m_Openness -= m_ReboundSpeed * Time.deltaTime;
            if (m_Openness <= 1f) {
                m_Openness = 1f;
                m_Rebound = false;
            }
        }

        if (m_Close) {
            m_Openness -= m_CloseSpeed * Time.deltaTime;
            if (m_Openness <= 0.05f) {
                m_Openness = 0f;
                m_Close = false;
            }
        }

        m_Center.transform.localScale = new Vector3(m_CenterScale.x * m_Openness, m_CenterScale.y, m_CenterScale.z); ;
        m_Left.transform.localPosition = m_LeftOrigin * m_Openness;
        m_Right.transform.localPosition = m_RightOrigin * m_Openness;

        float wave = 0f; // Board.Ticks % 4 > 3 || Board.Ticks % 4 < 1 ? 1f : 0f;
        m_Board.transform.localPosition = m_BoardOrigin + (1 - Mathf.Min(1f, m_Openness)) * m_BoardHeight * Vector3.up + wave * (1f / 16f) * Mathf.Sin(Mathf.PI * Board.Ticks) * Vector3.up;


    }

    #endregion

    #region Openning and Closing

    public void Open() {
        m_Open = true;
    }

    public void Close() {
        m_Close = true;
    }

    public float GetCloseDelay() {
        return 1f / m_CloseSpeed;
    }

    public float GetOpenDelay() {
        return (1f + m_ReboundLength) / m_OpenSpeed + 0.25f / m_ReboundSpeed;
    }

    #endregion

}
