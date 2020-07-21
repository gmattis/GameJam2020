using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Explore,
    Fight,
    Dead
}

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; }

    public int levelDepth = 1;

    public GameObject[] roomsDeadEnd;
    public GameObject[] roomsCorridor;
    public GameObject[] roomsTurn;
    public GameObject[] roomsTee;
    public GameObject[] roomsCross;

    [HideInInspector]
    public Dictionary<Room, GameObject> roomObjects;

    private LevelGenerator levelGenerator;
    private List<Room> roomList;

    // Génération du niveau
    private void Start()
    {
        if (Instance == null) { Instance = this; }

        roomObjects = new Dictionary<Room, GameObject>();

        levelGenerator = new LevelGenerator();
        roomList = levelGenerator.Build();
        BuildLevel();
    }

    public void NextLevel()
    {
        levelDepth += 1;
        foreach (GameObject room in roomObjects.Values)
        {
            Destroy(room);
        }
        roomObjects.Clear();

        levelGenerator.LevelDepth = levelDepth;
        roomList = levelGenerator.Build();
    }

    public void BuildLevel()
    {
        foreach (Room room in roomList)
        {
            GameObject roomObject = null;
            int roomID = 0;

            if (room.Shape == RoomShape.DeadEnd)
            {
                roomID = levelGenerator.randomGenerator.Next(0, roomsDeadEnd.Length);
                roomObject = roomsDeadEnd[roomID];
            }
            else if (room.Shape == RoomShape.Corridor)
            {
                roomID = levelGenerator.randomGenerator.Next(0, roomsCorridor.Length);
                roomObject = roomsCorridor[roomID];
            }
            else if (room.Shape == RoomShape.Turn)
            {
                roomID = levelGenerator.randomGenerator.Next(0, roomsTurn.Length);
                roomObject = roomsTurn[roomID];
            }
            else if (room.Shape == RoomShape.Tee)
            {
                roomID = levelGenerator.randomGenerator.Next(0, roomsTee.Length);
                roomObject = roomsTee[roomID];
            }
            else if (room.Shape == RoomShape.Cross)
            {
                roomID = levelGenerator.randomGenerator.Next(0, roomsCross.Length);
                roomObject = roomsCross[roomID];
            }

            GameObject newTile = Instantiate(roomObject, Vector2to3(room.Position), Quaternion.Euler(0f, OrientationToAngle(room.Orientation), 0f));
            newTile.GetComponent<RoomScript>().RoomInfo = room;
            roomObjects.Add(room, newTile);

            if (room.Type == RoomType.Start)
            {
                foreach (Renderer renderer in newTile.GetComponentsInChildren<Renderer>())
                {
                    renderer.material.color = Color.green;
                }
            }
            else if (room.Type == RoomType.Boss)
            {
                foreach (Renderer renderer in newTile.GetComponentsInChildren<Renderer>())
                {
                    renderer.material.color = Color.red;
                }
            }
        }
    }

    private Vector3 Vector2to3(Vector2 vector)
    {
        return new Vector3(vector.x * 20, 0, vector.y * 20);
    }

    private float OrientationToAngle(RoomOrientation orientation)
    {
        switch (orientation)
        {
            case RoomOrientation.None: return 0f;
            case RoomOrientation.Quarter: return 90f;
            case RoomOrientation.Half: return 180f;
            case RoomOrientation.MinusQuarter: return 270f;
            default: return 0f;
        }
    }
}
