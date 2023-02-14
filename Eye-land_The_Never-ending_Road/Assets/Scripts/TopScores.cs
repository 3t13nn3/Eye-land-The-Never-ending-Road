using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class TopScores : MonoBehaviour
{
    private Transform container;
    private Transform template;

    public void Awake()
    {
        
        container = GameObject.FindWithTag("container").transform;
        template = GameObject.FindWithTag("template").transform;
        template.gameObject.SetActive(false);


        float templateHeight = 33;

        List<playersScore> l =  EndGameManager.LoadFromJson();
        if (l != null)
        {
            l.Sort((x, y) => y.playerScore.CompareTo(x.playerScore));
            int nbr = l.Count;
            for (int i = 0; i < nbr; i++)
            {
                Transform entryTransform = Instantiate(template, container);
                RectTransform rectTransform = entryTransform.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -templateHeight * (i));
                entryTransform.gameObject.SetActive(true);
                entryTransform.Find("playerName").GetComponent<TextMeshProUGUI>().text = l[i].playerName;
                entryTransform.Find("playerScore").GetComponent<TextMeshProUGUI>().text = l[i].playerScore + "";
                entryTransform.Find("background").gameObject.SetActive((i+1) % 2 ==1);
          
            
            
            
            }   
        }
    }
}