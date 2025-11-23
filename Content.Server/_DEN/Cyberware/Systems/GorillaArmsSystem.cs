
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Containers;


namespace Content.Server._DEN.Cyberware.Systems;
public sealed class GorillaArmsSystem : EntitySystem
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
