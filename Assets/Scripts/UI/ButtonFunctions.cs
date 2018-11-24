using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFunctions : MonoBehaviour{
    public void SkillPressed() {
        // The name of each button gameobject must be the same as the name of the skill
        // The ideal implementation would use IDs but let's keep it simple
        BattleManager.selectedAction = BattleManager.currentUnit.skillList.Find(
            skill => skill.name == EventSystem.current.currentSelectedGameObject.name);
        BattleManager.SkillSelected();
    }
    public void TargetSelected() {
        BattleManager.targetUnit = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Unit>();
        BattleManager.TargetSelected();
    }
}
