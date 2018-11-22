using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject[] tileTypes;
    public GameObject[] enemyTypes;
    public GameObject[] itemTypes;

    GameObject[,] mapData;
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> items = new List<GameObject>();

    public GameObject stairs;

    // Will be controlled by the AGD
    int ROOM_MAX_SIZE = 10;
    int ROOM_MIN_SIZE = 6;
    int MAX_ROOMS = 30;

    int MAX_ROOM_MONSTERS = 3;
    int MAX_ROOM_ITEMS = 2;

    int mapWidth;
    int mapHeight;

    public GameObject playerObject;
    public Player player;

    void Awake() {
        player = (Player)playerObject.GetComponent<Player>();
    }

    public List<Room> GenerateRooms (int genWidth, int genHeight, int noRooms) {
        List<Room> rooms = new List<Room>();

        mapWidth = genWidth;
        mapHeight = genHeight;

        mapData = new GameObject[mapWidth, mapHeight];

        while (rooms.Count < noRooms) {
            int attempts = 0;

            while (attempts < 5) {
                int w = Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE + 1);
                int h = Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE + 1);
                int x = Random.Range(0, mapWidth - w - 1);
                int y = Random.Range(0, mapHeight - h - 1);

                Room r = new Room(x, y, w, h);

                // Check to see if new room intersects any existing room
                bool failed = false;
                for (int j = 0; j < rooms.Count; j++) {
                    if (r.Intersect(rooms[j])) {
                        failed = true;
                        attempts++;
                        break;
                    }
                }

                if (!failed) {
                    //CreateRoom(r);
                    rooms.Add(r);
                    break;
                }
            }

            if (attempts >= 5) {
                // Failed to create room
                // Increase map size
                mapWidth += 10;
                mapHeight += 10;

                mapData = new GameObject[mapWidth, mapHeight];
            }
        }

        // Fill map with Wall tiles
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                mapData[x, y] = tileTypes[1];
            }
        }

        foreach (Room r in rooms) {
            CreateRoom(r);
        }

        return rooms;
    }

    public void GenerateMapData() {
        mapHeight = 40;
        mapWidth = 40;

        mapData = new GameObject[mapWidth, mapHeight];

        Room[] rooms = new Room[MAX_ROOMS];
        int noRooms = 0;

        // Fill map with Wall tiles
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                mapData[x, y] = tileTypes[1];
            }
        }

        for (int i = 0; i < MAX_ROOMS; i++) {
            int w = Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE + 1);
            int h = Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE + 1);
            int x = Random.Range(0, mapWidth - w - 1);
            int y = Random.Range(0, mapHeight - h - 1);

            Room r = new Room(x, y, w, h);

            // Check to see if new room intersects any existing room
            bool failed = false;
            for (int j = 0; j < noRooms; j++) {
                if (r.Intersect(rooms[j])) {
                    failed = true;
                    break;
                }
            }

            if (!failed) {
                CreateRoom(r);

                // Position player in the centre of the first room for now
                if (noRooms == 0) {
                    //Move player
                    //player.UpdatePosition((r.x1 + r.x2) / 2, (r.y1 + r.y2) / 2);
                    player.UpdatePosition(r.Centre());

                } else {
                    // Connect new room via tunnel
                    Room prevRoom = rooms[noRooms - 1];

                    // Randomly get points the rooms should be connected by
                    int r1x = Random.Range(r.x1+1, r.x2);
                    int r1y = Random.Range(r.y1+1, r.y2);
                    int r2x = Random.Range(prevRoom.x1+1, prevRoom.x2);
                    int r2y = Random.Range(prevRoom.y1+1, prevRoom.y2);

                    if (Random.Range(0,2) == 1) {
                        // Create horizontal tunnel first, then vertical
                        Create_H_Tunnel(r2x, r1x, r2y);
                        Create_V_Tunnel(r2y, r1y, r1x);
                    } else {
                        Create_V_Tunnel(r2y, r1y, r2x);
                        Create_H_Tunnel(r2x, r1x, r1y);
                    }
                }

                PlaceEnemies(r);
                PlaceItems(r);

                rooms[noRooms] = r;
                noRooms++;
            }
        }

        // Place stairs at centre of last room
        stairs.GetComponent<BaseObject>().UpdatePosition((rooms[noRooms - 1].x1 + rooms[noRooms - 1].x2) / 2, (rooms[noRooms - 1].y1 + rooms[noRooms - 1].y2) / 2);

        // Generate the visual map
        GenerateMapVisual();

        CheckLineOfSight();
    }

    public void PlaceEnemies (Room room) {
        int numEnemies = Random.Range(0, MAX_ROOM_MONSTERS + 1);

        for (int i = 0; i < numEnemies; i++) {
            int x = Random.Range(room.x1 + 1, room.x2);
            int y = Random.Range(room.y1 + 1, room.y2);

            // Place random monster, update this later
            GameObject go = Instantiate(enemyTypes[0], new Vector3(x, y, 0), Quaternion.identity);

            BaseObject bo = go.GetComponent<BaseObject>();
            bo.UpdatePosition(x, y);

            enemies.Add(go);
        }
    }
    public void PlaceEnemies(Room room, List<GameObject> roomEnemies) {
        foreach (GameObject enemy in roomEnemies) {
            int x = Random.Range(room.x1 + 1, room.x2);
            int y = Random.Range(room.y1 + 1, room.y2);

            BaseObject bo = enemy.GetComponent<BaseObject>();
            bo.UpdatePosition(x, y);

            enemies.Add(enemy);
        }
    }

    public void PlaceItems (Room room) {
        int numItems = Random.Range(0, MAX_ROOM_ITEMS + 1);

        for (int i = 0; i < numItems; i++) {
            int x = Random.Range(room.x1 + 1, room.x2);
            int y = Random.Range(room.y1 + 1, room.y2);

            // Place random item, update later
            GameObject go = Instantiate(itemTypes[0], new Vector3(x, y, 0), Quaternion.identity);

            BaseObject bo = go.GetComponent<BaseObject>();
            bo.UpdatePosition(x, y);

            items.Add(go);
        }
    }
    public void PlaceItems(Room room, List<GameObject> roomItems) {
        foreach (GameObject item in roomItems) {
            int x = Random.Range(room.x1 + 1, room.x2);
            int y = Random.Range(room.y1 + 1, room.y2);

            BaseObject bo = item.GetComponent<BaseObject>();
            bo.UpdatePosition(x, y);

            items.Add(item);
        }
    }

    void CreateRoom(Room room) {
        for (int i = room.x1+1; i < room.x2; i++) {
            for (int j = room.y1+1; j < room.y2; j++) {
                mapData[i, j] = tileTypes[0];
            }

        }
    }

    void Create_H_Tunnel(int x1, int x2, int y) {
        for (int i = Mathf.Min(x1,x2); i <= Mathf.Max(x1,x2); i++) {
            mapData[i, y] = tileTypes[0];
        }
    }
    void Create_V_Tunnel(int y1, int y2, int x) {
        for (int i = Mathf.Min(y1, y2); i <= Mathf.Max(y1, y2); i++) {
            mapData[x, i] = tileTypes[0];
        }
    }

    public void ConnectRooms(Room r1, Room r2, int width) {
        // Randomly get points the rooms should be connected by
        int r1x = Random.Range(r1.x1 + 1, r1.x2);
        int r1y = Random.Range(r1.y1 + 1, r1.y2);
        int r2x = Random.Range(r2.x1 + 1, r2.x2);
        int r2y = Random.Range(r2.y1 + 1, r2.y2);

        if (Random.Range(0, 2) == 1) {
            // Create horizontal tunnel first, then vertical
            for (int i = 0; i < width; i++) {
                Create_H_Tunnel(r2x, r1x, r2y + i);
                Create_V_Tunnel(r2y, r1y, r1x + i);
            }

        } else {
            for (int i = 0; i < width; i++) {
                Create_V_Tunnel(r2y, r1y, r2x + i);
                Create_H_Tunnel(r2x, r1x, r1y + i);
            }
        }
    }

    public void GenerateMapVisual() {
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                GameObject go = Instantiate(mapData[x,y], new Vector3(x,y,0), Quaternion.identity);
                go.GetComponent<Tile>().x = x;
                go.GetComponent<Tile>().y = y;
                mapData[x, y] = go;
                mapData[x, y].SetActive(false);
            }
        }
            
    }

    public Vector2 AstarSearch (int x, int y, int tx, int ty) {
        // Reset all tiles on map
        foreach (GameObject go in mapData) {
            Tile t = go.GetComponent<Tile>();
            t.visited = false;
            t.minCost = Mathf.Infinity;

            int dx = tx - t.x;
            int dy = ty - t.y;

            t.distToTarget = Mathf.Sqrt((dx * dx) + (dy * dy));
        }

        Tile start = mapData[x, y].GetComponent<Tile>();
        Tile target = mapData[tx, ty].GetComponent<Tile>();

        List<Tile> nodes = new List<Tile>();

        start.minCost = 0;
        nodes.Add(start);

        Tile bestTile = null;
        float bestDist = Mathf.Infinity;

        int k = 0;
        do {
            bestTile = null;
            bestDist = Mathf.Infinity;

            // Get best tile in unvisted tiles
            foreach (Tile t in nodes) {
                if (t.minCost + t.distToTarget < bestDist) {
                    bestTile = t;
                    bestDist = t.minCost + t.distToTarget;            
                }
            }

            nodes.Remove(bestTile);

            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    int tileX = bestTile.x + i;
                    int tileY = bestTile.y + j;
                    Tile t = mapData[tileX, tileY].GetComponent<Tile>();

                    bool blocked = false;
                    foreach (GameObject enemy in enemies) {
                        BaseObject bo = enemy.GetComponent<BaseObject>();
                        if (bo.x == tileX && bo.y == tileY && bo.blocks == true) {
                            blocked = true;
                            continue;
                        }
                    }

                    if (t.visited == true || t.blocksMovement == true || blocked == true) {
                        continue;
                    }
                    if (bestTile.minCost + Mathf.Sqrt((i * i) + (j * j)) < t.minCost) {
                        t.minCost = bestTile.minCost + Mathf.Sqrt((i * i) + (j * j));
                        t.nearestToStart = bestTile;

                        if (nodes.Contains(t) == false) {
                            nodes.Add(t);
                        }
                    }
                }
            }

            bestTile.visited = true;

            if (bestTile == target) {
                break;
            }
            k++;
        } while (nodes.Count > 0);

        List<Tile> shortestPath = new List<Tile>();
        shortestPath.Add(target);
        Tile prevNode = target.nearestToStart;

        while (prevNode != start) {
            shortestPath.Add(prevNode);
            if (prevNode.nearestToStart == null) {
                return new Vector2(start.x, start.y); // If no path then stand still
            }
            prevNode = prevNode.nearestToStart;
        }

        shortestPath.Reverse();

        return new Vector2(shortestPath[0].x, shortestPath[0].y);
    }

    public bool CheckMove(int x, int y) {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight) {
            Tile tile = (Tile)mapData[x, y].GetComponent<Tile>();

            if (tile.blocksMovement == true) {
                return false;
            } else {
                foreach (GameObject enemy in enemies) {
                    BaseObject eb = enemy.GetComponent<BaseObject>();

                    if (eb.x == x && eb.y == y) {
                        return false;
                    }
                }

                // Can move;
                return true;
            }
        } else {
            // Not within map bounds
            return false;
        }
    }

    public bool PlayerCheckAttack (int x, int y) {
        foreach (GameObject enemy in enemies) {
            EnemyBase eb = enemy.GetComponent<EnemyBase>();

            if (eb.x == x && eb.y == y) {
                // Hit enemy
                GameManager.gameManager.selectedEnemyObject = enemy;
                player.AttackTarget(eb);

                return true;
            }
        }

        return false;
        //return CheckMove(x, y);
    }

    public GameObject PickUpItem (int x, int y) {
        // Check if any items are at the given co-ords

        foreach (GameObject item in items) {
            ItemBase ib = item.GetComponent<ItemBase>();

            if (x == ib.x && y == ib.y) {
                // Remove item from list of items on map
                items.Remove(item);
                ib.SetVisible(false);

                return item;
            }
        }

        return null;
    }

    public void DropItem (GameObject item) {
        items.Add(item);
        item.GetComponent<BaseObject>().SetVisible(true);
    }

    public void CheckLineOfSight() {
        for (int i = 0; i <360; i++) {
            float xRad = Mathf.Cos((float)i*0.01745f);
            float yRad = Mathf.Sin((float)i*0.01745f);

            CalculateSight(xRad, yRad);
        }

        // Check what enemies we can see
        foreach (GameObject enemy in enemies) {
            BaseObject bo = enemy.GetComponent<BaseObject>();

            Tile tile = mapData[bo.x, bo.y].GetComponent<Tile>();

            bo.SetVisible(tile.visible);
        }

        // Check what items we can see
        foreach (GameObject item in items) {
            BaseObject bo = item.GetComponent<BaseObject>();

            Tile tile = mapData[bo.x, bo.y].GetComponent<Tile>();

            // Need to make sure only 1 item/enemy is drawn on each tile to avoid overlap
            bo.SetVisible(tile.visible);
        }

        BaseObject boStairs = stairs.GetComponent<BaseObject>();
        boStairs.SetVisible(mapData[boStairs.x, boStairs.y].GetComponent<Tile>().visible);
    }

    void CalculateSight(float x, float y) {
        float ox = player.x + 0.5f;
        float oy = player.y + 0.5f;

        for (int i = 0; i <= player.sightRange; i++) {
            if (ox >= 0 && ox < mapWidth && oy >= 0 && oy < mapHeight) {
                mapData[(int)ox, (int)oy].SetActive(true);
                mapData[(int)ox, (int)oy].GetComponent<Tile>().visible = true;
                mapData[(int)ox, (int)oy].GetComponent<Tile>().explored = true;

                if (mapData[(int)ox, (int)oy].GetComponent<Tile>().blocksSight == true) {
                    return;
                }

                ox += x;
                oy += y;
            } else {
                return;
            }
        }
    }

    public void EnemyTurn() {
        foreach (GameObject enemy in enemies) {
            EnemyBase eb = enemy.GetComponent<EnemyBase>();

            if (eb.stunned > 0) {
                eb.stunned--;
                GameManager.gameManager.NewMessage("The " + eb.objectName + " is stunned");
            } else {
                eb.Turn();
            }
        }
    }

    public List<GameObject> EnemiesInRange(int x, int y, int range) {
        List<GameObject> enemiesInRange = new List<GameObject>();

        foreach (GameObject enemy in enemies) {
            BaseObject bo = enemy.GetComponent<BaseObject>();
            if (bo.DistanceTo(x,y) <= range && !(bo.x == x && bo.y == y)) {
                // Won't return enemies on the given tile co-ords
                enemiesInRange.Add(enemy);
            }
        }

        return enemiesInRange;
    }

    public bool TileExplored (int x, int y) {

        Tile tile = mapData[x, y].GetComponent<Tile>();

        return tile.explored;

    }

    public void CheckStairs (int x, int y) {
        BaseObject bo = stairs.GetComponent<BaseObject>();

        if (x == bo.x && y == bo.y) {
            GameManager.gameManager.NextLevel();
        }
    }

    public void CleanLevel () {
        // Destory the previous level before a new one is created
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                Destroy(mapData[x, y]);
            }
        }

        foreach (GameObject enemy in enemies) {
            Destroy(enemy);
        }
        enemies.Clear();

        foreach (GameObject item in items) {
            Destroy(item);
        }
        items.Clear();
    }

    public void EnemyDeath (GameObject enemy) {
        enemies.Remove(enemy);
    }
}
