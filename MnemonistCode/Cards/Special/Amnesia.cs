using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Mnemonist.MnemonistCode.Extensions;
using Mnemonist.MnemonistCode.Relics;

namespace Mnemonist.MnemonistCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class Amnesia() : CustomCardModel(0,
    CardType.Skill, CardRarity.Curse,
    TargetType.Self)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.ToLowerInvariant()}.png".CardImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Anamnesis>()];
    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
    ];

    private bool _didPlay;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cranialTrauma = Owner.GetRelic<CranialTrauma>();
        if (cranialTrauma is null || CombatState is null)
            return;
        await cranialTrauma.UseExhaust(choiceContext);
        _didPlay = true;
    }
    public override async Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? source)
    {
        if (card != this || CombatState is null || !_didPlay)
        {
            return;
        }
        var anamnesis = CombatState.CreateCard<Anamnesis>(Owner);
        await CardCmd.Transform(this, anamnesis);
    }

    public override int MaxUpgradeLevel => 0;
}