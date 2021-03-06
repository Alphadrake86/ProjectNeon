using UnityEngine;

public sealed class AdventureHeroUiSummary : MonoBehaviour
{
    [SerializeField] private HeroHpPresenter hpUi;
    [SerializeField] private HealHeroButton healButton;

    public void Init(HeroCharacter hero, int hp)
    {
        hpUi.Init(hero, hp);
        healButton.Init(hero);
    }
}
