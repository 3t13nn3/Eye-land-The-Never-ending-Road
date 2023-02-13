using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public RoadGeneratorBehaviour rg;
    public GameObject car;
    public TextMeshProUGUI scoreTMP;
    public static int score;


    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<RoadGeneratorBehaviour>();
        scoreTMP = GetComponent<TextMeshProUGUI>();
        car = GameObject.Find("Car");
        scoreTMP.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        rg = GetComponent<RoadGeneratorBehaviour>();
        Debug.Log(rg);
        // score = rg.GetPlayerDistance();
        // if (score > 0 && score%100 == 0)
        // {
        //     StartCoroutine(fadeInScore(score));
        //     StartCoroutine(fadeOutScore(score));
        // }
    }

    public IEnumerator fadeInScore(int scoreToDisplay)
    {
        float currentAlpha = 0.0f;
        while (currentAlpha < 1.0f)
        {
            currentAlpha += Time.deltaTime / fadeDuration;
            scoreTMP.text = scoreToDisplay.ToString();
            scoreTMP.alpha = currentAlpha;
            yield return null;
        }
    }
 
    public IEnumerator fadeOutScore(int scoreToDisplay)
    {
        float currentAlpha = 1.0f;
        while (currentAlpha > 0.0f)
        {
            currentAlpha -= Time.deltaTime / fadeDuration;
            scoreTMP.text = scoreToDisplay.ToString();
            scoreTMP.alpha = currentAlpha;
            yield return null;
        }
    }
}
