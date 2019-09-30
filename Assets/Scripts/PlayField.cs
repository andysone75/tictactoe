using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class PlayField : MonoBehaviour
{
    public enum Players { Circle, Cross }

    [SerializeField] private GameObject canvas;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Sprite crossSprite;
    [SerializeField] private GameObject[] Cells;
    
    public Players turn = Players.Circle;
    public static Dictionary<Players, Sprite> sprites = new Dictionary<Players, Sprite>();
    private GameObject[][] rows = new GameObject[8][]; // All combinatioins of rows that determine the winner
    public int moves = 0;
    public bool isGameLoop = false;

    private void Start()
    {
        if (sprites.Count == 0)
        {
            sprites.Add(Players.Circle, circleSprite);
            sprites.Add(Players.Cross,  crossSprite);
        }
        
        rows[0] = new GameObject[] { Cells[0], Cells[1], Cells[2] };
        rows[1] = new GameObject[] { Cells[3], Cells[4], Cells[5] };
        rows[2] = new GameObject[] { Cells[6], Cells[7], Cells[8] };
        rows[3] = new GameObject[] { Cells[0], Cells[3], Cells[6] };
        rows[4] = new GameObject[] { Cells[1], Cells[4], Cells[7] };
        rows[5] = new GameObject[] { Cells[2], Cells[5], Cells[8] };
        rows[6] = new GameObject[] { Cells[0], Cells[4], Cells[8] };
        rows[7] = new GameObject[] { Cells[2], Cells[4], Cells[6] };

        isGameLoop = true;
    }

    public void Move (GameObject cell)
    {
        var image = cell.GetComponent<Image>();
        if (image.sprite == null && isGameLoop)
        {
            image.sprite = sprites[turn];
            cell.GetComponent<Animation>().Play("Showing");
            moves++;

            var result = isWinMove(cell);
            if (result.Length != 0 || moves == 9)
            {
                var pve = GameObject.FindGameObjectWithTag("PVE");
                var pvp = GameObject.FindGameObjectWithTag("PVP");
                var botGame = GetComponent<BotGame>();

                if (pve != null)
                {
                    if (result.Length == 0) pve.GetComponent<PVE>().GameResult(result);
                    else pve.GetComponent<PVE>().GameResult(result, turn);
                }
                if (pvp != null) pvp.GetComponent<PVP>().GameResult(result, turn);
                if (botGame != null) botGame.Stop();

                turn = Players.Circle;
                isGameLoop = false;
                moves = 0;

                List<Image> images = new List<Image>();
                for (var i = 0; i < Cells.Length; i++)
                {
                    var _image = Cells[i].GetComponent<Image>();
                    if (Array.IndexOf(result, Cells[i]) == -1 && _image.sprite != null) images.Add(_image);
                }
                StartCoroutine(MakeCellsGray(images));

                if (result.Length != 0) StartCoroutine(MakeCellsRed(result));

                StartCoroutine(Restart());
            }
            else turn = turn == Players.Circle ? Players.Cross : Players.Circle;
        }
        else
        {
            var anim = cell.GetComponent<Animation>();
            if (!anim.isPlaying) anim.Play("ClickClosedCell");
        }
    }

    private GameObject[] isWinMove(GameObject cell)
    {
        for (var i = 0; i < rows.Length; i++)
        {
            if (Array.IndexOf(rows[i], cell) == -1) continue;
            for (var j = 0; j < rows[i].Length; j++)
            {
                if (rows[i][j].GetComponent<Image>().sprite != sprites[turn]) break;
                if (j == rows[i].Length - 1) return rows[i];
            }
        }

        return new GameObject[0];
    }

    private IEnumerator MakeCellsGray (List<Image> images)
    {
        Color color = new Color(0.3882353f, 0.4313726f, 0.4470588f);
        for (var i = 0f; i < 1; i += 0.05f)
        {
            foreach (Image image in images)
                image.color = Color.Lerp(image.color, color, i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator MakeCellsRed (GameObject[] cells)
    {
        Color color = new Color(0.8392157f, 0.1882353f, 0.1921569f);
        for (var i = 0f; i < 1; i += 0.05f)
        {
            foreach (GameObject cell in cells)
            {
                var image = cell.GetComponent<Image>();
                image.color = Color.Lerp(image.color, color, i);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(3);
        Clear();

        var botGame = GetComponent<BotGame>();
        if (botGame != null)
        {
            yield return new WaitForSeconds(1);
            botGame.StartGame();
        }

        yield return new WaitForSeconds(.7f);
        isGameLoop = true;

        var pve = GetComponent<PVE>();
        if (pve != null && pve.bot == Players.Circle) MakeBotMove(pve.bot); 
    }

    public void Clear()
    {
        foreach (GameObject cell in Cells)
            if (cell.GetComponent<Image>().sprite != null) cell.GetComponent<Animation>().Play("Hide");
    }

    public void MakeBotMove(Players skin)
    {
        Players rivalSkin = skin == Players.Circle ? Players.Cross : Players.Circle;

        // Check for possible winning moves
        foreach (GameObject[] row in rows)
        {
            int count = 0;
            foreach(GameObject cell in row)
                if (cell.GetComponent<Image>().sprite == sprites[skin]) count++;
            if (count == 2)
            {
                foreach (GameObject cell in row)
                {
                    if (cell.GetComponent<Image>().sprite == null)
                    {
                        Move(cell);
                        return;
                    }
                }
            }
        }

        // Check possible winning moves of the opponent
        foreach (GameObject[] row in rows)
        {
            int count = 0;
            foreach (GameObject cell in row)
                if (cell.GetComponent<Image>().sprite == sprites[rivalSkin]) count++;
            if (count == 2)
            {
                foreach (GameObject cell in row)
                {
                    if (cell.GetComponent<Image>().sprite == null)
                    {
                        Move(cell);
                        return;
                    }
                }
            }
        }

        // Otherwise, move to any FREE cell
        List<GameObject> freeCells = new List<GameObject>();
        foreach (GameObject cell in Cells)
            if (cell.GetComponent<Image>().sprite == null) freeCells.Add(cell);
        Move(freeCells[UnityEngine.Random.Range(0, freeCells.Count)]);
    }
}
