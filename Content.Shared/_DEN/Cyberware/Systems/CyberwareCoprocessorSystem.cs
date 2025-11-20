using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Systems;
using Robust.Shared.Serialization.Manager;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class CyberwareCoprocessorSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _bodySystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareCoprocessorComponent, OrganComponentsModifyEvent>(OnOrganCoprocessorComponentsModify);
    }

    public void OnOrganCoprocessorComponentsModify(
        EntityUid uid,
        CyberwareCoprocessorComponent component,
        OrganComponentsModifyEvent e
    )
    {
        if (e.Add)
        {
            AddComp<CyberwareCapableComponent>(e.Body);

            foreach (var part in _bodySystem.GetBodyPartChildren(e.Body))
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
        else
        {
            RemComp<CyberwareCapableComponent>(e.Body);

            foreach (var part in _bodySystem.GetBodyPartChildren(e.Body))
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
}
