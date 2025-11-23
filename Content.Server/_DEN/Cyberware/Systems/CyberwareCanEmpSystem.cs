using Content.Server.Emp;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._DEN.Cyberware.Systems;


namespace Content.Server._DEN.Cyberware.Systems;


internal sealed class CyberwareCanEmpSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareCanEmpComponent, EmpPulseEvent>(OnEmpPulse);
        SubscribeLocalEvent<CyberwareCanEmpComponent, EmpDisabledRemoved>(OnEmpDisabledRemoved);
    }
    private void OnEmpPulse(Entity<CyberwareCanEmpComponent> cyberEnt, ref EmpPulseEvent ev)
    {
        if (cyberEnt.Comp.Disabled)
            return;

        ev.Affected = true;
        ev.Disabled = true;
        cyberEnt.Comp.Disabled = true;

        if (!HasComp<CyberwareComponent>(cyberEnt))
            return;

        var evt = new CyberwareDisabledEvent();
        RaiseLocalEvent(cyberEnt, ref evt);
    }

    private void OnEmpDisabledRemoved(Entity<CyberwareCanEmpComponent> cyberEnt, ref EmpDisabledRemoved ev)
    {
        if (!cyberEnt.Comp.Disabled)
            return;

        cyberEnt.Comp.Disabled = false;

        if (!HasComp<CyberwareComponent>(cyberEnt))
            return;

        var evt = new CyberwareEnabledEvent();
        RaiseLocalEvent(cyberEnt, ref evt);
    }
}
