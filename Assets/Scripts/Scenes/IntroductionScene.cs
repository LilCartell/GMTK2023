using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionScene : MonoBehaviour
{
    public GameObject[] IntroSlides;

    public float TimeBeforePressAnyKeyPrompt = 5f;
    public GameObject PressAnyKeyPrompt;

    private int _currentSlideIndex = -1;
    private float _timeElapsed = 0;

    private void Awake()
    {
        PressAnyKeyPrompt.SetActive(false);
        foreach(var slide in IntroSlides)
        {
            slide.SetActive(false);
        }
        NextSlide();
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
            NextSlide();
        }
    }

    private void NextSlide()
    {
        PressAnyKeyPrompt.SetActive(false);
        _timeElapsed = 0;
        if (_currentSlideIndex >= 0)
            IntroSlides[_currentSlideIndex].SetActive(false);
        ++_currentSlideIndex;
        if (_currentSlideIndex < IntroSlides.Length)
            IntroSlides[_currentSlideIndex].SetActive(true);
        else
            GoToLevelsScene();
    }

    private void GoToLevelsScene()
    {
        SceneManager.LoadScene("LevelsScene");
    }
}