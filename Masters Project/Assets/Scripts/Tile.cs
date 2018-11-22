using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int x;
    public int y;

    public bool blocksMovement;
    public bool blocksSight;

    public bool explored;
    public bool visible;

    public bool visited; // For pathfinding
    public float minCost = Mathf.Infinity;
    public float distToTarget;
    public Tile nearestToStart;
}
