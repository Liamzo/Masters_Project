using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {

    public int x;
    public int y;

    public bool blocks;

    public string objectName;
    public string description;

    public int powerLevel;

    public void UpdatePosition(int x, int y) {
        this.x = x;
        this.y = y;

        transform.position = new Vector3(x, y, 0);
    }
    public void UpdatePosition(Vector2 pos) {
        this.x = (int)pos.x;
        this.y = (int)pos.y;

        transform.position = new Vector3(x, y, 0);
    }

    public void SetVisible (bool b) {
        GetComponent<SpriteRenderer>().enabled = b;
    }

    public void MoveTowards(int targetX, int targetY) {
        /*
        int dx = targetX - x;
        int dy = targetY - y;

        float dist = Mathf.Sqrt((dx * dx) + (dy * dy));

        //Normalise to length 1, convert movement to int to stay on map grid
        int normDX = (int)Mathf.RoundToInt(dx / dist);
        int normDY = (int)Mathf.RoundToInt(dy / dist);

        if (GameManager.mapManager.CheckMove(x + normDX, y + normDY)) {
            x += normDX;
            y += normDY;

            transform.position += new Vector3(normDX, normDY, 0);
        }
        */

        Vector2 v = GameManager.mapManager.AstarSearch(x, y, targetX, targetY);
        UpdatePosition(v);
    }

    public float DistanceTo(BaseObject bo) {
        int dx = bo.x - x;
        int dy = bo.y - y;

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }
    public float DistanceTo(int tx, int ty) {
        int dx = tx - x;
        int dy = ty - y;

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }
}
