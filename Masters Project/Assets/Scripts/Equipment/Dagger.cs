using UnityEngine;
using UnityEditor;

public class Dagger : EquipmentBase {
    void Start() {
        objectName = "Dagger";

        equipmentSlot = 0;

        attackBonus = 2;
        defenseBonus = 0;
        healthBonus = 0;

        description = "A " + objectName + "\n" +
           "Slot: Main Hand\n" +
           "Gives " + healthBonus + " bonus health\n" +
           "Gives " + attackBonus + " bonus attack\n" +
           "Gives " + defenseBonus + " bonus defense";
    }
}