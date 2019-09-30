using System.Collections;
using UnityEngine;

public class BotGame : MonoBehaviour
{
    private PlayField field;
    private bool flag = false;

    private void Start() { field = GetComponent<PlayField>(); }

    public void StartGame() { StartCoroutine(Play()); }
    public void Stop() { flag = false; }

    private IEnumerator Play()
    {
        flag = true;
        while (flag)
        {
            field.MakeBotMove(field.turn);
            yield return new WaitForSeconds(1);
        }
    }
}
