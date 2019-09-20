﻿using System.Linq;
using UnityEngine;

public class CardsVisualizer : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private CardPlayZone onCardClickDestination;
    [SerializeField] private float minX = 200;
    [SerializeField] private float maxX = 1920 - 200;
    [SerializeField] private float cardSpacingScreenPercent = 0.15f;
    [SerializeField] private CardPresenter cardPrototype;

    [ReadOnly] [SerializeField] private GameObject[] _shownCards = new GameObject[0];
    private bool _isDirty = false;

    void Awake()
    {
        zone.OnZoneCardsChanged.Subscribe(
            new GameEventSubscription(zone.OnZoneCardsChanged.name, x => _isDirty = true, this));
    }

    void OnDisable()
    {
        zone.OnZoneCardsChanged.Unsubscribe(this);
    }

    void Update()
    {
        if (!_isDirty)
            return;

        _isDirty = false;
        UpdateVisibleCards();
    }
    
    void UpdateVisibleCards()
    {
        KillPreviousCards();
        CreateCurrentCards(zone.Cards.ToArray());
    }
    
    // @todo #30:30min Animate these cards entrances. Should slide in from right of screen

    // @todo #30:15min Space card out from the center, instead of from Left of Zone, and add a little tilt, based on card index.

    private void CreateCurrentCards(Card[] cards)
    {
        var newShownCards = new GameObject[cards.Length];
        for (var i = 0; i < cards.Length; i++)
        {
            var cardIndex = i;
            var c = Instantiate(cardPrototype, 
                new Vector3(minX + cardSpacingScreenPercent * Screen.width * i, transform.position.y, transform.position.z), 
                cardPrototype.transform.rotation, 
                gameObject.transform);
            c.Set(cards[cardIndex], () => SelectCard(cardIndex));
            newShownCards[cardIndex] = c.gameObject;
        }
        _shownCards = newShownCards;
    }

    private void KillPreviousCards()
    {
        var shown = _shownCards.ToArray();
        shown.ForEach(x => DestroyImmediate(x));
    }

    void SelectCard(int cardIndex)
    {
        onCardClickDestination.PutOnBottom(zone.Take(cardIndex));
    }
}