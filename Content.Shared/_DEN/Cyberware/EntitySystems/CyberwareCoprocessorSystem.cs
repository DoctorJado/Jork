using Content.Shared._DEN.Cyberware.Components;
using Content.Shared.Body.Events;


namespace Content.Shared._DEN.Cyberware.EntitySystems;


public sealed class CyberwareCoprocessorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareCoprocessorComponent, OrganAddedToBodyEvent>(OnCoprocessorAdded);
        SubscribeLocalEvent<CyberwareCoprocessorComponent, OrganRemovedFromBodyEvent>(OnCoprocessorRemoved);
    }

    private void OnCoprocessorAdded(EntityUid uid, CyberwareCoprocessorComponent component, OrganAddedToBodyEvent e)
    {
        var evt = new CyberwareCoprocessorInstalledEvent();
        RaiseLocalEvent(e.Body, ref evt);
    }
    private void OnCoprocessorRemoved(EntityUid uid, CyberwareCoprocessorComponent component, OrganRemovedFromBodyEvent e)
    {
        var evt = new CyberwareCoprocessorRemovedEvent();
        RaiseLocalEvent(e.OldBody, ref evt);
    }
}

[ByRefEvent]
public readonly record struct CyberwareCoprocessorInstalledEvent(Entity<CyberwareCoprocessorComponent> Cyberware);

[ByRefEvent]
public readonly record struct CyberwareCoprocessorRemovedEvent(Entity<CyberwareCoprocessorComponent> Cyberware);
