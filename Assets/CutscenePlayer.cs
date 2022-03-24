using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{

   public GameObject[] scenes;

   int index = -1;

    void Start() {
        Next();
    }

   void Update() {

        if (Input.anyKeyDown) {
            Next();

        }

        //if (Input.GetMouseButtonDown(1)) {
        //    Next();

        //}

        //if (Input.GetMouseButtonDown(0)) {
        //    Next();
        //}

    }

    void Next() {

        index += 1;
        if (index < scenes.Length) {
            for (int i = 0; i < scenes.Length; i++) {
                scenes[i].SetActive(false);
            }
            scenes[index].SetActive(true);
        }
        else {
            SceneManager.LoadScene("Menu");
        }

    }
}
