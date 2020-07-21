using System.Collections.Generic;
using UnityEngine;

public enum RoomOrientation
{
    None,
    Quarter,
    Half,
    MinusQuarter
}

public enum RoomShape
{
    Alone,
    DeadEnd,
    Turn,
    Corridor,
    Tee,
    Cross
}

public enum RoomSize
{
    Tiny,
    Big
}

public enum RoomType
{
    Start,
    Safe,
    Unsafe,
    Boss
}

public class Room
{
    public int ID { get; set; } = 0;
    public Vector2Int Position { get; set; } = new Vector2Int(0, 0);
    public RoomOrientation Orientation { get; set; } = RoomOrientation.None;
    public RoomShape Shape { get; set; } = RoomShape.Alone;
    public RoomSize Size { get; set; } = RoomSize.Tiny;
    public RoomType Type { get; set; } = RoomType.Safe;

    public List<Room> neighbours;

    public int neighboursCount = 0;

    public Room()
    {
        neighbours = new List<Room> ( new Room[4] );
    }

    public void AddNeighbour(Room neighbour)
    {
        neighboursCount += 1;

        if (neighbour.Position == Position + Vector2Int.up) { neighbours[0] = neighbour; }
        if (neighbour.Position == Position + Vector2Int.right) { neighbours[1] = neighbour; }
        if (neighbour.Position == Position + Vector2Int.down) { neighbours[2] = neighbour; }
        if (neighbour.Position == Position + Vector2Int.left) { neighbours[3] = neighbour; }

        if (neighboursCount == 1)
        {
            Shape = RoomShape.DeadEnd;

            if (neighbours[0] != null) { Orientation = RoomOrientation.None; }
            else if (neighbours[1] != null) { Orientation = RoomOrientation.Quarter; }
            else if (neighbours[2] != null) { Orientation = RoomOrientation.Half; }
            else if (neighbours[3] != null) { Orientation = RoomOrientation.MinusQuarter; }
        }
        else if (neighboursCount == 2)
        {
            if ((neighbours[0] != null && neighbours[1] != null) || (neighbours[1] != null && neighbours[2] != null) || (neighbours[2] != null && neighbours[3] != null) || (neighbours[3] != null && neighbours[0] != null))
            {
                Shape = RoomShape.Turn;

                if (neighbours[0] != null)
                {
                    if (neighbours[1] != null) { Orientation = RoomOrientation.None; }
                    else { Orientation = RoomOrientation.MinusQuarter; }
                }
                else
                {
                    if (neighbours[1] != null) { Orientation = RoomOrientation.Quarter; }
                    else { Orientation = RoomOrientation.Half; }
                }
            }
            else
            {
                Shape = RoomShape.Corridor;

                if (neighbours[0] != null) { Orientation = RoomOrientation.None; }
                else { Orientation = RoomOrientation.Quarter; }
            }
        }
        else if (neighboursCount == 3)
        {
            Shape = RoomShape.Tee;

            if (neighbours[0] != null)
            {
                if (neighbours[1] != null)
                {
                    if (neighbours[2] != null) { Orientation = RoomOrientation.None; }
                    else { Orientation = RoomOrientation.MinusQuarter; }
                }
                else { Orientation = RoomOrientation.Half; }
            }
            else { Orientation = RoomOrientation.Quarter; }
        }
        else if (neighboursCount == 4)
        {
            Shape = RoomShape.Cross;
            Orientation = RoomOrientation.None;
        }
    }
}