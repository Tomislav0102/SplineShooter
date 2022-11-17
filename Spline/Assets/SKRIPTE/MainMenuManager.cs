using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] SO_postavke postavke;
    private void Start()
    {
        postavke.level = 1;
        postavke.score = 0;
        SceneManager.LoadScene(1);
    }
}
