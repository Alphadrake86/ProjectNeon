﻿using System.Collections.Generic;

public sealed class DumbAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        
        var card = playableCards.Random();
        var targets = new List<Target>();
        
        card.ActionSequences.ForEach(
            action =>
            {
                var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
                var target = possibleTargets.Random();
                targets.Add(target);
            }
        );

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
