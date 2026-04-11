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

    private void OnCoprocessorAdded(EntityUid uid, CyberwareCoprocessorComponent comp, OrganAddedToBodyEvent e)
    {
        var evt = new CyberwareCoprocessorInstalledEvent((uid, comp));
        RaiseLocalEvent(e.Body, ref evt);
    }

    private void OnCoprocessorRemoved(EntityUid uid, CyberwareCoprocessorComponent comp, OrganRemovedFromBodyEvent e)
    {
        var evt = new CyberwareCoprocessorRemovedEvent((uid, comp));
        RaiseLocalEvent(e.OldBody, ref evt);
    }
}

