using Content.Server._DEN.Cybernetics.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Containers;

namespace Content.Server.FloofStation.Traits.Cybernetics.Systems;
public sealed partial class BoxingSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GorillaArmsComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(EntityUid uid, GorillaArmsComponent component, MeleeHitEvent args)
    {
        args.ModifiersList.Add(component.UnarmedModifiers);
    }
}
