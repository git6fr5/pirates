using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IEDelayedMusic());
    }

    private IEnumerator IEDelayedMusic() {
        yield return new WaitForSeconds(2f);
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
