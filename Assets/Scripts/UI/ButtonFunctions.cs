using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour{
    private Button endGameButton;
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
    public static void SetEndGame(Button button) {
        Debug.Log("Assigning button");
        ButtonFunctions script = GameObject.Find("Main Camera").GetComponent<ButtonFunctions>();
        script.endGameButton = button;
        button.onClick.AddListener((UnityEngine.Events.UnityAction)script.EndGame);
    }
    public void EndGame() {
        Debug.Log("Button was pressed");
        endGameButton.GetComponentInChildren<Text>().text.Remove(17);
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.Exit(0);
#else
        Application.Quit();
#endif
    }
}
