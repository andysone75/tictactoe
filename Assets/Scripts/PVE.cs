using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PVE : MonoBehaviour
{
    public PlayField.Players user = PlayField.Players.Circle;
    public PlayField.Players bot  = PlayField.Players.Cross;

    [SerializeField] private Animation canvas;
    [SerializeField] private Animator[] lifesAnims = new Animator[3];
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Text highScoreTxt;

    [Header("Audio")]
    public AudioSource successMoveAudio;
    public AudioSource failMoveAudio;
    public AudioSource winAudio;
    public AudioSource loseAudio;
    public AudioSource tieAudio;

    [Header("Text")]
    [SerializeField] private Text[] texts;

    private int lifes = 3;
    public int Lifes
    {
        get { return lifes; }
        set
        {
            int old = lifes;

            if (value < 0) lifes = 0;
            else if (value > 3) lifes = 3;
            else lifes = value;

            if (old > lifes)
            {
                lifesAnims[lifes].SetBool("Full", false);
                lifesAnims[lifes].SetBool("Empty", true);
            }
            else if (old < lifes)
            {
                lifesAnims[lifes - 1].SetBool("Empty", false);
                lifesAnims[lifes - 1].SetBool("Full", true);
            }

            if (lifes == 0) Lose();
        }
    }

    private int score = 0;
    private int Score
    {
        get { return score; }
        set { scoreTxt.text = (score = value).ToString(); }
    }

    private static int highScore;
    public static int HighScore
    {
        get { return highScore; }
        set
        {
            highScore = value;
            PlayerPrefs.SetInt("HighScore", value);
        }
    }

    private bool firstStep = false;

    private void Start()
    {
        MainPage.SetGameVolume();
        GameManager.SetTheme();

        if (GameManager.fromMainPage)
        {
            canvas.Play("PVE_Start");
            GameManager.fromMainPage = false;
        }

        if (PlayerPrefs.HasKey("HighScore")) HighScore = PlayerPrefs.GetInt("HighScore");
        else HighScore = 0;
        highScoreTxt.text = LangSystem.lng.pve[0] + HighScore.ToString();

        for (int i = 1; i < texts.Length; i++)
            texts[i].text = LangSystem.lng.pve[i];
    }

    public void Move(GameObject cell)
    {
        if (!firstStep)
        {
            Score = 0;
            firstStep = true;
        }

        var field = GetComponent<PlayField>();
        int moves = field.moves;
        field.Move(cell);
        if (field.moves != 0 && moves != field.moves)
        {
            successMoveAudio.Play();
            field.MakeBotMove(bot);
        }
        else failMoveAudio.Play();
    }

    public void GameResult(GameObject[] result, PlayField.Players winner = PlayField.Players.Circle)
    {
        if (result.Length == 0) Tie();
        else if (user == winner) Win(); // If result contains user images
        else Lose();
    }

    public void Win()
    {
        Score++;
        Lifes++;
        if (score >= 12)
        {
            user = PlayField.Players.Cross;
            bot  = PlayField.Players.Circle;
        }
        else if (score >= 7)
        {
            user = Random.Range(0, 2) == 0 ? PlayField.Players.Circle : PlayField.Players.Cross;
            bot = user == PlayField.Players.Circle ? PlayField.Players.Cross : PlayField.Players.Circle;
        }
        winAudio.Play();
    }

    public void Lose()
    {
        canvas.Play("win");
        if (score > HighScore)
        {
            HighScore = score;
            highScoreTxt.text = LangSystem.lng.pve[0] + HighScore.ToString();
        }
        loseAudio.Play();
    }

    public void Tie()
    {
        Lifes--;
        tieAudio.Play();
    }

    public void Restart()
    {
        user = PlayField.Players.Circle;
        bot = PlayField.Players.Cross;

        var field = GetComponent<PlayField>();
        field.Clear();
        field.isGameLoop = true;

        canvas.Play("Restart");
        while (Lifes != 3) Lifes++;
        Score = 0;
    }

    public void ToMainPage(bool isLose) { StartCoroutine(GoToMainPage(isLose)); }

    private IEnumerator GoToMainPage(bool isLose)
    {
        if (isLose)
        {
            canvas.Play("Restart");
            yield return new WaitForSeconds(1.4f);
        }
        canvas.Play("PVEgoToMainPage");
        yield return new WaitForSeconds(1);
        GameManager.GoToMainPage();
    }
}
