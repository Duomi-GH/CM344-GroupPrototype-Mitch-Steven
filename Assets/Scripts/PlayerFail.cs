using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFail : MonoBehaviour
{
    public GameObject levelFailButton;
    public GameObject levelGameOver;

    public void Fail()
    {
        Time.timeScale = 0f;
        levelFailButton.SetActive(true);
        levelGameOver.SetActive(true);
    }

    public void RestartButton() {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }
}
