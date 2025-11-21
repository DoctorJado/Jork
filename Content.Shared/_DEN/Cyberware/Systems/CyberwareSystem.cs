using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Body.Organ;
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

        SubscribeLocalEvent<CyberwareComponent, BodyPartComponentsModifyEvent>(OnPartComponentsModify);
        SubscribeLocalEvent<CyberwareComponent, OrganComponentsModifyEvent>(OnOrganComponentsModify);
    }

    private void UpdateComplexityTotal(CyberwareComponent component, EntityUid evt, bool enabled)
    {
        if (!TryRootBodyFromOrgan(evt, out var body) || !TryComp<CyberwareCapableComponent>(body, out var capable))
            return;

        if (enabled)
            capable.CurrentUsage += component.Complexity;
        else
            capable.CurrentUsage -= component.Complexity;

        DirtyEntity(body);
        Logger.Debug("update complexity total: " + capable.CurrentUsage);
    }

    private bool SymmetryComponentCheck(EntityUid body, CyberwareSymmetryComponent symmetry)
    {
        foreach (var part in _bodySystem.GetBodyPartChildren(body))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id, part.Component))
            {
                if (TryComp<CyberwareSymmetryComponent>(organ.Id, out var organComp))
                    if (organComp.SelfID == symmetry.SisterID)
                    {
                        Logger.Debug("symmetry organ success");
                        return true;
                    }
            }

            if (TryComp<CyberwareSymmetryComponent>(part.Id, out var partComp))
                if (partComp.SelfID == symmetry.SisterID)
                {
                    Logger.Debug("symmetry part success");
                    return true;
                }
        }
        Logger.Debug("symmetry fail");
        return false;
    }
    public void OnPartComponentsModify(EntityUid uid, CyberwareComponent component, BodyPartComponentsModifyEvent e)
    {
        UpdateComplexityTotal(component, e.Body, e.Add);

        if(e.Add)
            component.Parent = e.Body;
        else
            component.Parent = null;

        if (!TryComp<BodyPartComponent>(component.Owner, out var bodyPartComp)
            || bodyPartComp.Body is null || !TryRootEntityFromOrgan(bodyPartComp.Body.Value, out var body))
            return;

        if (TryComp<CyberwareSymmetryComponent>(component.Owner, out var symmetry)
            && !SymmetryComponentCheck(body, symmetry))
            return;

        Logger.Debug("symmetry check part passed");

        if (e.Add)
        {
            var evt = new CyberwareEnabledEvent();
            RaiseLocalEvent(uid, ref evt);
        }
        else
        {
            var evt = new CyberwareDisabledEvent();
            RaiseLocalEvent(uid, ref evt);
        }
    }

    public void OnOrganComponentsModify(EntityUid uid, CyberwareComponent component, OrganComponentsModifyEvent e)
    {
        UpdateComplexityTotal(component, e.Body, e.Add);

        if(e.Add)
            component.Parent = e.Body;
        else
            component.Parent = null;

        if (!TryComp<OrganComponent>(component.Owner, out var organComp)
            || organComp.Body is null || !TryRootEntityFromOrgan(organComp.Body.Value, out var root))
            return;

        if (TryComp<CyberwareSymmetryComponent>(component.Owner, out var symmetry)
            && !SymmetryComponentCheck(root, symmetry))
            return;

        Logger.Debug("symmetry check organ passed");

        if (e.Add)
        {
            var evt = new CyberwareEnabledEvent();
            RaiseLocalEvent(uid, ref evt);
        }
        else
        {
            var evt = new CyberwareDisabledEvent();
            RaiseLocalEvent(uid, ref evt);
        }
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

        ;
        return true;
    }

    private (EntityUid Entity, BodyPartComponent BodyPart)? TryRootFromOrgan(EntityUid organ)
    {
        return _bodySystem.GetRootPartOrNull(organ);
    }
}

[ByRefEvent]
public readonly record struct CyberwareEnabledEvent(Entity<CyberwareComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareDisabledEvent(Entity<CyberwareComponent> Cyberware);
