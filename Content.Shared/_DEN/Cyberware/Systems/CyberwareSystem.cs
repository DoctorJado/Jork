using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class CyberwareSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _bodySystem = default!;
    [Dependency] private CyberwareSystem _cyberwareSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BodyComponent, BodyPartAttachedEvent>(OnPartAddedToBody);
        SubscribeLocalEvent<BodyComponent, BodyPartDroppedEvent>(OnPartRemovedFromBody);

        SubscribeLocalEvent<CyberwareComponent, OrganAddedToBodyEvent>(OnOrganAddedToBody);
        SubscribeLocalEvent<CyberwareComponent, OrganRemovedFromBodyEvent>(OnOrganRemovedFromBody);
    }

    public void UpdateComplexityTotal(CyberwareComponent component, EntityUid root, bool enabled)
    {
        if (!TryComp<CyberwareCapableComponent>(root, out var capable))
            return;

        if (enabled)
            capable.CurrentUsage += component.Complexity;
        else
            capable.CurrentUsage -= component.Complexity;

        DirtyEntity(root);

        var evt = new CyberwareComplexityTotalChange();
        RaiseLocalEvent(root, ref evt);
    }

    public void OnOrganAddedToBody(EntityUid uid, CyberwareComponent component, OrganAddedToBodyEvent e)
    {
        // TODO: implement symmetry properly like OnPartAddedToBody

        component.Parent = e.Part;
        TriggerCyberwareInstallEvent(uid);

        if (TryComp<CyberwareSymmetryComponent>(component.Owner, out var symmetry)
            && !SymmetryComponentCheck(e.Body, symmetry, out var sisterOrgan)
            || !TryRootBodyFromOrgan(e.Body, out var body)
            || !TryComp<CyberwareCapableComponent>(body, out _))
            return;

        var evt = new CyberwareEnabledEvent();
        RaiseLocalEvent(uid, ref evt);
    }

    public void OnOrganRemovedFromBody(EntityUid uid, CyberwareComponent component, ref OrganRemovedFromBodyEvent e)
    {
        // TODO: implement symmetry properly like OnPartRemovedFromBody

        component.Parent = null;

        if (TryComp<CyberwareSymmetryComponent>(component.Owner, out var symmetry)
            && !SymmetryComponentCheck(e.OldBody, symmetry, out var sisterOrgan)
            || !TryRootBodyFromOrgan(e.OldBody, out var body)
            || !TryComp<CyberwareCapableComponent>(body, out _))
            return;

        var evt = new CyberwareDisabledEvent();
        RaiseLocalEvent(uid, ref evt);
    }

    public void OnPartAddedToBody(EntityUid uid, BodyComponent component, ref BodyPartAttachedEvent e)
    {
        if (!TryComp<CyberwareComponent>(e.Part, out var cyberwareComp))
            return;

        cyberwareComp.Parent = e.Part;
        TriggerCyberwareInstallEvent(cyberwareComp.Owner);

        if (!TryComp<BodyPartComponent>(e.Part.Owner, out var bodyPartComp)
            || bodyPartComp.Body is null
            || !TryRootEntityFromOrgan(bodyPartComp.Body.Value, out var body)
            || !TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var bodyEnt)
            || !TryComp<CyberwareCapableComponent>(bodyEnt, out _))
            return;

        EntityUid sisterPart = default;
        if (TryComp<CyberwareSymmetryComponent>(e.Part, out var symmetry)
            && !SymmetryComponentCheck(body, symmetry, out sisterPart))
            return;

        var evt = new CyberwareEnabledEvent();
        RaiseLocalEvent(cyberwareComp.Owner, ref evt);
        if(sisterPart.Valid)
            RaiseLocalEvent(sisterPart, ref evt);
    }

    public void OnPartRemovedFromBody(EntityUid uid, BodyComponent component, BodyPartDroppedEvent e)
    {
        if (!TryComp<CyberwareComponent>(e.Part, out var cyberwareComp))
            return;

        cyberwareComp.Parent = null;

        if (!TryComp<BodyPartComponent>(e.Part.Owner, out var bodyPartComp)
            || bodyPartComp.Body is null
            || !TryRootEntityFromOrgan(bodyPartComp.Body.Value, out var body)
            || !TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var bodyEnt)
            || !TryComp<CyberwareCapableComponent>(bodyEnt, out _))
            return;

        EntityUid sisterPart = default;
        if (TryComp<CyberwareSymmetryComponent>(e.Part, out var symmetry)
            && !SymmetryComponentCheck(body, symmetry, out sisterPart))
            return;

        var evt = new CyberwareDisabledEvent();
        RaiseLocalEvent(cyberwareComp.Owner, ref evt);
        if(sisterPart.Valid)
            RaiseLocalEvent(sisterPart, ref evt);
    }

    public bool TryRootBodyFromOrgan(EntityUid organ, out EntityUid body)
    {
        var root = _bodySystem.GetRootPartOrNull(organ);

        body = default;

        if (root is null || root.Value.BodyPart.Body is null)
            return false;

        body =  root.Value.BodyPart.Body.Value;

        return true;
    }
    public bool TryRootEntityFromOrgan(EntityUid organ, out EntityUid body)
    {
        var root = TryRootFromOrgan(organ);

        body = default;

        if (root is null)
            return false;

        body = root.Value.Entity;

        return true;
    }

    private (EntityUid Entity, BodyPartComponent BodyPart)? TryRootFromOrgan(EntityUid organ)
    {
        return _bodySystem.GetRootPartOrNull(organ);
    }

    private bool SymmetryComponentCheck(EntityUid body, CyberwareSymmetryComponent symmetry, out EntityUid entity)
    {
        entity = new EntityUid();

        foreach (var part in _bodySystem.GetBodyPartChildren(body))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id, part.Component))
            {
                if (TryComp<CyberwareSymmetryComponent>(organ.Id, out var organComp))
                    if (organComp.SelfID == symmetry.SisterID)
                    {
                        Logger.Debug("symmetry organ success");
                        entity = organComp.Owner;
                        return true;
                    }
            }

            if (TryComp<CyberwareSymmetryComponent>(part.Id, out var partComp))
                if (partComp.SelfID == symmetry.SisterID)
                {
                    Logger.Debug("symmetry part success");
                    entity = partComp.Owner;
                    return true;
                }
        }
        Logger.Debug("symmetry fail");
        return false;
    }

    private void TriggerCyberwareInstallEvent(EntityUid uid)
    {
        var evt = new CyberwareInstalledEvent();
        RaiseLocalEvent(uid, ref evt);
    }
}

[ByRefEvent]
public readonly record struct CyberwareEnabledEvent(Entity<CyberwareComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareDisabledEvent(Entity<CyberwareComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareComplexityTotalChange(Entity<CyberwareCapableComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareInstalledEvent(Entity<CyberwareComponent> Cyberware);
