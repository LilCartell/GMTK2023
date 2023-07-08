using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayMusic(SoundManager.Instance.TitleScreenMusic);
    }

    public void GoToIntroduction()
    {
        SceneManager.LoadScene("IntroductionScene");
    }
}