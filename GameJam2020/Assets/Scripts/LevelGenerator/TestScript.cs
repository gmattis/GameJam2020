using System;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject cube;

    static char RoomTypeToChar(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Start: return 'D';
            case RoomType.Boss: return 'B';
            case RoomType.Safe: return 'S';
            case RoomType.Unsafe: return 'U';
            default: return '0';
        }
    }

    static Color RoomTypeToColor(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Start: return Color.red;
            case RoomType.Boss: return Color.yellow;
            case RoomType.Safe: return Color.green;
            case RoomType.Unsafe: return Color.blue;
            default: return Color.grey;
        }
    }

    static void DisplayRoomList(List<Room> roomList)
    {
        int[] maxCoordinates = new int[] { 0, 0, 0, 0 };
        foreach (Room room in roomList)
        {
            if (maxCoordinates[0] > room.Position.x) { maxCoordinates[0] = room.Position.x; }
            if (maxCoordinates[1] < room.Position.y) { maxCoordinates[1] = room.Position.y; }
            if (maxCoordinates[2] < room.Position.x) { maxCoordinates[2] = room.Position.x; }
            if (maxCoordinates[3] > room.Position.y) { maxCoordinates[3] = room.Position.y; }
        }

        int widthY = maxCoordinates[1] - maxCoordinates[3] + 1;
        int widthX = maxCoordinates[2] - maxCoordinates[0] + 1;

        char[,] chrMatrix = new char[widthX, widthY];

        foreach (Room room in roomList)
        {
            chrMatrix[room.Position.x - maxCoordinates[0], room.Position.y - maxCoordinates[3]] = RoomTypeToChar(room.Type);
        }

        for (int i = 0; i < chrMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < chrMatrix.GetLength(1); j++)
            {
                Console.Write(chrMatrix[i, j]);
            }
            Console.WriteLine();
        }
    }

    void CreateRoomList(List<Room> roomList)
    {
        foreach (Room room in roomList)
        {
            GameObject mCube = GameObject.Instantiate(cube, new Vector3(room.Position.x, room.Position.y, 0), Quaternion.identity);
            Renderer renderer = mCube.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", RoomTypeToColor(room.Type));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelGenerator levelGenerator = new LevelGenerator();
        List<Room> roomList = levelGenerator.Build();
        foreach (Room room in roomList)
        {
            Debug.Log(String.Format("Node [{0}]: {1} [{2}, {3}]", room.ID, room.Type, room.Position.x, room.Position.y));
        }
        DisplayRoomList(roomList);
        CreateRoomList(roomList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}