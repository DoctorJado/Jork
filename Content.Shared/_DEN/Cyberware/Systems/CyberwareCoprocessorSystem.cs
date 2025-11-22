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
    }

    public void OnOrganCoprocessorAddedToBody(EntityUid uid, CyberwareCoprocessorComponent component, OrganAddedToBodyEvent e)
    {
        if (!_cyberwareSystem.TryRootBodyFromOrgan(e.Body, out var body))
            return;

        AddComp(body, new CyberwareCapableComponent(), true);

        foreach (var part in _bodySystem.GetBodyPartChildren(body))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id))
            {
                if (HasComp<CyberwareComponent>(organ.Id))
                {
                    var evt = new OrganEnabledEvent();
                    RaiseLocalEvent(organ.Id, ref evt);
                }
            }

            if (HasComp<CyberwareComponent>(part.Id))
            {
                var evt = new BodyPartEnabledEvent();
                RaiseLocalEvent(part.Id, ref evt);
            }
        }
    }

    public void OnOrganCoprocessorRemovedFromBody(EntityUid uid, CyberwareCoprocessorComponent component, OrganRemovedFromBodyEvent e)
    {
        if (!_cyberwareSystem.TryRootBodyFromOrgan(e.OldBody, out var body))
            return;

        RemComp<CyberwareCapableComponent>(body);

        foreach (var part in _bodySystem.GetBodyPartChildren(body))
        {
            foreach (var organ in _bodySystem.GetPartOrgans(part.Id))
            {
                if (HasComp<CyberwareComponent>(organ.Id))
                {
                    var evt = new OrganDisabledEvent();
                    RaiseLocalEvent(organ.Id, ref evt);
                }
            }

            if (HasComp<CyberwareComponent>(part.Id))
            {
                var evt = new BodyPartDisabledEvent();
                RaiseLocalEvent(part.Id, ref evt);
            }
        }
    }
}
