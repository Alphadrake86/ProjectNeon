﻿using System;
using System.IO;
using UnityEngine;

public class Member 
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public MemberState State { get; }

    public override string ToString() => $"{Name} {Id}";
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats)
        : this(id, name, characterClass, team, baseStats, baseStats.MaxHP()) {}
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats, int initialHp)
    {
        if (id > -1 && baseStats.Damagability() < 0.01)
            throw new InvalidDataException($"Damagability of {name} is 0");
        
        Id = id;
        Name = name;
        Class = characterClass;
        TeamType = team;
        State = new MemberState(id, baseStats, initialHp);
    }

    public void Apply(Action<MemberState> effect)
    {
        effect(State);
    }
}

public static class MemberExtensions
{
    private static int RoundUp(float v) => Mathf.CeilToInt(v);
    
    public static int CurrentHp(this Member m) => RoundUp(m.State[TemporalStatType.HP]);
    public static int MaxHp(this Member m) => RoundUp(m.State.MaxHP());
    public static int RemainingShieldCapacity(this Member m) => m.MaxShield() - m.CurrentHp(); 
    public static int CurrentShield(this Member m) => RoundUp(m.State[TemporalStatType.Shield]);
    public static int MaxShield(this Member m) => RoundUp(m.State[StatType.Toughness] * 2); 
    public static bool IsConscious(this Member m) => m.State.IsConscious;
    public static bool IsStunnedForCurrentTurn(this Member m) => m.State[TemporalStatType.TurnStun] > 0;
    public static bool IsStunnedForCard(this Member m) => m.State[TemporalStatType.CardStun] > 0;
    public static int ResourceMax(this Member m, IResourceType resourceType) => RoundUp(m.State.Max(resourceType.Name));

    public static bool CanAfford(this Member m, CardType c)
    {
        if (!c.Cost.IsXCost && c.Cost.Amount == 0)
            return true;
        var cost = c.ResourcesSpent(m);
        var remaining = m.State[cost.ResourceType] - cost.Amount;
        return remaining >= 0;
    }
}
