using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathCanvas : MonoBehaviour
{
    public Text ComboText;
    public Text ScoreText;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        print("Go to menu");
        Application.Quit();
        // TODO: Going to menu
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Death(int score, int maxCombo)
    {
        gameObject.SetActive(true);
        ComboText.text = "Max combo " + maxCombo.ToString();
        ScoreText.text = "Score " + score.ToString();
    }

    private void MakeTime()
    {
        
    }
}
