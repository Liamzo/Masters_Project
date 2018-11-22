using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : BaseObject {

    public virtual void Use () {
        Debug.Log(objectName);
    }
}
