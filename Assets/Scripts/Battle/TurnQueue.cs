using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueue {
    private List<Unit> queue;
    private BattleManager battleManager;
    List<Unit> backupList;

    public TurnQueue(BattleManager bm) {
        battleManager = bm;
        queue = new List<Unit>();
        backupList = new List<Unit>();
    }
    
    // aux functions
    private void RollInitiative() {
        queue.AddRange(backupList);
        backupList.Clear();
        foreach (Unit unit in queue) {
            unit.RollInitiative();
        }
        queue.Sort();
    }

    // public functions
    public void FirstTurn(){
        backupList.Clear();
        queue.Clear();
        backupList.AddRange(battleManager.units);
        RollInitiative();
    }
    public void Clear() {
        queue.Clear();
        backupList.Clear();
    }
    public void Enqueue(Unit newUnit) {
        if(!queue.Contains(newUnit) && !backupList.Contains(newUnit))
            queue.Add(newUnit);
    }
    public Unit Dequeue() {
        if (queue.Count < 1) {
            battleManager.turnCounter++;
            RollInitiative();
        }
        // Debug.Log("queue: " + string.Join(",", queue));
        // Debug.Log("backupList: " + string.Join(",", backupList));
        backupList.Add(queue[0]);
        queue.RemoveAt(0);
        return backupList[backupList.Count-1];
    }
    public void Remove(Unit unit) {
        if (!queue.Remove(unit))
            backupList.Remove(unit);
    }
    public void TurnSkip() {
        queue.Insert(0, backupList[backupList.Count - 1]);
        backupList.RemoveAt(backupList.Count - 1);
    }
}
