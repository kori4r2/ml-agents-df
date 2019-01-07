using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void MyDelegate();

public static class BattleManager {
    public static List<Unit> units = new List<Unit>();
    public static Unit currentUnit;
    public static Unit targetUnit;
    public static Skill selectedAction;
    public static int expectedNUnits = 4;
    public static int turnCounter = -1;
    private static bool waiting;
    private static bool battleStarted = false;
    private static Text logObject;
    public static string BattleLog {
        get { return logObject.text; }
        set {
            string[] split = value.Split(new char[]{'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
            if (split.Length <= 0)
                logObject.text = "";
            else {
                logObject.text = turnCounter +": "+ split[split.Length-1]+"\n";
                for (int i = 0; i < split.Length - 1; i++) {
                    logObject.text += split[i] + "\n";
                }
            }
        }
    }
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
        turnCounter = 0;
        logObject = GameObject.Find("BattleLog").GetComponentInChildren<Text>();
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
        waiting = false;
        currentUnit = queue.Dequeue();
        currentUnit.outline.SetActive(true);
        MyDelegate continuation = currentUnit.CountdownEffects;
        continuation += ContinueTurn;
        continuation();
    }
    private static IEnumerator WaitSeconds(float dur) {
        waiting = true;
        yield return new WaitForSeconds(dur);
        waiting = false;
    }
    public static void Wait3Seconds() {
        waiting = true;
        currentUnit.StartCoroutine(WaitSeconds(1.0f));
    }
    private static IEnumerator WaitingForWait() {
        while (waiting)
            yield return new WaitForSeconds(0.1f);
        currentUnit.GetComponent<UnitAgent>().RequestDecision();
    }
    private static void WaitToGetAction() {
        currentUnit.StartCoroutine(WaitingForWait());
    }
    public static void ContinueTurn() {
        if (currentUnit.CurHP > 0 && !currentUnit.acted) {
            // Update all skill buttons, and skill cooldowns
            MyDelegate orderedFunctions = currentUnit.SetupTurn;
            // If it is not the player, get decision of movement from brain
            if (currentUnit.GetComponent<UnitAgent>().brain.brainType != MLAgents.BrainType.Player) {
                orderedFunctions += Wait3Seconds;
                orderedFunctions += WaitToGetAction;
            }
            orderedFunctions();
        } else {
            // In case of stun, put a notification on the battle log
            if (currentUnit.statusUI.text.Contains("Stunned"))
                BattleLog += currentUnit.name+"->Stunned";
            // In case of death, distribute rewards
            else {
                if (currentUnit.CurHP <= 0) {
                    UnitAgent currentAgent = currentUnit.GetComponent<UnitAgent>();
                    // Give rewards for enemies
                    if(currentAgent.enemy1.CurHP >= 0 || currentAgent.enemy1.GetComponent<UnitAgent>().type == UnitAgent.TYPEAGENT.Cooperative)
                        currentAgent.enemy1.GetComponent<UnitAgent>().AddReward(0.5f);
                    if (currentAgent.enemy2.CurHP >= 0 || currentAgent.enemy2.GetComponent<UnitAgent>().type == UnitAgent.TYPEAGENT.Cooperative)
                        currentAgent.enemy2.GetComponent<UnitAgent>().AddReward(0.5f);
                    // Give punishments for self and ally (if needed)
                    currentAgent.AddReward((currentAgent.type == UnitAgent.TYPEAGENT.Individualistic) ? -1.0f : -0.5f);
                    if (currentAgent.ally.GetComponent<UnitAgent>().type == UnitAgent.TYPEAGENT.Cooperative)
                        currentAgent.ally.GetComponent<UnitAgent>().AddReward(-0.5f);
                }
            }
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
        // Give rewards for enemies killed and negative rewards for deaths
        if (targetUnit.CurHP <= 0) {
            currentUnit.GetComponent<UnitAgent>().AddReward(0.5f);
            currentUnit.GetComponent<UnitAgent>().ally.GetComponent<UnitAgent>().AddReward(0.5f);
            UnitAgent targetAgent = targetUnit.GetComponent<UnitAgent>();
            targetAgent.AddReward((targetAgent.type == UnitAgent.TYPEAGENT.Individualistic)? -1.0f : -0.5f);
            if (targetAgent.ally.GetComponent<UnitAgent>().type == UnitAgent.TYPEAGENT.Cooperative)
                targetAgent.ally.GetComponent<UnitAgent>().AddReward(-0.5f);
        }
        EndTurn();
    }
}
