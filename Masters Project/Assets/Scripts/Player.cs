using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : FighterBase {

    public GameObject inventoryUI;
    public GameObject inventoryTextList;

    GameObject selectedItem;
    public GameObject selectedItemUI;
    public GameObject selectedItemName;
    public GameObject selectedItemDescription;

    public GameObject levelUpUI;
    public GameObject levelUpOption;
    bool levelingUp = false;

    public int sightRange;

    bool inventoryOpen = false;
    bool itemSelected = false;
    List<GameObject> inventory = new List<GameObject>();
    GameObject[] equipment = new GameObject[4];

    public int level = 1;
    public int xpCur = 0;
    public int xpToNextLevel = 200;
    public int xpIncreaseFactor = 100;

	// Use this for initialization
	void Awake () {

        objectName = "You";
        description = "You";

        healthMax = 50;
        healthCur = HealthMax;
        attack = 2;
        defense = 0;

        level = 1;
        xpCur = 0;
        xpToNextLevel = 200;
        xpIncreaseFactor = 50;
        
        sightRange = 7;

        inventoryOpen = false;
        itemSelected = false;
    }

    public override int HealthMax {
        get {
            int bonus = 0;

            foreach (GameObject item in equipment) {
                if (item != null) {
                    EquipmentBase equip = item.GetComponent<EquipmentBase>();
                    bonus += equip.healthBonus;
                }
            }

            return (healthMax + bonus);
        }

        set {
            healthMax = value;
        }
    }
    public override int Attack {
        get {
            int bonus = 0;

            foreach (GameObject item in equipment) {
                if (item != null) {
                    EquipmentBase equip = item.GetComponent<EquipmentBase>();
                    bonus += equip.attackBonus;
                }
            }

            return (attack + bonus);
        }

        set {
            attack = value;
        }
    }
    public override int Defense {
        get {
            int bonus = 0;

            foreach (GameObject item in equipment) {
                if (item != null) {
                    EquipmentBase equip = item.GetComponent<EquipmentBase>();
                    bonus += equip.defenseBonus;
                }
            }

            return (defense + bonus);
        }

        set {
            defense = value;
        }
    }
    
    public override void AttackTarget(FighterBase fighter) {
        int dam = Attack - fighter.Defense;

        if (dam > 0) {
            GameManager.gameManager.NewMessage(objectName + " deal " + dam + " damage to the " + fighter.objectName);
            fighter.TakeDamage(dam);
        } else {
            GameManager.gameManager.NewMessage(objectName + " attack the " + fighter.objectName + " but it has no effect");
        }
    }

    // Update is called once per frame
    void Update () {
        // Wait until player's turn
		if (GameManager.gameManager.playersTurn != true) {
            return;
        }

        if (levelingUp == true) {
            if (Input.GetKeyDown(KeyCode.A)) {
                healthMax += 20;
                healthCur += 20;
                levelUpUI.SetActive(false);
                levelingUp = false;
                GameManager.gameManager.UpdatePlayerUI();
            } else if (Input.GetKeyDown(KeyCode.B)) {
                attack += 1;
                levelUpUI.SetActive(false);
                levelingUp = false;
                GameManager.gameManager.UpdatePlayerUI();
            } else if (Input.GetKeyDown(KeyCode.C)) {
                Heal(HealthMax/2);
                levelUpUI.SetActive(false);
                levelingUp = false;
                GameManager.gameManager.UpdatePlayerUI();
            }

        } else if (itemSelected == true) {
            if (Input.GetKeyDown(KeyCode.U)) {
                selectedItem.GetComponent<ItemBase>().Use();

                HideSelectedItem();
                CloseInventory();
                GameManager.gameManager.playersTurn = false; // End player's turn
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                // Drop item
                BaseObject item = selectedItem.GetComponent<BaseObject>();
                item.UpdatePosition(x, y);

                if (inventory.Contains(selectedItem)) {
                    inventory.Remove(selectedItem);
                } else {
                    for (int i = 0; i < 4; i++) {
                        if (selectedItem == equipment[i]) {
                            selectedItem.GetComponent<EquipmentBase>().isEqipped = false;
                            equipment[i] = null;
                        }
                    }
                }

                GameManager.mapManager.DropItem(selectedItem);
                GameManager.gameManager.NewMessage("You drop a " + item.objectName);

                HideSelectedItem();
                CloseInventory();
                GameManager.gameManager.playersTurn = false; // End player's turn
            }

            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Escape)) {
                HideSelectedItem();
            }
        } else if (inventoryOpen == true) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                CloseInventory();
            }

            if (Input.anyKeyDown) {
                string s = "a";
                char letter = s[0];

                string itemString = Input.inputString;
                if (itemString.Length < 1) {
                    return;
                }
                char itemChar = itemString[0]; // Throws error if non-standard key is pressed, such as Esc or F# key etc.

                int itemNum = (int)itemChar - (int)letter;

                if (itemNum >= 0 && itemNum <= 3) {
                    if (equipment[itemNum] != null) {
                        ShowSelectedItem(equipment[itemNum]);
                    }
                }

                if (itemNum >= 4 && itemNum <= 26) {
                    itemNum -= 4;
                    if (itemNum < inventory.Count) {
                        ShowSelectedItem(inventory[itemNum]);
                    }
                }
            }
        } else {
            int dx = 0;
            int dy = 0;

            if (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.UpArrow)) {
                dy = 1; // Up
            } else if (Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.DownArrow)) {
                dy = -1; // Down
            } else if (Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.LeftArrow)) {
                dx = -1; // Left
            } else if (Input.GetKeyUp(KeyCode.Keypad6) || Input.GetKeyUp(KeyCode.RightArrow)) {
                dx = 1; // Right
            } else if (Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Home)) {
                dx = -1; // Up Left
                dy = 1;
            } else if (Input.GetKeyUp(KeyCode.Keypad9) || Input.GetKeyUp(KeyCode.PageUp)) {
                dx = 1; // Up Right
                dy = 1;
            } else if (Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.End)) {
                dx = -1; // Down Left
                dy = -1;
            } else if (Input.GetKeyUp(KeyCode.Keypad3) || Input.GetKeyUp(KeyCode.PageDown)) {
                dx = 1; // Down Right
                dy = -1;
            } else if (Input.GetKeyUp(KeyCode.Keypad5) || Input.GetKeyUp(KeyCode.Space)) {
                // Wait a turn
                GameManager.gameManager.playersTurn = false; // End player's turn
                return;
            }
            
            if (dx != 0 || dy != 0) {
                if (GameManager.mapManager.PlayerCheckAttack(x + dx, y + dy)) {
                    GameManager.gameManager.playersTurn = false; // End player's turn
                    return;
                } else if (GameManager.mapManager.CheckMove(x + dx, y + dy)) {
                    transform.position += new Vector3(dx, dy, 0);

                    x += dx;
                    y += dy;

                    GameManager.gameManager.playersTurn = false; // End player's turn
                    return;
                }
            }

            if (Input.GetKeyUp(KeyCode.G)) {
                GameObject item = GameManager.mapManager.PickUpItem(x, y);

                if (item != null) {
                    inventory.Add(item);
                    GameManager.gameManager.NewMessage("You pick up a " + item.GetComponent<BaseObject>().objectName);

                    GameManager.gameManager.playersTurn = false; // End player's turn
                    return;
                }
            }

            if (Input.GetKeyUp(KeyCode.I)) {
                OpenInventory();
            }

            if (Input.GetKeyUp(KeyCode.Return)) {
                GameManager.mapManager.CheckStairs(x, y);
            }

            if (Input.GetKeyUp(KeyCode.Y)) {
                GameManager.gameManager.NewMessage("This is a quick test.\n Level: " + level);
                level++;
            }
        }

    

    }

    void OpenInventory () {
        inventoryOpen = true;
        inventoryUI.SetActive(true);

        string s = "e";
        char letter = s[0];

        Text inventoryText = inventoryTextList.GetComponent<Text>();
        inventoryText.text = "";

        // Display equipment slots
        string mainHand = "(a)Main Hand: ";
        string offHand = "(b)Off Hand: ";
        string head = "(c)Head: ";
        string body = "(d)Body: ";

        EquipmentBase eb;

        if (equipment[0] != null) {
            eb = equipment[0].GetComponent<EquipmentBase>();
            mainHand += eb.objectName;
        }
        if (equipment[1] != null) {
            eb = equipment[1].GetComponent<EquipmentBase>();
            offHand += eb.objectName;
        }
        if (equipment[2] != null) {
            eb = equipment[2].GetComponent<EquipmentBase>();
            head += eb.objectName;
        }
        if (equipment[3] != null) {
            eb = equipment[3].GetComponent<EquipmentBase>();
            body += eb.objectName;
        }

        inventoryText.text += mainHand + "\n";
        inventoryText.text += offHand + "\n";
        inventoryText.text += head + "\n";
        inventoryText.text += body + "\n";

        for (int i = 0; i < inventory.Count; i++) {
            inventoryText.text += "(" + letter + ")" + inventory[i].GetComponent<BaseObject>().objectName + "\n";

            letter++;
        }
    }
    void CloseInventory () {
        inventoryOpen = false;
        inventoryUI.SetActive(false);
    }

    void ShowSelectedItem (GameObject item) {
        itemSelected = true;
        selectedItemUI.SetActive(true);

        selectedItem = item;

        ItemBase ib = item.GetComponent<ItemBase>();

        Text itemName = selectedItemName.GetComponent<Text>();
        itemName.text = ib.objectName.ToUpper();

        Text itemDescription = selectedItemDescription.GetComponent<Text>();
        itemDescription.text = ib.description;

    }
    void HideSelectedItem () {
        itemSelected = false;
        selectedItem = null;
        selectedItemUI.SetActive(false);
    }

    public void EquipItem (GameObject equip) {
        // First check if there is an item already in the slot
        // If so, remove it then add the new item
        int slot = equip.GetComponent<EquipmentBase>().equipmentSlot;

        if (equipment[slot] != null) {
            DequipItem(equipment[slot]);
        }

        equipment[slot] = equip;

        if (healthCur > HealthMax) {
            healthCur = HealthMax;
        }

        inventory.Remove(equip); // Remove from inventory list as item is now equipped

        GameManager.gameManager.NewMessage("You equip the " + equip.GetComponent<BaseObject>().objectName);
    }
    public void DequipItem (GameObject equip) {
        // Remove item from list of equipped items, then add back to inventory
        int slot = equip.GetComponent<EquipmentBase>().equipmentSlot;

        equipment[slot] = null;
        inventory.Add(equip);
        GameManager.gameManager.NewMessage("You dequip the " + selectedItem.GetComponent<BaseObject>().objectName);
    }

    public void RemoveItem (GameObject item) {
        inventory.Remove(item);
    }

    

    public void GainXP (int xp) {
        xpCur += xp;

        if (xpCur >= xpToNextLevel) {
            LevelUp();
        }
    }

    void LevelUp () {
        xpCur -= xpToNextLevel;
        xpToNextLevel += xpIncreaseFactor;
        level++;

        GameManager.gameManager.NewMessage("Your skills grow stronger! You reached level " + level);

        // Do level up stuff
        levelUpUI.SetActive(true);
        levelingUp = true;

        Text levelUpText = levelUpOption.GetComponent<Text>();
        levelUpText.text = "Choose a boon:\n" +
            "(a)Constitution (+20 Max HP)\n" +
            "(b)Strength (+1 Attack)\n" +
            "(c)Health (+ Heal 1/2 HP)";
    }

    public int TotalPowerLevel () {
        return SelfPowerLevel() + EquipmentPowerLevel() + InventoryPowerLevel();
    }

    public int SelfPowerLevel () {
        int totalPowerLevel = 0;

        totalPowerLevel += level * 200; // Each level is worth 100 points

        totalPowerLevel += (int)(((float)xpCur / xpToNextLevel) * 200); // Add in current xp

        totalPowerLevel += (int)(((float)healthCur / HealthMax) * 200); // Add in health

        return totalPowerLevel;
    }

    public int EquipmentPowerLevel () {
        int powerlevel = 0;

        // Add up the total power of current equipment
        foreach (GameObject item in equipment) {
            if (item != null) {
                EquipmentBase equip = item.GetComponent<EquipmentBase>();
                powerlevel += equip.powerLevel;
            }
        }

        // Check if any unequiped items are better than currently equipped
        // Add the power of each item in inventory
        foreach (GameObject item in inventory) {
            EquipmentBase equip = item.GetComponent<EquipmentBase>();

            if (equip != null) {
                if (equipment[equip.equipmentSlot] != null) {
                    if (equip.powerLevel > equipment[equip.equipmentSlot].GetComponent<EquipmentBase>().powerLevel) {
                        powerLevel += (equip.powerLevel - equipment[equip.equipmentSlot].GetComponent<EquipmentBase>().powerLevel);
                    }
                } else {
                    powerLevel += equip.powerLevel;
                }
            }
        }

        return powerlevel;
    }

    public int InventoryPowerLevel () {
        int powerlevel = 0;

        foreach (GameObject item in inventory) {
            EquipmentBase equip = item.GetComponent<EquipmentBase>();

            // Only count non-equipment
            if (equip == null) {
                BaseObject bo = item.GetComponent<BaseObject>();
                powerlevel += bo.powerLevel;
            }
        }

        return powerlevel;
    }

    public Vector2Int FindNeededEquipment () {
        Vector2Int needSlot = new Vector2Int();
        int lowestPower = 1000;

        // Add up the total power of current equipment
        for (int i = 0; i < equipment.Length; i++) {
            GameObject item = equipment[i];
            if (item != null) {
                EquipmentBase equip = item.GetComponent<EquipmentBase>();
                 if (equip.powerLevel < lowestPower) {
                    lowestPower = equip.powerLevel;
                    needSlot.x = equip.equipmentSlot;
                    needSlot.y = lowestPower;
                }
            } else {
                needSlot.x = i;
                needSlot.y = 0;
                return needSlot;
            }
        }

        return needSlot;
    }

    public int PowerInSlot (int slot) {
        GameObject item = equipment[slot];

        if (item == null) {
            return 0;
        } else {
            return item.GetComponent<BaseObject>().powerLevel;
        }
    }

    public int NumberOfHealthPotions () {
        int count = 0;
        foreach (GameObject item in inventory) {
            BaseObject bo = item.GetComponent<BaseObject>();

            if (bo.objectName.Equals("Healing Potion")) {
                count++;
            }
        }

        return count;
    }

    protected override void Death () {
        GameManager.gameManager.EndGame();
    }
}
