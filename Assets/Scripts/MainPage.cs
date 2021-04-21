using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainPage : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private GameObject field;
    [SerializeField] private Animation canvas;

    [Header("Other Digital Content")]
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private VideoPlayer logoVideo;

    [Header("Sound")]
    [SerializeField] private Image sound;
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;
    private Dictionary<bool, Sprite> soundStates = new Dictionary<bool, Sprite>();

    [Header("Theme")]
    [SerializeField] private Color[] themeColors;
    [SerializeField] private Theme[] themes;
    static int curentTheme = 0;

    [Header("Text")]
    [SerializeField] private Text[] texts;
    

    private void Start()
    {
        soundStates.Add(true,  soundOn);
        soundStates.Add(false, soundOff);
        sound.sprite = soundStates[GameManager.SoundState];
        SetGameVolume();

        if (!GameManager.isGameStarted)
        {
            StartCoroutine(GameStart());
            GameManager.isGameStarted = true;
            GameManager.theme = themeColors[0];
        }
        else canvas.Play("ReturnHome");

        var highScore = PlayerPrefs.HasKey("HighScore") ? PlayerPrefs.GetInt("HighScore") : 0;
        for (var i = 1; i < themes.Length; i++)
            if (themes[i].GetGoal() <= highScore) themes[i].UnBlock();
        themes[curentTheme].Pick();

        GameManager.SetTheme();
        LangSwitch();
    }

    private IEnumerator GameStart()
    {
        logoVideo.Play();
        canvas.Play("Start");
        yield return new WaitForSeconds(8);
        field.GetComponent<BotGame>().StartGame();
        Options.Block = false;
    }

    public static void SetGameVolume()
    {
        GameObject[] audios = GameObject.FindGameObjectsWithTag("Audio");
        foreach (GameObject audio in audios)
            audio.GetComponent<AudioSource>().volume = GameManager.SoundState ? 1 : 0;
    }

    // Buttons
    public void LangSwitch()
    {
        for (int i = 0; i < texts.Length; i++)
            texts[i].text = LangSystem.lng.mainPage[i];
    }

    public void SoundSwitch()
    {
        GameManager.SoundState = !GameManager.SoundState;
        sound.sprite = soundStates[GameManager.SoundState];
        SetGameVolume();
    }

    public void ChangeTheme(GameObject theme)
    {
        var index = int.Parse(theme.name);
        if (!themes[index].open)
        {
            theme.GetComponent<Animation>().Play("Blocked");
            return;
        }
        else
        {
            GameManager.theme = themeColors[index];
            var gameManager = GetComponent<GameManager>();
            gameManager.StartCoroutine(gameManager.SetThemeIE());
            themes[index].Pick();
            themes[curentTheme].UnPick();
            curentTheme = index;
        }
    }

    public void StartPVP()
    {
        if (canvas.isPlaying) return;
        StartCoroutine(IeStartPVP());
        clickAudio.Play();
    }

    public void StartPVE()
    {
        if (canvas.isPlaying) return;
        StartCoroutine(IeStartPVE());
        clickAudio.Play();
    }

    private IEnumerator IeStartPVP()
    {
        field.GetComponent<BotGame>().Stop();
        field.GetComponent<PlayField>().Clear();
        canvas.Play("HideInterface");
        yield return new WaitForSeconds(1);
        GameManager.StartPVP();
    }

    private IEnumerator IeStartPVE()
    {
        field.GetComponent<BotGame>().Stop();
        field.GetComponent<PlayField>().Clear();
        canvas.Play("HideInterface");
        yield return new WaitForSeconds(1);
        GameManager.StartPVE();
    }
}
