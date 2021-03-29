using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // VARIABLES

    // UPDATES

    // METHODS
    public void LoadScene(int x)
    {
        SceneManager.LoadScene(x);
    }
}

