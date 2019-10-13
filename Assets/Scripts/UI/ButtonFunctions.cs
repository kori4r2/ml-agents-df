using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BattleManager))]
public class ButtonFunctions : MonoBehaviour{
    private BattleManager battleManager;
    public void Awake(){
        battleManager = GetComponent<BattleManager>();
    }
    public void SkillPressed() {
        // The name of each button gameobject must be the same as the name of the skill
        // The ideal implementation would use IDs but let's keep it simple
        battleManager.selectedAction = battleManager.currentUnit.skillList.Find(
            skill => skill.name == EventSystem.current.currentSelectedGameObject.name);
        battleManager.SkillSelected();
    }
    public void TargetSelected() {
        battleManager.targetUnit = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Unit>();
        battleManager.TargetSelected();
    }
    public static void EndGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
