using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TARGETS {
    SELF,
    ALLIES,
    ENEMIES,
    ALL
};

public abstract class Skill {
    private int curCD;
    protected Unit thisUnit;
    protected bool available;
    // Name of skills must be the same as the button game object names
    public readonly int cooldown;
    public readonly string name;
    public readonly TARGETS skillType;
    public int CurCD {
        get { return curCD; }
        set {
            curCD = (value <= 0)? 0 : value;
            Available = (curCD <= 0);
        }
    }
    public abstract bool Available { get; protected set; }
    public virtual void Use() {
        CurCD = cooldown;
    }

    public void ResetCooldown() {
        CurCD = 0;
    }
    protected Skill(Unit unit, string str, int cd, TARGETS type) {
        thisUnit = unit;
        name = str;
        cooldown = cd;
        skillType = type;
        ResetCooldown();
    }
}
