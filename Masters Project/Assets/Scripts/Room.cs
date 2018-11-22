using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {
    public int x1;
    public int y1;
    public int x2;
    public int y2;

    public Room(int x, int y, int w, int h) {
        x1 = x;
        y1 = y;
        x2 = x + w;
        y2 = y + h;
    }

    public Vector2 Centre () {
        Vector2 c = new Vector2();
        c.x = (x1 + x2) / 2;
        c.y = (y1 + y2) / 2;

        return c;
    }

    public bool Intersect(Room room) {
        // Returns true if the rooms intersect
        return (x1 <= room.x2 && x2 >= room.x1 && y1 <= room.y2 && y2 >= room.y1);
    }

}
