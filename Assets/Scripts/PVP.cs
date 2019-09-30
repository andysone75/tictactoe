using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVP : MonoBehaviour {
    [SerializeField] private Text Winner;
    [SerializeField] private Animation canvas;

    [Header("Win Score")]
    [SerializeField] private GameObject WinScoreObject;
    [SerializeField] private Text WinScoreValue;
    private int minValue = 1;
    private int winScore = 1;
    public int WinScore
    {
        get
        {
            return winScore;
        }
        set
        {
            if (value < minValue) winScore = minValue;
            else if (value > 99) winScore = 99;
            else winScore = value;
            WinScoreValue.text = winScore.ToString();
        }
    }

    [Header("Score")]
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Image turn;

    [Header("Mode")]
    [SerializeField] private Image modeImg;
    [SerializeField] private Sprite infinitySprite;
    [SerializeField] private Sprite scoreSprite;
    public enum Modes { Infinity, Score }
    private Modes mode = Modes.Infinity;
    private Dictionary<Modes, Sprite> modeSprites = new Dictionary<Modes, Sprite>();

    private Dictionary<PlayField.Players, int> score = new Dictionary<PlayField.Players, int> {
        { PlayField.Players.Circle, 0 },
        { PlayField.Players.Cross, 0 }
    };

    [Header("Audio")]
    [SerializeField] private AudioSource successMoveAudio;
    [SerializeField] private AudioSource failMoveAudio;
    [SerializeField] private AudioSource winAudio;
    [SerializeField] private AudioSource clickAudio;

    [Header("Text")]
    [SerializeField] private Text[] texts;

    private void Start()
    {
        MainPage.SetGameVolume();
        GameManager.SetTheme();

        modeSprites.Add(Modes.Infinity, infinitySprite);
        modeSprites.Add(Modes.Score, scoreSprite);

        GameObject.FindGameObjectWithTag("Canvas").GetComponent<Animation>().Play("ShowInterface");

        for (int i = 0; i < texts.Length; i++)
            texts[i].text = LangSystem.lng.pvp[i];
    }

    public void Move (GameObject cell)
    {
        var field = GetComponent<PlayField>();
        var moves = field.moves;
        GetComponent<PlayField>().Move(cell);
        if (moves == field.moves) failMoveAudio.Play();
        else successMoveAudio.Play();
        SetTurn();
    }

    public void GameResult(GameObject[] result, PlayField.Players winner)
    {
        if (result.Length != 0)
        {
            winAudio.Play();
            score[winner]++;
            minValue = Mathf.Max(score[PlayField.Players.Circle], score[PlayField.Players.Cross]) + 1;
            RenderScore();
            if (score[winner] == WinScore && mode == Modes.Score)
            {
                string winnerTxt = string.Empty;
                if (winner == PlayField.Players.Circle)
                {
                    if (PlayerPrefs.GetString("Language") == "ru_RU") winnerTxt = "Нолики";
                    else if (PlayerPrefs.GetString("Language") == "en_US") winnerTxt = "Circles";
                }
                else
                {
                    if (PlayerPrefs.GetString("Language") == "ru_RU") winnerTxt = "Крестики";
                    else if (PlayerPrefs.GetString("Language") == "en_US") winnerTxt = "Crosses";
                }

                Winner.text = LangSystem.lng.pvp[1] + winnerTxt;
                GameObject.FindGameObjectWithTag("Canvas").GetComponent<Animation>().Play("win");
                winAudio.Play();
            }
        }
    }

    public void SetTurn () { turn.sprite = PlayField.sprites[GetComponent<PlayField>().turn]; }
    private void RenderScore() { scoreTxt.text = string.Format("{0} : {1}", score[PlayField.Players.Circle], score[PlayField.Players.Cross]); }

    public void SwitchMode()
    {
        clickAudio.Play();
        if (mode == Modes.Infinity)
        {
            mode = Modes.Score;
            WinScore = minValue;
        }
        else mode = Modes.Infinity;
        WinScoreObject.SetActive(mode == Modes.Score);
        modeImg.sprite = modeSprites[mode];
    }

    public void IncrementWinScore () { WinScore++; }
    public void DecrementWinScore()  { WinScore--; }

    public void Restart()
    {
        canvas.Play("Restart");
        SwitchMode();
        score[PlayField.Players.Circle] = score[PlayField.Players.Cross] = 0;
        RenderScore();
        minValue = WinScore = 1;
    }

    public void GoToMainPage() { StartCoroutine(toToMainPageEnumerator()); }

    private IEnumerator toToMainPageEnumerator()
    {
        SwitchMode();
        canvas.Play("Restart");
        canvas.PlayQueued("HideInterface");
        yield return new WaitForSeconds(2.1f);
        GameManager.GoToMainPage();
    }
}
