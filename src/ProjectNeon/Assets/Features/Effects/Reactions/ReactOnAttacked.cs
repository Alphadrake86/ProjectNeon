using System.Collections.Generic;
using System.Linq;

public sealed class EffectOnAttacked : Effect
{
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly ReactionCardType _reaction;
    private readonly IReadOnlyDictionary<int, Member> _allMembers;

    public EffectOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, ReactionCardType reaction, IReadOnlyDictionary<int, Member> allMembers)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _reaction = reaction;
        _allMembers = allMembers;
    }
    
    public void Apply(Member source, Target target)
    {
        target.ApplyToAllConscious(m => 
            m.AddReactiveState(new ReactOnAttacked(_isDebuff, _numberOfUses, _maxDurationTurns, _allMembers, m.MemberId, source, _reaction)));
    }
}

public sealed class ReactOnAttacked : ReactiveEffectV2Base
{
    public ReactOnAttacked(bool isDebuff, int numberOfUses, int maxDurationTurns, IReadOnlyDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction)
        : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, reaction, effect => effect.EffectData.EffectType == EffectType.Attack)) {}

    public override StatusTag Tag => StatusTag.CounterAttack;
}
