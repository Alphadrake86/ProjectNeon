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

    [ReadOnly, SerializeField] private Card _selectedCard;
    private Member _hero;
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
        _selectedCard = selectedCardZone.Cards[0];
        Message.Publish(new TargetSelectionBegun(_selectedCard));

        var cardClass = _selectedCard.LimitedToClass;
        if (!cardClass.IsPresent)
        {
            Debug.Log($"Card {_selectedCard.Name} is not playable by Heroes", _selectedCard);
            return;
        }

        cardPresenter.Set(_selectedCard, () => { });
        Debug.Log($"Showing Selected Card {_selectedCard.name}", gameObject);
        uiView.SetActive(true);

        _hero = battleState.Members.Values.FirstOrDefault(x => x.Class.Equals(cardClass.Value));
        if (_hero == null)
        {
            Debug.Log($"Could not find Party Member with Class {cardClass.Value}");
            return;
        }

        _actionIndex = 0;
        _numActions = _selectedCard.ActionSequences.Length;
        _actionTargets = new Target[_numActions];
        if (_numActions == 0)
        {
            Debug.Log($"Card {_selectedCard.Name} has no Card Actions");
            OnTargetConfirmed();
            return;
        }

        PresentPossibleTargets();
    }

    private void PresentPossibleTargets()
    {
        var action = _selectedCard.ActionSequences[_actionIndex];
        var possibleTargets = battleState.GetPossibleTargets(_hero, action.Group, action.Scope);
        targetingState.WithPossibleTargets(possibleTargets);
        if (possibleTargets.Length == 1)
            OnTargetConfirmed();
    }

    public void Cancel() => OnCancelled();
    public void OnCancelled()
    {
        OnSelectionComplete(sourceCardZone);
    }

    public void Confirm() => OnTargetConfirmed();
    public void OnTargetConfirmed()
    {
        _actionTargets[_actionIndex] = targetingState.Current;
        targetingState.Clear();

        if (_actionIndex + 1 == _numActions)
        {
            var resourcesSpent = new ResourcesSpent { ResourceType = _selectedCard.Cost.ResourceType };
            resourcesSpent.Amount = _selectedCard.Cost.IsXCost ? battleState.Heroes.First(x => x.Id == _hero.Id).State[_selectedCard.Cost.ResourceType] : _selectedCard.Cost.Cost;
            cardResolutionZone.Add(new PlayedCardV2(_hero, _actionTargets, _selectedCard, resourcesSpent));
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
        _selectedCard = null;
        uiView.SetActive(false);
        battleState.SelectionStarted = false;
        Message.Publish(new TargetSelectionFinished());
    }
}