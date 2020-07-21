using System.Collections.Generic;
using UnityEngine;

public enum PlayersNumber
{
    One,
    Two
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject levelBuilder;
    public GameObject musicManager;
    public GameObject[] players;

    // Effets GameObject
    public GameObject[] weaponsProjectiles;
    public GameObject impact;

    public PlayersNumber playersNumber = PlayersNumber.One;

    public PlayerState playerState = PlayerState.Explore;

    void Awake()
    {
        if (Instance == null) { Instance = this; }

        Instantiate(levelBuilder);
        Instantiate(musicManager);

        if (playersNumber == PlayersNumber.One)
        {
            Destroy(players[1]);
            players = new GameObject[] { players[0] };
        }
    }
}
