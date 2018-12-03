using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public int nextScene = 1;

    public void NextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
