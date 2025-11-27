using System.Diagnostics;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared.Actions;
using Content.Shared.Body.Events;
using Content.Shared.Body.Part;
using Robust.Shared.Network;


namespace Content.Shared._DEN.Cyberware.Systems;


public abstract class CyberwareAddActionSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private CyberwareSystem _cyberwareSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareAddActionComponent, CyberwareEnabledEvent>(OnCyberwareEnable);
        SubscribeLocalEvent<CyberwareAddActionComponent, CyberwareDisabledEvent>(OnCyberwareDisable);

        SubscribeLocalEvent<CyberwareAddActionComponent, OrganAddedToBodyEvent>(OnAddedOrgan);
        SubscribeLocalEvent<CyberwareAddActionComponent, BodyPartAttachedEvent>(OnAddedPart);
    }

    private void OnCyberwareEnable(EntityUid uid, CyberwareAddActionComponent component, CyberwareEnabledEvent ev)
    {
        if (_net.IsClient || component.TargetEntity is null)
            return;

        if (!string.IsNullOrWhiteSpace(component.CyberwareAction))
        {
            _actionsSystem.AddAction(component.TargetEntity.Value, ref component.Action, component.CyberwareAction, uid);
        }
    }

    private void OnCyberwareDisable(EntityUid uid, CyberwareAddActionComponent component, CyberwareDisabledEvent ev)
    {
        _actionsSystem.RemoveAction(component.Action);
    }

    private void OnAddedOrgan(EntityUid uid, CyberwareAddActionComponent component, OrganAddedToBodyEvent ev)
    {
        if(_cyberwareSystem.TryRootBodyFromOrgan(ev.Body, out var body))
            component.TargetEntity = body;
    }

    private void OnAddedPart(EntityUid uid, CyberwareAddActionComponent component, BodyPartAttachedEvent ev)
    {
        if (TryComp<BodyPartComponent>(ev.Part.Owner, out var bodyPartComp)
            && bodyPartComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var body))
            component.TargetEntity = body;
    }

}
