using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    [SerializeField] GameObject rg;
    public TextMeshProUGUI scoreTMP;
    public static int score;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        scoreTMP = GetComponent<TextMeshProUGUI>();
        scoreTMP.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        score = rg.GetComponent<RoadGeneratorBehaviour>().GetPlayerDistance();
        // Debug.Log(score);
        if (score > 0 && score%50 == 0)
        {
            StartCoroutine(fadeInScore(score));
            StartCoroutine(fadeOutScore(score));
        }
    }

    public IEnumerator fadeInScore(int scoreToDisplay)
    {
        float currentAlpha = 0.0f;
        while (currentAlpha < 1.0f)
        {
            currentAlpha += Time.deltaTime / fadeDuration;
            scoreTMP.transform.position = Input.mousePosition;
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
