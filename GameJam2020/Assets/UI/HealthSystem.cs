using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public GameObject heart;
    public float heartWidth = 80f;
    public float heartSpacing = 20f;
    public float heartTopMargin = -40f;
    public float heartOffset = 60f;
    public Sprite[] heartSprites;

    private GameObject[] displayedHeartsP1;
    private GameObject[] displayedHeartsP2;

    private void Start()
    {
        displayedHeartsP1 = new GameObject[0];
        displayedHeartsP2 = new GameObject[0];
    }

    void Update()
    {
        GameObject[] players = GameManager.Instance.players;

        if (GameManager.Instance.playersNumber == PlayersNumber.Two)
        {
            PlayerScript player2 = players[1].GetComponent<PlayerScript>();
            int life2 = player2.life;
            int maxLife2 = player2.Statistics.Life;

            int heartNumber2 = (int)Math.Ceiling(maxLife2 / 2f);
            int remainingLife2 = life2;

            GameObject[] newDisplayedHearts2 = new GameObject[heartNumber2];
            for (int i = 0; i < heartNumber2; i++)
            {
                if (i < displayedHeartsP2.Length) { newDisplayedHearts2[i] = displayedHeartsP2[i]; }
                else { newDisplayedHearts2[i] = Instantiate(heart); newDisplayedHearts2[i].transform.SetParent(transform); }

                newDisplayedHearts2[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 - ((i * (heartWidth + heartSpacing)) + heartSpacing + heartOffset), heartTopMargin);
                newDisplayedHearts2[i].GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
                if (remainingLife2 >= 2) { newDisplayedHearts2[i].GetComponent<Image>().sprite = heartSprites[2]; }
                else if (remainingLife2 == 1) { newDisplayedHearts2[i].GetComponent<Image>().sprite = heartSprites[1]; }
                else { newDisplayedHearts2[i].GetComponent<Image>().sprite = heartSprites[0]; }
                remainingLife2 -= 2;
            }

            displayedHeartsP2 = newDisplayedHearts2;
        }

        PlayerScript player = players[0].GetComponent<PlayerScript>();
        int life = player.life;
        int maxLife = player.Statistics.Life;

        int heartNumber = (int)Math.Ceiling(maxLife / 2f);
        int remainingLife = life;

        GameObject[] newDisplayedHearts = new GameObject[heartNumber];
        for (int i = 0; i < heartNumber; i++)
        {
            if (i < displayedHeartsP1.Length) { newDisplayedHearts[i] = displayedHeartsP1[i]; }
            else { newDisplayedHearts[i] = Instantiate(heart); newDisplayedHearts[i].transform.SetParent(transform); }

            newDisplayedHearts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(((i * (heartWidth + heartSpacing)) + heartSpacing + heartOffset), heartTopMargin);
            newDisplayedHearts[i].GetComponent<RectTransform>().localScale = Vector3.one;
            if (remainingLife >= 2) { newDisplayedHearts[i].GetComponent<Image>().sprite = heartSprites[2]; }
            else if (remainingLife == 1) { newDisplayedHearts[i].GetComponent<Image>().sprite = heartSprites[1]; }
            else { newDisplayedHearts[i].GetComponent<Image>().sprite = heartSprites[0]; }
            remainingLife -= 2;
        }

        displayedHeartsP1 = newDisplayedHearts;
    }
}
