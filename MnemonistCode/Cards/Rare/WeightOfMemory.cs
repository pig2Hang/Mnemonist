using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Mnemonist.MnemonistCode.Powers;

namespace Mnemonist.MnemonistCode.Cards.Rare;

public class WeightOfMemory() : MnemonistCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private int _currentMemory;
    private int _increasedMemory;
    
    [SavedProperty]
    public int CurrentMemory
    {
        get => this._currentMemory;
        set
        {
            this.AssertMutable();
            this._currentMemory = value;
            this.DynamicVars["Memory"].BaseValue = (Decimal) this._currentMemory;
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new ExtraDamageVar(2M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier( (card, _) => card.CombatState != null ? card.Owner.Creature.GetPowerAmount<Memory>() + card.DynamicVars["Memory"].IntValue : 0),
        new IntVar("Memory", 1M),
        new IntVar("Increase", 2M)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Memory>(), HoverTipFactory.Static(StaticHoverTip.Fatal)];
    
    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    [SavedProperty]
    public int IncreasedMemory
    {
        get => this._increasedMemory;
        set
        {
            this.AssertMutable();
            this._increasedMemory = value;
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        bool shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_big_slash", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        await PowerCmd.Apply<Memory>(choiceContext,this.Owner.Creature, DynamicVars["Memory"].IntValue, this.Owner.Creature, (CardModel) this, false);
        if (!shouldTriggerFatal || !attackCommand.Results.SelectMany( r => r).Any((Func<DamageResult, bool>) (r => r.WasTargetKilled)))
            return;
        int intValue = DynamicVars["Increase"].IntValue;
        BuffFromPlay(intValue);
        if (!(DeckVersion is WeightOfMemory deckVersion))
            return;
        deckVersion.BuffFromPlay(intValue);
    }

    private void BuffFromPlay(int extraMemory)
    {
        this.IncreasedMemory += extraMemory;
        this.UpdateMemory();
    }
    
    private void UpdateMemory() => this.CurrentMemory = 1 + this.IncreasedMemory;
    protected override void OnUpgrade() => this.DynamicVars["Increase"].UpgradeValueBy(1M);
}