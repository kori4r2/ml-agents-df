using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect {
    private Unit source;
    private Unit target;
    private int duration;
    public int turnsLeft;
    public readonly string text;
    public readonly bool visible;
    // aux functions
    protected abstract void Apply();
    // public functions
    public StatusEffect(Unit src, Unit trgt) {
        source = src;
        target = trgt;
        Apply();
    }
    public abstract bool Countdown();
    public abstract bool Remove();
}
