﻿using System;
using System.Linq;
using UnityEngine;

public class LibraryUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardInLibraryButton cardInLibraryButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private PartyCardCollection partyCards;
    [SerializeField] private DeckBuilderState state;

    protected override void Execute(DeckBuilderHeroSelected msg) => GenerateLibrary();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => GenerateLibrary();
    
    private void GenerateLibrary()
    {
        var cardsForHero = partyCards.AllCards
            .Where(x => !x.Key.LimitedToClass.IsPresent || x.Key.LimitedToClass.Value.Name == state.SelectedHeroesDeck.Hero.Class.Name);
        var cardUsage = cardsForHero.ToDictionary(c => c.Key,
            c => new Tuple<int, int>(c.Value, c.Value - state.SelectedHeroesDeck.Deck.Count(card => card == c.Key)));
        pageViewer.Init(
            cardInLibraryButtonTemplate.gameObject, 
            emptyCard, 
            cardUsage
                .Select(x => InitCardInLibraryButton(x.Key, x.Value.Item1, x.Value.Item2))
                .ToList(), 
            x => {});
    }

    private Action<GameObject> InitCardInLibraryButton(CardType card, int numTotal, int numAvailable)
    {
        void Init(GameObject gameObj) => gameObj.GetComponent<CardInLibraryButton>().Init(card, numTotal, numAvailable);
        return Init;
    }
}
