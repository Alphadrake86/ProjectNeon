﻿using NUnit.Framework;
using UnityEngine;

public class OnShieldBrokenTests
{
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(2, false)]
    public void OnShieldBroken_Attacked_TriggersCorrectly(int startingShields, bool triggered)
    {
        var target = TestMembers.Create(x => x.With(StatType.MaxHP, 10).With(StatType.Toughness, 1));
        var attacker = TestMembers.Create(s => s.With(StatType.Attack, 1));
        target.State.GainShield(startingShields);

        var reactionCardType = TestCards.Reaction(
            ReactiveMember.Possessor,
            ReactiveTargetScope.Self,
            new EffectData { EffectType = EffectType.AdjustStatAdditively, FloatAmount = new FloatReference(1), EffectScope = new StringReference("Armor"), NumberOfTurns = new IntReference(-1) });

        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.OnShieldBroken,
            NumberOfTurns = new IntReference(3),
            ReactionSequence = reactionCardType
        }, target, target);

        ReactiveTestUtilities.ApplyEffectAndReactions(new EffectData
        {
            EffectType = EffectType.Attack,
            FloatAmount = new FloatReference(1),
            EffectScope = new StringReference(ReactiveTargetScope.Self.ToString())
        }, attacker, target);

        Assert.AreEqual(triggered ? 1 : 0, target.State.Armor());
    }
}