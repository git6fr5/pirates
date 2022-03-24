using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedEffectBugFixer : MonoBehaviour
{

    public static DelayedEffectBugFixer Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    public void DelayEffectAnim(Effect effect, AudioClip sound, float delay, Vector2Int target, Board board, int i) {
        StartCoroutine(IEDelayedEffectAnim(effect, sound, delay, target, board, i));
    }

    private IEnumerator IEDelayedEffectAnim(Effect effect, AudioClip sound, float delay, Vector2Int target, Board board, int i) {
        yield return new WaitForSeconds(delay);
        Effect newEffect = effect.Create(target, 3f * board.TurnDelay);
        SoundController.PlaySound(sound, i % 2);
    }
}
