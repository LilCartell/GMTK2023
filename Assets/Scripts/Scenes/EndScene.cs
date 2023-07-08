using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayMusic(SoundManager.Instance.EndMusic);
    }
    public void BackToBeginning()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void CloseGame()
    {
        #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}