using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Skill {
    public Block(Unit unit) :
        base(unit, "Block", 6, TARGETS.SELF) {
        Available = true;
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        // Apply defense boost to self
        BattleManager.targetUnit.effects.Add(new DefUp(thisUnit, thisUnit));
    }
}
