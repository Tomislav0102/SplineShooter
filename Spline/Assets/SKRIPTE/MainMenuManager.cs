using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] SO_postavke postavke;
    [SerializeField] Button[] buttons;
    [SerializeField] TMP_Dropdown dropDown;
    private void Start()
    {
        postavke.level = 1;
        postavke.score = 0;
        dropDown.value = postavke.level - 1;
    }
    private void OnEnable()
    {
        buttons[0].onClick.AddListener(NewGame);
        buttons[1].onClick.AddListener(QuitGame);
        dropDown.onValueChanged.AddListener(delegate
        {
            ChangeDropDown();
        });
    }

    private void ChangeDropDown()
    {
        postavke.level = dropDown.value + 1;
    }

    private void OnDisable()
    {
        buttons[0].onClick.RemoveListener(NewGame);
        buttons[1].onClick.RemoveListener(QuitGame);
        dropDown.onValueChanged.RemoveAllListeners();
    }

    void NewGame()
    {
        SceneManager.LoadScene(1);
    }
    void QuitGame()
    {
        Application.Quit();
    }
}
