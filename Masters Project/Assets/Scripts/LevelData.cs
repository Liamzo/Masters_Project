using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LevelData {
    public int floor;

    public int selfPower;
    public int equipmentPower;
    public int itemPower;
    public int totalPower;

    public int skillLevel;

    public int levelDifficulty;
    public int levelItems;
    
    public int noRooms;

    public int EPI;
    public int API;

    public override string ToString() {
        StringBuilder sb = new StringBuilder("Floor: ");
        sb.Append(floor);
        sb.Append(" TotalPower: " + totalPower + " SelfPower: " + selfPower + " EquipmentPower: " + equipmentPower + " ItemPower: " + itemPower);
        sb.Append(" Skill: " + skillLevel);
        sb.Append(" Enemy Power: " + levelDifficulty);
        sb.Append(" Level Items: " + levelItems);
        sb.Append(" Length: " + noRooms);
        sb.Append(" EPI: " + EPI + " API: " + API);

        return sb.ToString();
    }
}
