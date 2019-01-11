using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Skill {
    public Block(Unit unit) :
        base(unit, "Block", 8, TARGETS.SELF) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // Apply defense boost to self
        BattleManager.targetUnit.AddEffect(new DefUp(thisUnit, thisUnit));
        BattleManager.BattleLog += thisUnit.name + "->" + name;
    }
}
