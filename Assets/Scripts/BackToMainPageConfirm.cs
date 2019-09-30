using System.Collections;
using UnityEngine;

public class BackToMainPageConfirm : MonoBehaviour {
    [SerializeField] private Animation canvas;
    [SerializeField] private GameObject field;

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudio;

    public void Show ()
    {
        gameObject.SetActive(true);
        GetComponent<Animation>().Play("Showing");
        clickAudio.Play();
    }

    public void YesOnClick()
    {
        StartCoroutine(YesEnumerator());
        clickAudio.Play();
    }

    public void YesOnClick_temp()
    {
        StartCoroutine(YesEnumerator_ForPVE_temp());
        clickAudio.Play();
    }

    private IEnumerator YesEnumerator ()
    {
        GetComponent<Animation>().Play("HideYes");
        yield return new WaitForSeconds(1.5f);
        canvas.Play("HideInterface");
        field.GetComponent<PlayField>().Clear();
        yield return new WaitForSeconds(.75f);
        GameManager.GoToMainPage();
    }

    private IEnumerator YesEnumerator_ForPVE_temp ()
    {
        GetComponent<Animation>().Play("HideYes");
        yield return new WaitForSeconds(1.5f);
        field.GetComponent<PlayField>().Clear();
        field.GetComponent<PVE>().ToMainPage(false);
    }

    public void Hide ()
    {
        GetComponent<Animation>().Play("Hide");
        clickAudio.Play();
    }

    public void Deactivate () { gameObject.SetActive(false); }
    public void GoToMainPage() { GameManager.GoToMainPage(); }
}
