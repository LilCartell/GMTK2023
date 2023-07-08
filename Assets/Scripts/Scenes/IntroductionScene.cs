using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionScene : MonoBehaviour
{
    public float TimeBeforePressAnyKeyPrompt = 5f;
    public GameObject PressAnyKeyPrompt;

    private float _timeElapsed = 0;

    private void Awake()
    {
        PressAnyKeyPrompt.SetActive(false);
    }

    private void Start()
    {
        SoundManager.Instance.PlayMusic(SoundManager.Instance.IntroductionMusic);
    }

    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > TimeBeforePressAnyKeyPrompt && PressAnyKeyPrompt != null)
            PressAnyKeyPrompt.SetActive(true);
        if(Input.anyKeyDown)
        {
            GoToLevelsScene();
        }
    }

    private void GoToLevelsScene()
    {
        SceneManager.LoadScene("LevelsScene");
    }
}