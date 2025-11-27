using Content.Server.Body.Components;
using Content.Server.Popups;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._DEN.Cyberware.Systems;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Movement.Pulling.Components;


namespace Content.Server._DEN.Cyberware.Systems;


public sealed class CyberwareAddActionServerSystem : CyberwareAddActionSystem
{
    [Dependency] private CyberwareSystem _cyberwareSystem = default!;
    [Dependency] private SimpleCyberwareSystem _simpleCyberwareSystem = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareAddActionComponent, ActivateActionToggleGenericCyberwareEvent>(OnToggleGenericCyberware);
    }

    private void OnToggleGenericCyberware(
        EntityUid uid,
        CyberwareAddActionComponent comp,
        ActivateActionToggleGenericCyberwareEvent ev
    )
    {
        if(comp.TargetEntity is null)
            return;

        if (comp.EnabledState)
        {
            ChangeAddState(uid, comp, false);
            comp.EnabledState = false;
            if(comp.CyberwareDisabledPopupText is not null)
                _popup.PopupEntity(Loc.GetString(comp.CyberwareDisabledPopupText), comp.TargetEntity.Value, comp.TargetEntity.Value);
        }
        else
        {
            ChangeAddState(uid, comp, true);
            comp.EnabledState = true;
            if(comp.CyberwareEnabledPopupText is not null)
                _popup.PopupEntity(Loc.GetString(comp.CyberwareEnabledPopupText), comp.TargetEntity.Value, comp.TargetEntity.Value);
        }
    }
    // yes I know this is literally copy and pasted but it's just nicer to have it here.
    private void ChangeAddState(EntityUid uid, CyberwareAddActionComponent comp, bool enable)
    {
        if(!TryComp<CyberwareComponent>(uid, out var cyberwareComp))
            return;

        if (TryComp<BodyPartComponent>(cyberwareComp.Owner, out var bodyPartComp)
            && bodyPartComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var parentTarget))
        {
            Logger.Debug("enabling part cyberware: " + cyberwareComp.Owner);
            UpdateComponents(cyberwareComp, comp, parentTarget, enable);

        }

        else if (TryComp<OrganComponent>(cyberwareComp.Owner, out var organComp)
            && organComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(organComp.Body.Value, out var bodyTarget))
        {
            Logger.Debug("enabling organ cyberware: " + cyberwareComp.Owner);
            UpdateComponents(cyberwareComp, comp, bodyTarget, enable);
        }

        DirtyEntity(uid);
    }
    private void UpdateComponents(CyberwareComponent cyberwareComp, CyberwareAddActionComponent comp, EntityUid target, bool enable)
    {
        if (enable)
        {
            if(comp.AddParent is not null && _simpleCyberwareSystem.UpdateAddedComponents(comp.AddParent, target, cyberwareComp.Owner, true, out var addParentReg))
                _simpleCyberwareSystem.AddComponents(target, addParentReg);

            if(comp.AddSelf is not null)
                _simpleCyberwareSystem.AddComponents(cyberwareComp.Owner, comp.AddSelf);

            if(comp.RemoveParent is not null && _simpleCyberwareSystem.UpdateAddedComponents(comp.RemoveParent, target, cyberwareComp.Owner, false, out var removeParentReg))
                _simpleCyberwareSystem.RemoveComponents(target, removeParentReg);

            if(comp.RemoveSelf is not null)
                _simpleCyberwareSystem.RemoveComponents(cyberwareComp.Owner, comp.RemoveSelf);
        }
        else
        {
            if(comp.AddParent is not null && _simpleCyberwareSystem.UpdateAddedComponents(comp.AddParent, target, cyberwareComp.Owner, false, out var addParentReg))
                _simpleCyberwareSystem.RemoveComponents(target, addParentReg);

            if(comp.AddSelf is not null)
                _simpleCyberwareSystem.RemoveComponents(cyberwareComp.Owner, comp.AddSelf);

            if(comp.RemoveParent is not null && _simpleCyberwareSystem.UpdateAddedComponents(comp.RemoveParent, target, cyberwareComp.Owner, true, out var removeParentReg))
                _simpleCyberwareSystem.AddComponents(target, removeParentReg);

            if(comp.RemoveSelf is not null)
                _simpleCyberwareSystem.AddComponents(cyberwareComp.Owner, comp.RemoveSelf);
        }

        DirtyEntity(target);
        DirtyEntity(cyberwareComp.Owner);
    }
}
