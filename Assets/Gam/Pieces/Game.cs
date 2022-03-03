using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] private Character[] m_Characters;
    [SerializeField] private int m_TurnNumber;
    [SerializeField] private int m_MaxTurnNumber;
    [SerializeField] private int m_RoundNumber;

    void Start() {
        m_Characters = (Character[])GetAll<Character>();
        m_MaxTurnNumber = m_Characters.Length;

        StartCoroutine(IEGameLoop());
    }

    public static object[] GetAll<T>() {
        return GameObject.FindObjectsOfType(typeof(T));
    }

    public static object Get<T>() {
        return GameObject.FindObjectOfType(typeof(T));
    }

    private IEnumerator IEGameLoop() {
        while (true) {
            for (int i = 0; i < m_Characters.Length; i++) {
                m_TurnNumber = i;
                m_Characters[i].NewTurn();
                yield return new WaitUntil(() => m_Characters[i].CompletedTurn);
            }
            m_RoundNumber += 1;
        }
    }

}
