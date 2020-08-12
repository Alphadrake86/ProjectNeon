﻿using System;
using System.Linq;

public interface Target  
{
    Member[] Members { get; }
}

public static class TargetExtensions
{
    public static void ApplyToAll(this Target t, Action<MemberState> effect) 
        => t.Members.ForEach(m => m.Apply(effect));

    public static string MembersDescriptions(this Target t) 
        => string.Join(", ", t.Members.Select(m => $"{m.Name} {m.Id}"));

    public static int TotalHpAndShields(this Target t) 
        => t.Members.Sum(m => m.CurrentShield() + m.CurrentHp());

    public static int TotalMissingHp(this Target t)
        => t.Members.Sum(m => m.MaxHp() - m.CurrentHp());

    public static int TotalRemainingShieldCapacity(this Target t)
        => t.Members.Sum(m => m.RemainingShieldCapacity());

    public static bool HasShield(this Target t)
        => t.Members.Sum(m => m.CurrentShield()) > 0;
}
