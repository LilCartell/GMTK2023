using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public void GoToIntroduction()
    {
        SceneManager.LoadScene("IntroductionScene");
    }
}