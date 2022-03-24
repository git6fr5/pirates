/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // States.
    [SerializeField, ReadOnly] private bool m_Open;
    [SerializeField, ReadOnly] private bool m_Close;

    // Settings.
    [SerializeField, Range(0f, 100f)] private float m_CloseSpeed;
    [SerializeField, Range(0f, 100f)] private float m_OpenSpeed;
    [SerializeField, Range(0f, 100f)] private float m_BoardHeight;

    // Controls.
    [SerializeField, Range(0f, 1f)] private float m_Openness;

    // Origins.
    [SerializeField] private Vector3 m_LeftOrigin;
    [SerializeField] private Vector3 m_RightOrigin;
    [SerializeField] private Vector3 m_BoardOrigin;
    [SerializeField] private Vector3 m_CenterScale;

    public AnimationCurve animationCurve;

    private Board board;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {

        m_CenterScale = m_Center.transform.localScale;
        m_LeftOrigin = m_Left.transform.localPosition;
        m_RightOrigin = m_Right.transform.localPosition;
        m_BoardOrigin = m_Board.transform.localPosition;

        board = m_Board.GetComponent<Board>();

    }

    void Update() {


        if (m_Open) {
            m_Openness += m_OpenSpeed * Time.deltaTime;
            if (m_Openness >= 1f) {
                m_Open = false;
            }
        }

        if (m_Close) {
            m_Openness -= m_CloseSpeed * Time.deltaTime;
            if (m_Openness <= 0.05f) {
                m_Openness = 0f;
                m_Close = false;
            }
        }

        m_Center.transform.localScale = new Vector3(m_CenterScale.x * animationCurve.Evaluate(m_Openness), m_CenterScale.y, m_CenterScale.z); ;
        m_Left.transform.localPosition = m_LeftOrigin * animationCurve.Evaluate(m_Openness);
        m_Right.transform.localPosition = m_RightOrigin * animationCurve.Evaluate(m_Openness);

        float wave = 0f; // Board.Ticks % 4 > 3 || Board.Ticks % 4 < 1 ? 1f : 0f;
        m_Board.transform.localPosition = m_BoardOrigin + (1 - animationCurve.Evaluate(m_Openness)) * m_BoardHeight * Vector3.up + wave * (1f / 16f) * Mathf.Sin(Mathf.PI * Board.Ticks) * Vector3.up;


    }

    private bool playerHasExisted = false;
    void LateUpdate() {
        if (board != null) {
            if (!playerHasExisted && board.Get<Player>() != null) {
                playerHasExisted = true;
            }
            if (playerHasExisted && board.Get<Player>() == null) {
                FadeToBlack();
            }
        }
    }

    #endregion

    /* --- Openining and Closing --- */
    #region Openning and Closing

    public void Open() {
        m_Open = true;
        SoundController.PlaySound(SoundController.MapOpen);
    }

    public void Close() {
        m_Close = true;
        SoundController.PlaySound(SoundController.MapClose);
    }

    public float GetCloseDelay() {
        return 1f / m_CloseSpeed;
    }

    public float GetOpenDelay() {
        return 1f / m_OpenSpeed;
    }

    #endregion


    bool m_Fading = false;
    private void FadeToBlack() {
        if (m_Fading) {
            return;
        }

        SoundController.PlaySound(SoundController.LoseSound2, 0);

        for (int i = 0; i < board.Pieces.Count; i++) {
            if (board.Pieces[i] != null) {
                StartCoroutine(IEDestroyPiece(i, i * 0.05f));
            }
        }

        StartCoroutine(IEGameOver());

        m_Fading = true;
    }

    public Effect explosionEffect;

    private IEnumerator IEDestroyPiece(int i, float delay) {
        yield return new WaitForSeconds(delay);
        if (board.Pieces[i] != null) {

            if (i != 0 && i % 5 == 0) {
                // SoundController.PlaySound(SoundController.LoseSound2, (i / 5) % 2);
            }

            explosionEffect.Create(board.Pieces[i].Position, 1f);
            Destroy(board.Pieces[i].gameObject);
        }
    }

    private IEnumerator IEGameOver() {
        yield return new WaitForSeconds(4f);
        board.Reset();
        Close();
        yield return new WaitForSeconds(GetCloseDelay());
        SceneManager.LoadScene("Game Over");
    }


    public void WinGame() {
        if (m_Fading) {
            return;
        }

        SoundController.PlaySound(SoundController.LoseSound2, 0);

        for (int i = 0; i < board.Pieces.Count; i++) {
            if (board.Pieces[i] != null) {
                StartCoroutine(IEDestroyPiece(i, i * 0.05f));
            }
        }

        StartCoroutine(IEWin());

        m_Fading = true;
    }

    private IEnumerator IEWin() {
        yield return new WaitForSeconds(4f);
        board.Reset();
        Close();
        yield return new WaitForSeconds(GetCloseDelay());
        SceneManager.LoadScene("Win");
    }

}
