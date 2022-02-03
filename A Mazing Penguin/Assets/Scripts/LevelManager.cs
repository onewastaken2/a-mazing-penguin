using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Light mainLight;   //References main light for scene transitions


    private void Awake()
    {
        mainLight.intensity = 0;
    }


    private void Start()
    {
        StartCoroutine(FadeOut());
    }


    //Detects for player
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(SceneManager.GetActiveScene().buildIndex != 4)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }


    //Player has reach end of level
    //Scene goes dims to black
    IEnumerator FadeIn()
    {
        float _duration = 2f;
        float _interval = 0.1f;
        
        while(_duration > 0.0f)
        {
            mainLight.intensity -= 0.05f;
            _duration -= _interval;
            yield return new WaitForSeconds(_interval);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    //Player has started a new level
    //Scene brightens up from black screen
    IEnumerator FadeOut()
    {
        float _duration = 2f;
        float _interval = 0.1f;

        while(_duration > 0.0f)
        {
            mainLight.intensity += 0.05f;
            _duration -= _interval;
            yield return new WaitForSeconds(_interval);
        }
    }
}
