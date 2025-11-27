using System.Diagnostics;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared.Actions;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Part;
using Content.Shared.Popups;
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
        SubscribeLocalEvent<CyberwareAddActionComponent, CyberwareInstalledEvent>(OnCyberwareInstall);
    }

    private void OnCyberwareEnable(EntityUid uid, CyberwareAddActionComponent component, CyberwareEnabledEvent ev)
    {
        if (_net.IsClient || component.TargetEntity is null || !TryComp<CyberwareComponent>(component.Owner, out var cyberwareComp))
            return;

        _cyberwareSystem.UpdateComplexityTotal(cyberwareComp, component.TargetEntity.Value, true);

        if (!string.IsNullOrWhiteSpace(component.CyberwareAction))
        {
            _actionsSystem.AddAction(component.TargetEntity.Value, ref component.Action, component.CyberwareAction, uid);
        }
    }

    private void OnCyberwareDisable(EntityUid uid, CyberwareAddActionComponent component, CyberwareDisabledEvent ev)
    {
        if (_net.IsClient || component.TargetEntity is null || !TryComp<CyberwareComponent>(component.Owner, out var cyberwareComp))
            return;

        _cyberwareSystem.UpdateComplexityTotal(cyberwareComp, component.TargetEntity.Value, false);

        _actionsSystem.RemoveAction(component.Action);
    }

    private void OnCyberwareInstall(EntityUid uid, CyberwareAddActionComponent component, CyberwareInstalledEvent ev)
    {
        if (TryComp<BodyPartComponent>(uid, out var bodyPartComp)
            && bodyPartComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var body))
            component.TargetEntity = body;
        else if(_cyberwareSystem.TryRootBodyFromOrgan(ev.Cyberware.Owner, out var organ))
            component.TargetEntity = organ;
    }
}
