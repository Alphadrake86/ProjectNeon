using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface CardTypeData 
{
    string Name { get; }
    ResourceCost Cost { get; }
    ResourceCost Gain  { get; }
    Sprite Art  { get; }
    string Description  { get; }
    HashSet<CardTag> Tags  { get; }
    string TypeDescription  { get; }
    Maybe<CharacterClass> LimitedToClass  { get; }
    [Obsolete] CardActionSequence[] ActionSequences  { get; }
    CardActionsData[] Actions { get; }
    Rarity Rarity { get; }
}

public static class CardTypeDataExtensions
{
    public static bool Is(this CardTypeData c, params CardTag[] tags) => tags.All(tag => c.Tags.Contains(tag));
    
    public static Card CreateInstance(this CardTypeData c, int id, Member owner) => new Card(id, owner, c);
}
