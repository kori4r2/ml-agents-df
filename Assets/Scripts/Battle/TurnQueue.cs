using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueue {
    private List<Unit> queue;
    List<Unit> backupList;

    public TurnQueue() {
        queue = new List<Unit>();
        backupList = new List<Unit>();
    }
    
    // aux functions
    private void RollInitiative() {
        queue.AddRange(backupList);
        backupList.Clear();
        queue.Sort();
    }

    // public functions
    public void Clear() {
        queue.Clear();
        backupList.Clear();
    }
    public void Enqueue(Unit newUnit) {
        queue.Add(newUnit);
    }
    public Unit Dequeue() {
        if (queue.Count < 1) {
            BattleManager.turnCounter++;
            RollInitiative();
        }
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
