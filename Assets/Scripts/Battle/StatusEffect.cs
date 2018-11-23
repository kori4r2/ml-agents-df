using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect {
    protected Unit source;
    protected Unit target;
    protected readonly int duration;
    private int turnsLeft;
    public int TurnsLeft { get; protected set; }
    public readonly string text;
    public readonly bool visible;
    // aux functions
    protected abstract void Apply();
    // public functions
    public StatusEffect(Unit src, Unit trgt, string txt, int dur, bool see=true) {
        source = src;
        target = trgt;
        text = txt;
        duration = dur;
        visible = see;
        Apply();
    }
    public abstract bool Countdown();
    public abstract void Remove();
}
