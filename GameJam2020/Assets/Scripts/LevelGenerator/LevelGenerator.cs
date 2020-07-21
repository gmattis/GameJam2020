using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

class LevelGenerator
{
    int distanceStartEnd = 10;
    int maxRoomNumber = 35;

    string levelSeed = "AAAAAA";

    public int LevelDepth { get; set; } = 1;

    public System.Random randomGenerator;

    public LevelGenerator()
    {
        levelSeed = GenerateSeed();
        Console.WriteLine($"Level Generator[ Seed: {levelSeed} ]");
    }

    private int Norm1(Vector2Int pos1, Vector2Int pos2)
    {
        return Math.Abs(pos2.x - pos1.x) + Math.Abs(pos2.y - pos1.y);
    }

    private Room PositionToRoom(List<Room> roomList, Vector2Int position)
    {
        foreach (Room room in roomList)
        {
            if (room.Position.Equals(position)) { return room; }
        }
        return null;
    }

    private float Heuristic(List<Room> roomList, Vector2Int position)
    {
        foreach (Room neighbour in FilledNeighbours(roomList, position))
        {
            if (neighbour.neighboursCount == 3) { return 0; }
        }

        if (PositionNeighbours(position).Contains(roomList[1].Position) || PositionNeighbours(position).Contains(roomList[0].Position) || (EmptyNeighbours(roomList, position).Count == 0)) { return 0; }
        else if (EmptyNeighbours(roomList, position).Count == 2) { return 2 + (1 / Norm1(position, Vector2Int.zero)); }
        else { return 1 + (1 / Norm1(position, Vector2Int.zero)); }
    }

    private string GenerateSeed()
    {
        string seed = "";
        System.Random random = new System.Random();
        for (int i = 0; i < 6; i++)
        {
            seed += Encoding.ASCII.GetString(new byte[] { (byte) random.Next(65, 91) });
        }
        return seed;
    }

    private List<Vector2Int> PositionNeighbours(Vector2Int position)
    {
        return new List<Vector2Int>(new Vector2Int[] { position + Vector2Int.up, position + Vector2Int.right, position + Vector2Int.down, position + Vector2Int.left });
    }

    private List<Vector2Int> EmptyNeighbours(Room room)
    {
        List<Vector2Int> emptyNeighboursList = PositionNeighbours(room.Position);
        foreach (Room neighbour in room.neighbours)
        {
            if (neighbour != null && emptyNeighboursList.Contains(neighbour.Position)) { emptyNeighboursList.Remove(neighbour.Position); }
        }
        return emptyNeighboursList;
    }

    private List<Vector2Int> EmptyNeighbours(List<Room> roomList, Vector2Int position)
    {
        List<Vector2Int> emptyNeighbours = PositionNeighbours(position);
        foreach (Room room in roomList)
        {
            if (emptyNeighbours.Contains(room.Position)) { emptyNeighbours.Remove(room.Position); }
        }
        return emptyNeighbours;
    }

    private List<Room> FilledNeighbours(List<Room> roomList, Vector2Int position)
    {
        List<Vector2Int> neighbours = PositionNeighbours(position);
        List<Vector2Int> emptyNeighbours = EmptyNeighbours(roomList, position);
        foreach (Vector2Int emptyRoom in emptyNeighbours)
        {
            if (neighbours.Contains(emptyRoom)) { neighbours.Remove(emptyRoom); }
        }

        List<Room> filledNeighbours = new List<Room>();
        foreach (Vector2Int neighbourPosition in neighbours)
        {
            filledNeighbours.Add(PositionToRoom(roomList, neighbourPosition));
        }
        return filledNeighbours;
    }

    private void UpdateNeighbours(List<Room> roomList, Room room)
    {
        List<Vector2Int> neighboursList = PositionNeighbours(room.Position);

        foreach (Room otherRoom in roomList)
        {
            if (neighboursList.Contains(otherRoom.Position))
            {
                room.AddNeighbour(otherRoom);
                otherRoom.AddNeighbour(room);
            }
        }
    }

    public List<Room> Build()
    {
        // Initialisation
        randomGenerator = new System.Random(BitConverter.ToInt32(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(levelSeed + LevelDepth.ToString())), 0));
        List<Room> roomList = new List<Room>();

        // Placement des salles de debut et de boss
        int bossRoomDX = randomGenerator.Next(distanceStartEnd) * Math.Sign(randomGenerator.NextDouble() - 0.5d);
        int bossRoomDY = (distanceStartEnd - Math.Abs(bossRoomDX)) * Math.Sign(randomGenerator.NextDouble() - 0.5d);
        Vector2Int startRoom = Vector2Int.zero;
        Vector2Int bossRoom = new Vector2Int(bossRoomDX, bossRoomDY);
        roomList.Add(new Room() { ID = 0, Position = startRoom, Size = RoomSize.Tiny, Type = RoomType.Start });
        roomList.Add(new Room() { ID = 1, Position = bossRoom, Size = RoomSize.Tiny, Type = RoomType.Boss });

        // Creation d'un chemin reliant le debut et le boss
        Vector2Int remaining = bossRoom;
        Vector2Int nextRoom = startRoom;
        int id = 2;

        while (Norm1(bossRoom, nextRoom) > 1)
        {
            if (remaining.x == 0 || Math.Abs(remaining.y) > Math.Abs(remaining.x))
            {
                Vector2Int delta = (Math.Sign(remaining.y) == 1 ? Vector2Int.up : Vector2Int.down);
                nextRoom += delta;
                remaining -= delta;
            }
            else if (remaining.y == 0 || Math.Abs(remaining.x) >= Math.Abs(remaining.y))
            {
                Vector2Int delta = (Math.Sign(remaining.x) == 1 ? Vector2Int.right : Vector2Int.left);
                nextRoom += delta;
                remaining -= delta;
            }

            roomList.Add(new Room() { ID = id, Position = nextRoom, Size = RoomSize.Tiny, Type = RoomType.Safe });
            UpdateNeighbours(roomList, roomList.Last());
            id += 1;
        }

        // Initialisation de la selection random
        Dictionary<Vector2Int, float> tileList = new Dictionary<Vector2Int, float>();
        foreach (Room room in roomList)
        {
            foreach (Vector2Int tile in EmptyNeighbours(room))
            {
                if (!tileList.ContainsKey(tile)) { tileList.Add(tile, Heuristic(roomList, tile)); };
            }
        }

        for (int i = distanceStartEnd; i < maxRoomNumber; i++) // On creer chaque salle
        {
            // Calcul des poids de chaque salle
            float sum = tileList.Values.Sum();
            float[] weigths = tileList.Values.ToArray();
            for (int j = 0; j < weigths.Length; j++) { weigths[j] = weigths[j] / sum; }

            // Initialisation de la selection random
            double randomRoomDouble = randomGenerator.NextDouble();
            int randomRoomID = 0;
            float arraySum = weigths[0];

            // Choix de la salle
            while (arraySum < randomRoomDouble)
            {
                randomRoomID += 1;
                arraySum += weigths[randomRoomID];
            }

            // Ajout de la salle
            Vector2Int randomRoom = tileList.ElementAt(randomRoomID).Key;
            roomList.Add(new Room() { ID = i, Position = randomRoom, Size = RoomSize.Tiny, Type = RoomType.Safe });
            UpdateNeighbours(roomList, roomList.Last());

            tileList.Clear();
            foreach (Room room in roomList)
            {
                foreach (Vector2Int tile in EmptyNeighbours(room))
                {
                    if (!tileList.Keys.Contains(tile)) { tileList.Add(tile, Heuristic(roomList, tile)); };
                }
            }
        };

        return roomList;
    }
}