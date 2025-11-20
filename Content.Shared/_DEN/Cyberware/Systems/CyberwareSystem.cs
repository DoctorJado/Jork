using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Systems;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class CyberwareSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _bodySystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareComponent, BodyPartComponentsModifyEvent>(OnPartComponentsModify);
        SubscribeLocalEvent<CyberwareComponent, OrganComponentsModifyEvent>(OnOrganComponentsModify);
    }

    private void UpdateComplexityTotal(CyberwareComponent component, EntityUid e, bool enabled)
    {
        if (!TryComp<CyberwareCapableComponent>(e, out var capable))
            return;

        if (enabled)
            capable.CurrentUsage += component.Complexity;
        else
            capable.CurrentUsage -= component.Complexity;
    }

    private bool SymmetryComponentCheck(EntityUid cyberware, EntityUid body, CyberwareSymmetryComponent symmetry)
    {
        foreach (var part in _bodySystem.GetBodyPartChildren(body))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id))
            {
                if (TryComp<CyberwareSymmetryComponent>(organ.Id, out var organComp))
                    if(organComp.SymmetryID == symmetry.SymmetryID)
                        return true;
            }

            if (TryComp<CyberwareSymmetryComponent>(part.Id, out var partComp))
                if(partComp.SymmetryID == symmetry.SymmetryID)
                    return true;
        }
        return false;
    }
    public void OnPartComponentsModify(EntityUid uid, CyberwareComponent component, BodyPartComponentsModifyEvent e)
    {
        UpdateComplexityTotal(component, e.Body, e.Add);

        if(e.Add)
            component.Parent = e.Body;
        else
            component.Parent = null;

        if (TryComp<CyberwareSymmetryComponent>(uid, out var symmetry) && !SymmetryComponentCheck(uid, e.Body, symmetry))
            return;

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

        if (TryComp<CyberwareSymmetryComponent>(uid, out var symmetry) && !SymmetryComponentCheck(uid, e.Body, symmetry))
            return;

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
}

[ByRefEvent]
public readonly record struct CyberwareEnabledEvent(Entity<CyberwareComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareDisabledEvent(Entity<CyberwareComponent> Cyberware);
