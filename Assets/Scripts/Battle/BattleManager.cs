using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void MyDelegate();

public static class BattleManager{
    public static List<Unit> units = new List<Unit>();
    public static Unit currentUnit;
    public static Skill selectedAction;
    public static Unit targetUnit;
    public static int expectedNUnits = 4;
    private static bool battleStarted = false;
    private static int nUnits;
    public static int NUnits {
        get {
            return nUnits;
        }
        set {
            nUnits = value;
            if (NUnits == expectedNUnits && !battleStarted)
                StartBattle();
        }
    }
    private static TurnQueue queue = new TurnQueue();
    
    public static void Kill(Unit unit) {
        Debug.Log(unit.name + " was killed");
        unit.FadeOut();
        unit.MakeUnclickable();
        units.Remove(unit);
        queue.Remove(unit);
        NUnits--;
    }
    public static void Add(Unit unit) {
        Debug.Log("Added unit: " + unit.name);
        units.Add(unit);
        queue.Enqueue(unit);
        NUnits++;
    }
    public static void StartBattle() {
        Debug.Log("Battle started");
        Unit unit = units[0];
        // make sure skill buttons are clickable
        foreach (Button button in unit.skillButtons) {
            button.interactable = true;
        }
        battleStarted = true;
        foreach (Unit u in units) {
            queue.Dequeue();
        }
        StartTurn();
    }
    public static void EndBattle(char winningTeam) {
        Debug.Log("Battle ended");
        Unit survivor = units[0];
        foreach(Button button in survivor.skillButtons) {
            button.interactable = false;
        }

        GameObject message = GameObject.Instantiate(Resources.Load("WinMessage") as GameObject, GameObject.Find("Canvas").transform);
        message.GetComponentInChildren<Text>().text += winningTeam;
        message.GetComponent<Button>().onClick.AddListener(ButtonFunctions.EndGame);
        battleStarted = false;
    }
    public static void StartTurn() {
        //Debug.Log("Turn started");
        currentUnit = queue.Dequeue();
        currentUnit.outline.SetActive(true);
        MyDelegate continuation = currentUnit.CountdownEffects;
        continuation += ContinueTurn;
        continuation();
    }
    public static void ContinueTurn() {
        if (currentUnit.CurHP > 0 && !currentUnit.acted) {
            // Update all skill buttons, and skill cooldowns
            currentUnit.SetupTurn();
        } else {
            Debug.Log(currentUnit.name + " has " + ((currentUnit.acted) ? "" : "not ") + "acted, " + currentUnit.CurHP + " HP left");
            EndTurn();
        }
    }
    public static void EndTurn() {
        //Debug.Log("Turn ended");
        currentUnit.acted = false;
        currentUnit.outline.SetActive(false);
        int team1Count = 0;
        int team2Count = 0;
        foreach (Unit unit in units) {
            if (unit.team == '1')
                team1Count++;
            else
                team2Count++;
            unit.MakeUnclickable();
            unit.FadeIn();
        }
        Debug.Log("team1count = " + team1Count + "; team2count = " + team2Count);
        if (team1Count == 0 || team2Count == 0)
            EndBattle((team1Count == 0) ? '2' : '1');
        else
            StartTurn();
    }
    public static void SkillSelected() {
        //Debug.Log("Skill selected");
        foreach(Unit unit in units) {
            switch (selectedAction.skillType) {
                case TARGETS.ALL:
                    unit.FadeIn();
                    unit.MakeClickable();
                    break;
                case TARGETS.ALLIES:
                    if(unit.team == currentUnit.team) {
                        unit.FadeIn();
                        unit.MakeClickable();
                    } else {
                        unit.FadeOut();
                        unit.MakeUnclickable();
                    }
                    break;
                case TARGETS.ENEMIES:
                    if (unit.team != currentUnit.team) {
                        unit.FadeIn();
                        unit.MakeClickable();
                    } else {
                        unit.FadeOut();
                        unit.MakeUnclickable();
                    }
                    break;
                case TARGETS.SELF:
                    if (unit == currentUnit) {
                        unit.FadeIn();
                        unit.MakeClickable();
                    } else {
                        unit.FadeOut();
                        unit.MakeUnclickable();
                    }
                    break;
            }
        }
    }
    public static void TargetSelected() {
        Debug.Log("Target selected, "+currentUnit.name+" used "+selectedAction.name+" on "+targetUnit.name);
        selectedAction.Use();
        EndTurn();
    }
}
