using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Toolshed.Commands.Math;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class CyberwareCoprocessorSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _bodySystem = default!;
    [Dependency] private CyberwareSystem _cyberwareSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareCoprocessorComponent, OrganAddedToBodyEvent>(OnOrganCoprocessorAddedToBody);
        SubscribeLocalEvent<CyberwareCoprocessorComponent, OrganRemovedFromBodyEvent>(OnOrganCoprocessorRemovedFromBody);

        SubscribeLocalEvent<CyberwareCapableComponent, CyberwareComplexityTotalChange>(OnCyberwareComplexityTotalChange);
    }

    private void OnOrganCoprocessorAddedToBody(EntityUid uid, CyberwareCoprocessorComponent component, OrganAddedToBodyEvent e)
    {
        if (!_cyberwareSystem.TryRootBodyFromOrgan(e.Body, out var body) || !_cyberwareSystem.TryRootEntityFromOrgan(e.Body, out var root))
            return;

        Logger.Debug("coprocessor added, activating sigma mode");

        AddComp(body, new CyberwareCapableComponent{ CoprocessorUid = component.Owner }, true);

        foreach (var part in _bodySystem.GetBodyPartChildren(root))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id))
            {
                if (TryComp<CyberwareComponent>(organ.Id, out var organComp))
                {
                    Logger.Debug("coprocessor added, enabling organ");
                    var evt = new CyberwareEnabledEvent();
                    RaiseLocalEvent(organComp.Owner, ref evt);
                }
            }

            if (TryComp<CyberwareComponent>(part.Id, out var partComp))
            {
                var evt = new CyberwareEnabledEvent();
                RaiseLocalEvent(partComp.Owner, ref evt);
            }
        }
        DirtyEntity(body);
    }

    private void OnOrganCoprocessorRemovedFromBody(EntityUid uid, CyberwareCoprocessorComponent component, OrganRemovedFromBodyEvent e)
    {
        if (!_cyberwareSystem.TryRootBodyFromOrgan(e.OldBody, out var body) || !_cyberwareSystem.TryRootEntityFromOrgan(e.OldBody, out var root))
            return;

        Logger.Debug("coprocessor removed, disabling sigma mode");

        RemComp<CyberwareCapableComponent>(body);

        foreach (var part in _bodySystem.GetBodyPartChildren(root))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id, part.Component))
            {
                if (TryComp<CyberwareComponent>(organ.Id, out var organComp))
                {
                    Logger.Debug("coprocessor removed, disabling organ");
                    var evt = new CyberwareDisabledEvent();
                    RaiseLocalEvent(organComp.Owner, ref evt);
                }
            }

            if (TryComp<CyberwareComponent>(part.Id, out var partComp))
            {
                var evt = new CyberwareDisabledEvent();
                RaiseLocalEvent(partComp.Owner, ref evt);
            }
        }
        DirtyEntity(body);
    }

    private void OnCyberwareComplexityTotalChange(
        EntityUid uid,
        CyberwareCapableComponent component,
        CyberwareComplexityTotalChange e
    )
    {
        Logger.Debug("someone farted and increased gyatt levels to: " + component.CurrentUsage);

        if (!TryComp<CyberwareCoprocessorComponent>(component.CoprocessorUid, out var coprocessorComp))
            return;

        if (component.CurrentUsage > coprocessorComp.SafeTolerance)
        {
            // do medium shit
        }

        if (component.CurrentUsage > coprocessorComp.DangerTolerance)
        {
            // do pretty fucking bad shit
        }

        if (component.CurrentUsage > coprocessorComp.MaximumTolerance)
        {
            // fucking kill them probably
        }
    }
}
