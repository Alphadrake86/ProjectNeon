using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleSetupV2 : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleWorldVisuals visuals;
    
    [Header("BattleField")]
    [SerializeField] private float battleFieldScale = 0.929f;

    [Header("Party")]
    [SerializeField] private Party party;
    [SerializeField] private CardPlayZones playerCardPlayZones;
    [SerializeField] private IntReference startingCards;
    
    [Header("Enemies")]
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EncounterBuilder encounterBuilder;
    
    [Header("Technical")]
    [SerializeField] private CardPlayZone resolutionZone;
    
    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;

    public void InitBattleField(GameObject battlefield) => state.SetNextBattleground(battlefield);
    public void InitParty(Hero h1, Hero h2, Hero h3) => party.Initialized(h1, h2, h3);
    
    public IEnumerator Execute()
    {
        ClearResolutionZone();
        SetupBattleField();
        SetupEnemyEncounter();
        yield return visuals.Setup(); // Could Animate
        SetupPlayerCards(); // Could Animate
    }

    private void ClearResolutionZone()
    {
        resolutionZone.Clear();
    }
    
    private void SetupBattleField()
    {
        var battlefield = Instantiate(state.Battlefield, new Vector3(0, 0, 10), Quaternion.identity, transform);
        battlefield.transform.localScale = new Vector3(battleFieldScale, battleFieldScale, battleFieldScale);
    }

    private void SetupPlayerCards()
    {
        if (!party.IsInitialized)
            throw new Exception("Cannot Setup Player Cards, Party Is Not Initialized");

        playerCardPlayZones.ClearAll();
        Deck.InitShuffled(party.Decks.SelectMany(x => x.Cards));
        
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
    }

    private void SetupEnemyEncounter()
    {
        enemyArea = enemyArea.Initialized(encounterBuilder.Generate());
    }
}
