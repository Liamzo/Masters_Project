using UnityEngine;
using UnityEditor;

public class ShortSword : EquipmentBase {
    void Start() {
        objectName = "Short Sword";

        equipmentSlot = 0;

        attackBonus = 3;
        defenseBonus = 0;
        healthBonus = 0;

        description = "A short sword.\n" +
            "Slot: Main Hand\n" +
            "Gives " + attackBonus + " bonus attack";
    }
}