﻿using System.Linq;
using UnityEngine;

public class SelectCardTargetsV2 : MonoBehaviour, IConfirmCancellable
{
    [SerializeField] private CardResolutionZone cardResolutionZone;
    [SerializeField] private CardPlayZone selectedCardZone;
    [SerializeField] private CardPlayZone destinationCardZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private GameObject uiView;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingState targetingState;

    [ReadOnly, SerializeField] private Card _card;
    private int _actionIndex;
    private int _numActions;
    private Target[] _actionTargets;
    
    protected void OnEnable() => selectedCardZone.OnZoneCardsChanged.Subscribe(BeginSelection, this);
    protected void OnDisable() => selectedCardZone.OnZoneCardsChanged.Unsubscribe(this);

    private void BeginSelection()
    {
        if (selectedCardZone.Count < 1)
            return;

        battleState.SelectionStarted = true;
        _card = selectedCardZone.Cards[0];
        Message.Publish(new TargetSelectionBegun(_card.Type));

        cardPresenter.Set(_card, () => { });
        cardPresenter.SetHighlightGraphicState(true);
        Log.Info($"Showing Selected Card {_card.Name}", gameObject);
        uiView.SetActive(true);

        _actionIndex = 0;
        _numActions = _card.ActionSequences.Length;
        _actionTargets = new Target[_numActions];
        if (_numActions == 0)
        {
            Log.Info($"Card {_card.Name} has no Card Actions");
            OnTargetConfirmed(Group.All, Scope.All);
            return;
        }

        PresentPossibleTargets();
    }

    private void PresentPossibleTargets()
    {
        var action = _card.ActionSequences[_actionIndex];
        var possibleTargets = battleState.GetPossibleConsciousTargets(_card.Owner, action.Group, action.Scope == Scope.AllExcept ? Scope.One : action.Scope);
        targetingState.WithPossibleTargets(possibleTargets);
        if (possibleTargets.Length == 1)
            OnTargetConfirmed(action.Group, action.Scope);
    }

    public void Cancel() => OnCancelled();
    public void OnCancelled()
    {
        Message.Publish(new PlayerCardCanceled());
        OnSelectionComplete(sourceCardZone);
    }

    public void Confirm() => OnTargetConfirmed(_card.ActionSequences[_actionIndex].Group, _card.ActionSequences[_actionIndex].Scope);
    public void OnTargetConfirmed(Group group, Scope scope)
    {
        if (scope != Scope.AllExcept)
            _actionTargets[_actionIndex] = targetingState.Current;
        else
            _actionTargets[_actionIndex] = battleState.GetPossibleConsciousTargets(_card.Owner, group, scope)
                .First(target => target.Members.All(member => member != targetingState.Current.Members[0]));
        targetingState.Clear();

        if (_actionIndex + 1 == _numActions)
        {
            var playedCard = new PlayedCardV2(_card.Owner, _actionTargets, _card);
            cardResolutionZone.Add(playedCard);
            Message.Publish(new PlayerCardSelected());;
            OnSelectionComplete(destinationCardZone);
        }
        else
        {
            _actionIndex++;
            PresentPossibleTargets();
        }
    }

    private void OnSelectionComplete(CardPlayZone sendToZone)
    {
        sendToZone.PutOnBottom(selectedCardZone.DrawOneCard());
        _card = null;
        uiView.SetActive(false);
        battleState.SelectionStarted = false;
        Message.Publish(new TargetSelectionFinished());
    }
}
