using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;

namespace Content.Shared._DEN.Cyberware.EntitySystems;


public sealed class CyberwareSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _bodySystem = default!;
    [Dependency] private CyberwareManagerSystem _cyberwareManagerSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BodyComponent, BodyPartAttachedEvent>(OnPartAddedToBody);
        SubscribeLocalEvent<BodyComponent, BodyPartDroppedEvent>(OnPartRemovedFromBody);

        SubscribeLocalEvent<CyberwareComponent, OrganAddedToBodyEvent>(OnOrganAddedToBody);
        SubscribeLocalEvent<CyberwareComponent, OrganRemovedFromBodyEvent>(OnOrganRemovedFromBody);
    }

    private void OnOrganAddedToBody(EntityUid uid, CyberwareComponent comp, OrganAddedToBodyEvent e)
    {
        comp.RootEntity = e.Body;

        var evt = new CyberwareInstalledEvent();
        RaiseLocalEvent(uid, ref evt);
        if(_cyberwareManagerSystem.HasCoprocessor(e.Body, out _))
            RaiseLocalEvent(e.Body, ref evt);

        var manager = EnsureComp<CyberwareManagerComponent>(e.Body);
        manager.InstalledCyberware.Add(uid);
        UpdateComplexityTotal((uid, comp), (e.Body, manager), true);
    }

    private void OnOrganRemovedFromBody(EntityUid uid, CyberwareComponent comp, OrganRemovedFromBodyEvent e)
    {
        comp.RootEntity = null;

        if(!TryComp<CyberwareManagerComponent>(e.OldBody, out var manager))
            return;

        manager.InstalledCyberware.Remove(uid);
        UpdateComplexityTotal((uid, comp), (e.OldBody, manager), false);

        var evt = new CyberwareRemovedEvent();
        RaiseLocalEvent(uid, ref evt);
        if(_cyberwareManagerSystem.HasCoprocessor(e.OldBody, out _))
            RaiseLocalEvent(e.OldBody, ref evt);
    }

    private void OnPartAddedToBody(EntityUid uid, BodyComponent comp, BodyPartAttachedEvent e)
    {
        if (!TryComp<CyberwareComponent>(e.Part, out var cyberwareComp))
            return;

        cyberwareComp.RootEntity = uid;

        var evt = new CyberwareInstalledEvent();
        RaiseLocalEvent(e.Part, ref evt);
        if(_cyberwareManagerSystem.HasCoprocessor(uid, out _))
            RaiseLocalEvent(uid, ref evt);

        var manager = EnsureComp<CyberwareManagerComponent>(uid);
        manager.InstalledCyberware.Add(uid);
        UpdateComplexityTotal((e.Part, cyberwareComp), (uid, manager), true);
    }

    private void OnPartRemovedFromBody(EntityUid uid, BodyComponent comp, BodyPartDroppedEvent e)
    {
        if (!TryComp<CyberwareComponent>(e.Part, out var cyberwareComp))
            return;

        cyberwareComp.RootEntity = null;

        if(!TryComp<CyberwareManagerComponent>(uid, out var manager))
            return;

        manager.InstalledCyberware.Remove(uid);
        UpdateComplexityTotal((e.Part, cyberwareComp), (uid, manager), false);

        var evt = new CyberwareRemovedEvent();
        RaiseLocalEvent(e.Part, ref evt);
        if(_cyberwareManagerSystem.HasCoprocessor(uid, out _))
            RaiseLocalEvent(uid, ref evt);
    }

    private void UpdateComplexityTotal(Entity<CyberwareComponent> cyberwareEnt, Entity<CyberwareManagerComponent> managerEnt, bool enabled)
    {
        if (enabled)
            managerEnt.Comp.CurrentComplexity += cyberwareEnt.Comp.ComplexityScore;
        else
            managerEnt.Comp.CurrentComplexity -= cyberwareEnt.Comp.ComplexityScore;

        var evt = new CyberwareComplexityTotalChange();
        RaiseLocalEvent(managerEnt, ref evt);
        Dirty(managerEnt);
    }
}

[ByRefEvent]
public readonly record struct CyberwareComplexityTotalChange(Entity<CyberwareManagerComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareInstalledEvent();

[ByRefEvent]
public readonly record struct CyberwareRemovedEvent();

[ByRefEvent]
public readonly record struct CyberwareEnabledEvent();

[ByRefEvent]
public sealed class CyberwareAttemptEnabledEvent() : CancellableEntityEventArgs;

[ByRefEvent]
public readonly record struct CyberwareDisabledEvent();

[ByRefEvent]
public sealed class CyberwareAttemptDisabledEvent() : CancellableEntityEventArgs;
