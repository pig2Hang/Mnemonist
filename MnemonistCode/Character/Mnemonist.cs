using BaseLib.Abstracts;
using Mnemonist.MnemonistCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Mnemonist.MnemonistCode.Cards.Basic;
using Mnemonist.MnemonistCode.Relics;

namespace Mnemonist.MnemonistCode.Character;

public class Mnemonist : PlaceholderCharacterModel
{
    public const string CharacterId = "Mnemonist";
    public override string PlaceholderID => "defect";

    public static readonly Color Color = new("ffffff");
    
    // public override CustomEnergyCounter? CustomEnergyCounter =>
    //     new CustomEnergyCounter((i) => "res://Mnemonist/images/charui/mnemonist_energy_icon.png", new Color(0.4f, 0.1f, 0.9f), new Color(0.7f, 0.1f, 0.9f));
    
    public override string CustomVisualPath => "res://Mnemonist/scenes/combat/creature_visuals/mnemonist.tscn";
    public override string CustomMerchantAnimPath => "res://Mnemonist/scenes/merchant/mnemonist.tscn";
    public override string CustomRestSiteAnimPath => "res://Mnemonist/scenes/rest/mnemonist_rest.tscn";
    public override string CustomCharacterSelectBg => "res://Mnemonist/scenes/ui/mnemonist_portrait.tscn";

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 10;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<MnemonistStrike>(),
        ModelDb.Card<MnemonistStrike>(),
        ModelDb.Card<MnemonistStrike>(),
        ModelDb.Card<MnemonistStrike>(),
        ModelDb.Card<MnemonistDefend>(),
        ModelDb.Card<MnemonistDefend>(),
        ModelDb.Card<MnemonistDefend>(),
        ModelDb.Card<MnemonistDefend>(),
        ModelDb.Card<NaggingThought>(),
        ModelDb.Card<ActiveRecall>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<IdyllicMemories>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<MnemonistCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MnemonistRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MnemonistPotionPool>();

    public override string CustomIconPath => "res://Mnemonist/scenes/character_icons/mnemonist_icon.tscn";
    public override string CustomIconTexturePath => "character_icon_mnemonist.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_mnemonist.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_mnemonist_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_mnemonist.png".CharacterUiPath();
    
    public void PlayAnimation(Creature? creature, string trigger)
    {
        if (creature == null || string.IsNullOrEmpty(trigger)) return;

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.Visuals == null) return;

        var animPlayer = node.Visuals.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer != null)
        {
            string godotTrigger = trigger.ToLowerInvariant();
            if (animPlayer.HasAnimation(godotTrigger))
            {
                var anim = animPlayer.GetAnimation(godotTrigger);

                animPlayer.Play(godotTrigger);
                if (godotTrigger != "idle_loop" && godotTrigger != "dead" && godotTrigger != "cast")
                {
                    animPlayer.Queue("idle_loop");
                }
            }
        }
    }
    
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    public override CreatureAnimator? GenerateAnimator(MegaSprite controller)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    {
        return null; 
    }
}