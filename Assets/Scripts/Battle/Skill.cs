using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TARGETS {
    ALLIES,
    ENEMIES,
    ALL
};

public abstract class Skill {
    private readonly int cooldown;
    private int curCD;
    // Name of skills must be the same as the button game object names
    public readonly string name;
    public readonly TARGETS skillType;
    public int CurCD {
        get {
            return curCD;
        }
        set {
            curCD = (value <= 0)? 0 : value;
            if (curCD <= 0)
                Available = true;
        }
    }
    public bool Available { get; private set; }
    public abstract void Use();

    public void ResetCooldown() {
        CurCD = cooldown;
    }

    public Skill() {
        ResetCooldown();
    }
}
