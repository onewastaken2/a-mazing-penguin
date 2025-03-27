using UnityEngine.UI;
using System.Collections;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    //THIRD BUILD ONLY
    [SerializeField] private GameObject howToPlay;
    [SerializeField] private Button pauseButton;
    

    private void Awake()
    {
        StartCoroutine(Countdown());
    }


    public void StartPlaying()
    {
        Time.timeScale = 1f;
        gameObject.GetComponent<PauseMenu>().enabled = true;
        pauseButton.interactable = true;
        howToPlay.SetActive(false);
    }


    IEnumerator Countdown()
    {
        float _timer = 3f;

        while(_timer > 0.0f)
        {
            yield return new WaitForSeconds(1);
            _timer--;
        }
        gameObject.GetComponent<PauseMenu>().enabled = false;
        pauseButton.interactable = false;
        Time.timeScale = 0f;
        howToPlay.SetActive(true);
    }
}
