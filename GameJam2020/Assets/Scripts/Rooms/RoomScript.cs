using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RoomState
{
    Focused,
    Unfocused
}

public class RoomScript : MonoBehaviour
{
    public RoomState roomState = RoomState.Focused;
    public Room RoomInfo { get; set; }

    private float teleportDistance = 3f;

    private float unfocusTime = 0f;

    private GameObject WallUp;
    private GameObject WallRight;
    private GameObject WallDown;
    private GameObject WallLeft;
    private GameObject WallUpReverse;
    private GameObject WallRightReverse;
    private GameObject WallDownReverse;
    private GameObject WallLeftReverse;

    private GameObject[] activeWalls;
    private GameObject[] inactiveWalls;

    private GameObject[] teleportWalls;

    private GameObject[] players;
    private List<EnemyScript> enemies;

    private bool inWallTrigger = true;

    private void Start()
    {
        // Récupération des joueurs et ennemis
        players = GameManager.Instance.players;
        enemies = GetComponentsInChildren<EnemyScript>().ToList();

        // Récupération des murs
        WallUp = transform.GetChild(1).gameObject;
        WallRight = transform.GetChild(2).gameObject;
        WallDown = transform.GetChild(3).gameObject;
        WallLeft = transform.GetChild(4).gameObject;
        WallUpReverse = transform.GetChild(5).gameObject;
        WallRightReverse = transform.GetChild(6).gameObject;
        WallDownReverse = transform.GetChild(7).gameObject;
        WallLeftReverse = transform.GetChild(8).gameObject;

        // Murs actifs (visibles) et inactifs (invisibles)
        if (RoomInfo.Orientation == RoomOrientation.None) {
            activeWalls = new GameObject[] { WallUp, WallRight, WallDownReverse, WallLeftReverse };
            inactiveWalls = new GameObject[] { WallDown, WallLeft, WallUpReverse, WallRightReverse };
        }
        else if (RoomInfo.Orientation == RoomOrientation.Quarter) {
            activeWalls = new GameObject[] { WallLeft, WallUp, WallRightReverse, WallDownReverse };
            inactiveWalls = new GameObject[] { WallRight, WallDown, WallLeftReverse, WallUpReverse };
        }
        else if (RoomInfo.Orientation == RoomOrientation.Half) {
            activeWalls = new GameObject[] { WallDown, WallLeft, WallUpReverse, WallRightReverse };
            inactiveWalls = new GameObject[] { WallUp, WallRight, WallDownReverse, WallLeftReverse};
        }
        else if (RoomInfo.Orientation == RoomOrientation.MinusQuarter) {
            activeWalls = new GameObject[] { WallRight, WallDown, WallLeftReverse, WallUpReverse };
            inactiveWalls = new GameObject[] { WallLeft, WallUp, WallRightReverse, WallDownReverse };
        }

        foreach (GameObject child in inactiveWalls)
        {
            foreach(Renderer renderer in child.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
            foreach(Collider collider in child.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        // Murs téléporteurs
        if (RoomInfo.Shape == RoomShape.DeadEnd) { teleportWalls = new GameObject[] { WallUp }; }
        else if (RoomInfo.Shape == RoomShape.Corridor) { teleportWalls = new GameObject[] { WallUp, WallDown }; }
        else if (RoomInfo.Shape == RoomShape.Turn) { teleportWalls = new GameObject[] { WallUp, WallRight }; }
        else if (RoomInfo.Shape == RoomShape.Tee) { teleportWalls = new GameObject[] { WallUp, WallRight, WallDown }; }
        else if (RoomInfo.Shape == RoomShape.Cross) { teleportWalls = new GameObject[] { WallUp, WallRight, WallDown, WallLeft }; }

        if (RoomInfo.Type != RoomType.Start) { Unfocused(); } else { Focused(); }
    }

    private void FixedUpdate()
    {
        if (roomState == RoomState.Focused)
        {
            if (GameManager.Instance.playerState == PlayerState.Explore)
            {
                for (int i = 0; i < teleportWalls.Length; i++)
                {
                    float[] playersDistance = new float[players.Length];
                    for (int j = 0; j < players.Length; j++)
                    {
                        playersDistance[j] = Vector3.Distance(players[j].transform.position, teleportWalls[i].transform.position);
                    }

                    if (inWallTrigger && playersDistance.Max() > teleportDistance) { inWallTrigger = false; }
                    else if (!inWallTrigger && playersDistance.Max() <= teleportDistance)
                    {
                        TeleportPlayer(LevelBuilder.Instance.roomObjects[FindNeighbourRoom(teleportWalls[i])]);
                        break;
                    }
                }
            }
            else if (GameManager.Instance.playerState == PlayerState.Fight)
            {
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (enemies[i] == null) { enemies.RemoveAt(i); }
                }
                if (enemies.Count == 0)
                {
                    GameManager.Instance.playerState = PlayerState.Explore;
                    Animator[] animators = GetComponentsInChildren<Animator>();
                    foreach (Animator animator in animators) { animator.SetBool("open", true); }
                }
            }
        }
        else
        {
            float colorTint = Mathf.Min((unfocusTime - Time.time) * 2, 1f);
            Color renderColor = new Color(colorTint, colorTint, colorTint, 1f);

            // Teinture des éléments de la pièce
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = renderColor;
            }

            if (Time.time >= unfocusTime)
            {
                inWallTrigger = true;
                gameObject.GetComponent<RoomScript>().enabled = false;
            }
        }

        GameManager.Instance.playerState = PlayerState.Fight;
    }

    public void Focused()
    {
        roomState = RoomState.Focused;

        Color renderColor = new Color(1f, 1f, 1f, 1f);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            //renderer.enabled = true;
            renderer.material.color = renderColor;
        }

        // Désactivation du rendu des murs voisins
        /*foreach (GameObject wall in teleportWalls)
        {
            Room neighbourRoom = FindNeighbourRoom(wall);
            GameObject neighbourObject = LevelBuilder.GetComponent<LevelBuilder>().roomObjects[neighbourRoom];

            GameObject disabledWall = neighbourObject.GetComponent<RoomScript>().activeWalls[(NeighbourRoomIndex(wall) + 2) % 4];
            foreach (Renderer renderer in disabledWall.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }*/

        if (enemies.Count > 0)
        {
            GameManager.Instance.playerState = PlayerState.Fight;
            foreach (EnemyScript enemy in enemies) { if (enemy != null) { enemy.enabled = true; } }
        }
        else
        {
            GameManager.Instance.playerState = PlayerState.Explore;
            Animator[] animators = GetComponentsInChildren<Animator>();
            foreach (Animator animator in animators) { animator.SetBool("open", true); }
        }

        gameObject.GetComponent<RoomScript>().enabled = true;
    }

    private int NeighbourRoomIndex(GameObject facingWall)
    {
        int idRoom = 0;
        if (facingWall == WallUp) { idRoom = 0; }
        else if (facingWall == WallRight) { idRoom = 1; }
        else if (facingWall == WallDown) { idRoom = 2; }
        else if (facingWall == WallLeft) { idRoom = 3; }

        int idOrientation = 0;
        if (RoomInfo.Orientation == RoomOrientation.None) { idOrientation = 0; }
        else if (RoomInfo.Orientation == RoomOrientation.Quarter) { idOrientation = 1; }
        else if (RoomInfo.Orientation == RoomOrientation.Half) { idOrientation = 2; }
        else if (RoomInfo.Orientation == RoomOrientation.MinusQuarter) { idOrientation = 3; }

        return (idRoom + idOrientation) % 4;
    }

    private Room FindNeighbourRoom(GameObject facingWall)
    {
        return RoomInfo.neighbours[NeighbourRoomIndex(facingWall)];
    }

    private void TeleportPlayer(GameObject roomObject)
    {
        if (GameManager.Instance.playersNumber == PlayersNumber.One)
        {
            players[0].transform.position = transform.position + (roomObject.transform.position - transform.position) * 0.7f + Vector3.up * 0.81f;
        }
        else
        {
            players[0].transform.position = transform.position + (roomObject.transform.position - transform.position) * 0.7f - transform.right + Vector3.up * 0.81f;
            players[1].transform.position = transform.position + (roomObject.transform.position - transform.position) * 0.7f + transform.right + Vector3.up * 0.81f;
        }
        roomObject.GetComponent<RoomScript>().Focused();
        Unfocused();
    }

    private void Unfocused()
    {
        roomState = RoomState.Unfocused;
        unfocusTime = Time.time + 0.5f;

        foreach (EnemyScript enemy in enemies)
        {
            if (enemy != null) { enemy.enabled = false; }
        }
    }
}
