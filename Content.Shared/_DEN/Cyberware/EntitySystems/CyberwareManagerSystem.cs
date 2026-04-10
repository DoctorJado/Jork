using Content.Shared._DEN.Cyberware.Components;


namespace Content.Shared._DEN.Cyberware.EntitySystems;


public sealed class CyberwareManagerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareManagerComponent, CyberwareCoprocessorInstalledEvent>(OnCoprocessorInstall);
        SubscribeLocalEvent<CyberwareManagerComponent, CyberwareCoprocessorRemovedEvent>(OnCoprocessorRemove);

        SubscribeLocalEvent<CyberwareManagerComponent, CyberwareInstalledEvent>(OnCyberwareInstall);
        SubscribeLocalEvent<CyberwareManagerComponent, CyberwareRemovedEvent>(OnCyberwareRemove);
    }

    public bool HasCoprocessor(EntityUid uid, out EntityUid coprocessorUid)
    {
        coprocessorUid = default;

        if (!TryComp<CyberwareManagerComponent>(uid, out var comp) || comp.CoprocessorUid is null)
            return false;

        coprocessorUid = comp.CoprocessorUid.Value;

        return true;
    }

    private void OnCoprocessorInstall(EntityUid uid, CyberwareManagerComponent comp, CyberwareCoprocessorInstalledEvent e)
    {
        comp.CoprocessorUid = e.Cyberware;
        EnableAllCyberware(uid, comp);
    }

    private void OnCoprocessorRemove(EntityUid uid, CyberwareManagerComponent comp, CyberwareCoprocessorRemovedEvent e)
    {
        comp.CoprocessorUid = null;
        DisableAllCyberware(uid, comp);
    }

    private void OnCyberwareInstall(EntityUid uid, CyberwareManagerComponent comp, CyberwareInstalledEvent e)
    {
        EnableCyberware(uid);
    }

    private void OnCyberwareRemove(EntityUid uid, CyberwareManagerComponent comp, CyberwareRemovedEvent e)
    {
        DisableCyberware(uid);
        EnableAllCyberware(uid, comp);
    }

    private void EnableAllCyberware(EntityUid uid, CyberwareManagerComponent comp)
    {
        foreach (var cyberware in comp.InstalledCyberware)
        {
            EnableCyberware(cyberware);
        }
    }

    private void DisableAllCyberware(EntityUid uid, CyberwareManagerComponent comp)
    {
        foreach (var cyberware in comp.InstalledCyberware)
        {
            DisableCyberware(cyberware);
        }
    }

    private void EnableCyberware(EntityUid uid)
    {
        var attemptEvt = new CyberwareAttemptEnabledEvent();
        RaiseLocalEvent(uid, ref attemptEvt);
        if(attemptEvt.Cancelled)
            return;

        var evt = new CyberwareEnabledEvent();
        RaiseLocalEvent(uid, ref evt);
    }

    private void DisableCyberware(EntityUid uid)
    {
        var attemptEvt = new CyberwareAttemptDisabledEvent();
        RaiseLocalEvent(uid, ref attemptEvt);
        if(attemptEvt.Cancelled)
            return;

        var evt = new CyberwareDisabledEvent();
        RaiseLocalEvent(uid, ref evt);
    }
}
