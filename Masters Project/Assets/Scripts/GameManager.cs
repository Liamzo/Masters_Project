using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour {

    string logPath = "Assets/Resources/log.txt";

    public static GameManager gameManager;
    public static MapManager mapManager;

    public GameObject playerObject;
    public Player player;
    public GameObject startingDagger; // Start the player off with a dagger

    public bool playersTurn = true;

    public GameObject selectedEnemyObject;

    public int dungeonLevel = 1;

    /*
     * 
     * User Interface Objects
     * 
    */

    public GameObject dungeonLevelUI;
    
    // Player Stats
    public GameObject healthSlider;
    public GameObject healthNum;
    public GameObject playerLevel;
    public GameObject xpSlider;
    public GameObject xpNum;
    public GameObject playerPower;

    // Selected Enemy Stats
    public GameObject enemyStatsPanel;
    public GameObject enemyName;
    public GameObject enemyHealthSlider;
    public GameObject enemyHealthNum;
    public GameObject enemyPower;
    public GameObject enemyDescription;
    public GameObject enemyTargetHighlight;

    // Message Log
    public GameObject messageLogUI;
    GameObject[] messageDisplay;
    public string[] messages;
    int oldestMessage = 0;
    int newestMessage = -1;


    // Enemy and Item Types
    public List<GameObject> enemyTypes;
    public List<GameObject> itemTypes;
    public List<GameObject> mainHandTypes;
    public List<GameObject> offHandTypes;
    public List<GameObject> headTypes;
    public List<GameObject> bodyTypes;


    // Level Generation
    int noRooms;


    int playerSkill = 1000;
    int playerEquipmentPower = 0;

    int levelDifficultyTarget = 2000;
    int levelDifficultyActual = 0;
    int difficultyIncrese = 500;
    int maxEnemyDifficulty = 0;

    int levelItemPowerTarget = 0;
    int levelItemPowerActual = 0;

    int EPIBase = 500;
    int EPI = 0;

    List<LevelData> levelDataList;

    // Use this for initialization
    void Awake () {
        gameManager = this;

        mapManager = GetComponent<MapManager>();

        player = (Player)playerObject.GetComponent<Player>();

        messages = new string[5];
    }

    void Start () {
        levelDataList = new List<LevelData>();

        // Get all the text objects that display messages from the message log UI
        messageDisplay = new GameObject[messageLogUI.transform.childCount];

        for (int i = 0; i < messageLogUI.transform.childCount; i++) {
            messageDisplay[i] = messageLogUI.transform.GetChild(i).gameObject;
        }
        
        GameObject go = Instantiate(startingDagger, new Vector3(-10, -10, 0), Quaternion.identity);
        go.GetComponent<EquipmentBase>().Use();

        LevelGen();

        UpdatePlayerUI();
    }

    void LevelGen () {

        levelDifficultyTarget = 1500 + (difficultyIncrese * dungeonLevel);

        levelDifficultyActual = 0;
        levelItemPowerActual = 0;


        playerEquipmentPower = player.EquipmentPowerLevel();

        LevelData lastLevel;

        if (levelDataList.Count == 0) {
            EPI = 500;
            lastLevel = null;
        } else {
            lastLevel = levelDataList[levelDataList.Count - 1];

            EPI = EPIBase - (lastLevel.API - lastLevel.EPI);
        }
        
        noRooms = 5;
        if (EPI > 600) {
            noRooms++;
        }

        List<Room> rooms = mapManager.GenerateRooms(40, 40, noRooms);
        Room startRoom = rooms[0];
        player.UpdatePosition(startRoom.Centre()); // Player in first room

        List<Room> orderedRooms = new List<Room>();

        // Place stairs down in furthest away room
        Room farRoom = null;
        float roomDist = 0;

        for (int i = 0; i < noRooms; i++) {
            farRoom = null;
            roomDist = Mathf.Infinity;

            foreach (Room r in rooms) {
                float dx = r.Centre().x - startRoom.Centre().x;
                float dy = r.Centre().y - startRoom.Centre().y;

                float distance = Mathf.Sqrt((dx * dx) + (dy * dy));

                if (distance < roomDist) {
                    farRoom = r;
                    roomDist = distance;
                }
            }

            orderedRooms.Add(farRoom);
            rooms.Remove(farRoom);
        }

        mapManager.stairs.GetComponent<BaseObject>().UpdatePosition(orderedRooms[orderedRooms.Count - 1].Centre());

        for (int i = 1; i < orderedRooms.Count; i++) {
            mapManager.ConnectRooms(orderedRooms[i - 1], orderedRooms[i], 2);
        }



        // Place enemies
        maxEnemyDifficulty = (int)(levelDifficultyTarget / 10);

        if (EPI > 500) {
            levelDifficultyTarget -= (EPI - 500) / 2;
        }

        float roomDifficulty = levelDifficultyTarget / (noRooms - 1);

        for (int i = noRooms - 1; i > 0; i--) {
            int curDifficulty = 0;
            List<GameObject> roomEnemies = new List<GameObject>();

            int hallWidth = 1;
            
            while (curDifficulty < roomDifficulty && levelDifficultyActual < levelDifficultyTarget) {
                int j = Random.Range(0, enemyTypes.Count);

                GameObject enemy = enemyTypes[j];

                if (enemy.GetComponent<BaseObject>().powerLevel <= maxEnemyDifficulty) {
                    curDifficulty += enemy.GetComponent<BaseObject>().powerLevel;
                    levelDifficultyActual += enemy.GetComponent<BaseObject>().powerLevel;

                    GameObject go = Instantiate(enemy, new Vector3(0, 0, 0), Quaternion.identity);

                    roomEnemies.Add(go);

                    if (go.GetComponent<BaseObject>().objectName.Equals("Wolf")) {
                        int rand = Random.Range(0, 100);

                        if (rand < 30) {
                            hallWidth = 2;
                            levelDifficultyActual += 50;
                        }
                    } else if (go.GetComponent<BaseObject>().objectName.Equals("Kobold")) {
                        // Spawn another kobold
                        int rand = Random.Range(0, 100);

                        if (rand < 70) {
                            curDifficulty += enemy.GetComponent<BaseObject>().powerLevel;
                            levelDifficultyActual += enemy.GetComponent<BaseObject>().powerLevel;

                            go = Instantiate(enemy, new Vector3(0, 0, 0), Quaternion.identity);

                            roomEnemies.Add(go);
                            levelDifficultyActual += 50;
                        }
                    }
                }
                
            }

            mapManager.PlaceEnemies(orderedRooms[i], roomEnemies);
            
            mapManager.ConnectRooms(orderedRooms[i - 1], orderedRooms[i], hallWidth);
            

        }

        // Place items

        levelItemPowerTarget = (levelDifficultyActual + EPI) - (playerSkill + player.TotalPowerLevel());

        if (EPI > 500) {
            levelItemPowerTarget -= (EPI - 500) / 2;
        }

        GameObject equipmentToSpawn = null;
        List<GameObject> equipmentSpawned = new List<GameObject>();

        // Decide whether to spawn new equipment
        if (lastLevel == null) {
            // Spawn equipment
            equipmentToSpawn = ChooseEquipementToSpawn();
        } else {
            if (playerEquipmentPower == lastLevel.equipmentPower) {
                // Spawn
                equipmentToSpawn = ChooseEquipementToSpawn();
            } else if (playerEquipmentPower / player.TotalPowerLevel() < 0.3) {
                // Spawn
                equipmentToSpawn = ChooseEquipementToSpawn();
            }
        }


        if (equipmentToSpawn != null) {
            equipmentSpawned.Add(equipmentToSpawn);

            int rand = Random.Range(0, noRooms - 1);

            levelItemPowerActual += equipmentToSpawn.GetComponent<BaseObject>().powerLevel;

            GameObject go = Instantiate(equipmentToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
            List<GameObject> items = new List<GameObject>();
            items.Add(go);
            mapManager.PlaceItems(orderedRooms[rand], items);
        }


        float roomPower = (levelItemPowerTarget - levelItemPowerActual) / noRooms;

        int healthPotionsInPlay = player.NumberOfHealthPotions();
        
        for (int i = 0; i < noRooms; i++) {
            int currentPower = 0;

            List<GameObject> roomItems = new List<GameObject>();

            while (currentPower < roomPower && levelItemPowerActual < levelItemPowerTarget) {

                // Spawn a health potion if the player is in need
                if (healthPotionsInPlay == 0) {
                    GameObject item = itemTypes[0];
                    GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                    currentPower += go.GetComponent<BaseObject>().powerLevel;
                    levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                    roomItems.Add(go);

                    healthPotionsInPlay++;
                    continue;
                } else {

                    int chance = (int)(100 - (int)((float)player.healthCur / player.HealthMax)) / (healthPotionsInPlay * 4);

                    int rand = Random.Range(0, 100);
                    if (rand < chance) {
                        GameObject item = itemTypes[0];
                        GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                        currentPower += go.GetComponent<BaseObject>().powerLevel;
                        levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                        roomItems.Add(go);
                        healthPotionsInPlay++;
                        continue;
                    }
                }
        
                int dice = Random.Range(0, 100);

                if (dice < 85) {
                    // Decide which items to spawn
                    int rand = Random.Range(1, itemTypes.Count);

                    GameObject item = itemTypes[rand];
                    GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                    currentPower += go.GetComponent<BaseObject>().powerLevel;
                    levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                    roomItems.Add(go);

                } else {
                    // Spawn equipment
                    int randSlot = Random.Range(0, 4);

                    int power = player.PowerInSlot(randSlot);

                    if (randSlot == 0) {
                        foreach (GameObject item in mainHandTypes) {
                            if (item.GetComponent<BaseObject>().powerLevel > power) {
                                if (equipmentSpawned.Contains(item)) {
                                    // Dont spawn equipment that has already been spawned
                                    break;
                                }
                                GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                                currentPower += go.GetComponent<BaseObject>().powerLevel;
                                levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                                roomItems.Add(go);
                                break;
                            }
                        }
                    } else if (randSlot == 1) {
                        foreach (GameObject item in offHandTypes) {
                            if (item.GetComponent<BaseObject>().powerLevel > power) {
                                if (equipmentSpawned.Contains(item)) {
                                    // Dont spawn equipment that has already been spawned
                                    break;
                                }
                                GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                                currentPower += go.GetComponent<BaseObject>().powerLevel;
                                levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                                roomItems.Add(go);
                                break;
                            }
                        }
                    } else if (randSlot == 2) {
                        foreach (GameObject item in headTypes) {
                            if (item.GetComponent<BaseObject>().powerLevel > power) {
                                if (equipmentSpawned.Contains(item)) {
                                    // Dont spawn equipment that has already been spawned
                                    break;
                                }
                                GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                                currentPower += go.GetComponent<BaseObject>().powerLevel;
                                levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                                roomItems.Add(go);
                                break;
                            }
                        }
                    } else if (randSlot == 3) {
                        foreach (GameObject item in bodyTypes) {
                            if (item.GetComponent<BaseObject>().powerLevel > power) {
                                if (equipmentSpawned.Contains(item)) {
                                    // Dont spawn equipment that has already been spawned
                                    break;
                                }
                                GameObject go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);

                                currentPower += go.GetComponent<BaseObject>().powerLevel;
                                levelItemPowerActual += go.GetComponent<BaseObject>().powerLevel;
                                roomItems.Add(go);
                                break;
                            }
                        }
                    }

                }
            }

            mapManager.PlaceItems(orderedRooms[i], roomItems);
        }


        mapManager.GenerateMapVisual();
        mapManager.CheckLineOfSight();

       
        UpdatePlayerUI();


        LevelData levelData = new LevelData();
        levelData.floor = dungeonLevel;
        levelData.selfPower = player.SelfPowerLevel();
        levelData.equipmentPower = player.EquipmentPowerLevel();
        levelData.itemPower = player.InventoryPowerLevel();
        levelData.totalPower = player.TotalPowerLevel();
        levelData.skillLevel = playerSkill;
        levelData.levelDifficulty = levelDifficultyActual;
        levelData.levelItems = levelItemPowerActual;
        levelData.noRooms = noRooms;
        levelData.EPI = EPI;

        levelDataList.Add(levelData);
    }

    public GameObject ChooseEquipementToSpawn () {
        // Spawn equipment
        Vector2Int v = player.FindNeededEquipment();

        if (v.x == 0) {
            // Main Hand
            foreach (GameObject item in mainHandTypes) {
                EquipmentBase eb = item.GetComponent<EquipmentBase>();

                if (eb.powerLevel > v.y) {
                    return item;
                }
            }

        } else if (v.x == 1) {
            // Off Hand
            foreach (GameObject item in offHandTypes) {
                EquipmentBase eb = item.GetComponent<EquipmentBase>();

                if (eb.powerLevel > v.y) {
                    return item;
                }
            }
        } else if (v.x == 2) {
            // Head
            foreach (GameObject item in headTypes) {
                EquipmentBase eb = item.GetComponent<EquipmentBase>();

                if (eb.powerLevel > v.y) {
                    return item;
                }
            }
        } else if (v.x == 3) {
            // Body
            foreach (GameObject item in bodyTypes) {
                EquipmentBase eb = item.GetComponent<EquipmentBase>();

                if (eb.powerLevel > v.y) {
                    return item;
                }
            }
        }


        return null;
    }

    // Update is called once per frame
    void Update () {
		if (playersTurn == true) {
            return;
        }
      

        // Take enemies turns
        mapManager.EnemyTurn();

        mapManager.CheckLineOfSight();

        UpdatePlayerUI();
        UpdateEnemyUI();

        playersTurn = true;
	}

    public void NextLevel () {
        NewMessage("You take a moment to catch your breath before continuing deeper into the dungeon");
        dungeonLevel++;

        Text dungeonLevelText = dungeonLevelUI.GetComponent<Text>();
        dungeonLevelText.text = "Floor: " + dungeonLevel;

        player.Heal(player.HealthMax / 2);
        UpdatePlayerUI();

        mapManager.CleanLevel();

        levelDataList[levelDataList.Count - 1].API = (player.TotalPowerLevel() - levelDataList[levelDataList.Count - 1].totalPower);

        playerSkill += levelDataList[levelDataList.Count - 1].API - levelDataList[levelDataList.Count - 1].EPI;

        if (dungeonLevel > 5) {
            // End game
            EndGame();
        }

        LevelGen();
    }

    public void UpdatePlayerUI () {
        // Health
        Slider sliderHealth = healthSlider.GetComponent<Slider>();
        sliderHealth.maxValue = player.HealthMax;
        sliderHealth.value = player.healthCur;

        Text healthText = healthNum.GetComponent<Text>();
        healthText.text = player.healthCur + "/" + player.HealthMax;

        // Level and XP
        Text playerLevelText = playerLevel.GetComponent<Text>();
        playerLevelText.text = "Level: " + player.level;

        Slider sliderXP = xpSlider.GetComponent<Slider>();
        sliderXP.maxValue = player.xpToNextLevel;
        sliderXP.value = player.xpCur;

        Text xpText = xpNum.GetComponent<Text>();
        xpText.text = player.xpCur + "/" + player.xpToNextLevel;

        // Power
        Text powerText = playerPower.GetComponent<Text>();
        powerText.text = "Att: " + player.Attack + "   " + "Def: " + player.Defense;
    }

    public void UpdateEnemyUI () {
        if (selectedEnemyObject != null) {
            enemyStatsPanel.SetActive(true);
        } else {
            enemyStatsPanel.SetActive(false);
            enemyTargetHighlight.transform.position = new Vector3(-100, -100, 0); // Move off screen
            return;
        }

        EnemyBase enemy = selectedEnemyObject.GetComponent<EnemyBase>();

        enemyTargetHighlight.transform.position = new Vector3(enemy.x, enemy.y, 0);

        // Name
        Text name = enemyName.GetComponent<Text>();
        name.text = enemy.objectName;

        // Health
        Slider slider = enemyHealthSlider.GetComponent<Slider>();
        slider.maxValue = enemy.HealthMax;
        slider.value = enemy.healthCur;

        Text healthText = enemyHealthNum.GetComponent<Text>();
        healthText.text = enemy.healthCur + "/" + enemy.HealthMax;

        // Power
        Text powerText = enemyPower.GetComponent<Text>();
        powerText.text = "Att: " + enemy.Attack + "   " + "Def: " + enemy.Defense;

        // Description
        Text descriptionText = enemyDescription.GetComponent<Text>();
        descriptionText.text = enemy.description;
    }

    public void NewMessage (string message) {
        float textYPos = 0.0f;

        messageDisplay[oldestMessage].GetComponent<Text>().text = message; // Replace oldest message with new one
        newestMessage = oldestMessage;
        oldestMessage = (oldestMessage + 1) % 5;

        int counter = newestMessage;

        for (int i = 0; i < 5; i++) {
            messageDisplay[counter].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, textYPos);

            textYPos += messageDisplay[counter].GetComponent<Text>().preferredHeight;

            counter -= 1;
            if (counter < 0) {
                counter = 4;
            }
        }
    }

    public void NewTarget (GameObject target) {
        selectedEnemyObject = target;
        UpdateEnemyUI();
    }

    public void EndGame () {
        // For testing purposes only
        /*
        StreamWriter writer = new StreamWriter(logPath, true);

        foreach (LevelData levelData in levelDataList) {
            writer.WriteLine(levelData.ToString());
        }
        writer.WriteLine("Final Skill: " + (levelDataList[levelDataList.Count - 1].API - levelDataList[levelDataList.Count - 1].EPI));

        writer.Close();
        */

        Application.Quit();
    }
}
