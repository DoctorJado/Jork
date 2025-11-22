using System.Linq;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.BodyEffects;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class SimpleCyberwareSystem : EntitySystem
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private CyberwareSystem _cyberwareSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SimpleCyberwareComponent, CyberwareEnabledEvent>(OnCyberwareEnable);
        SubscribeLocalEvent<SimpleCyberwareComponent, CyberwareDisabledEvent>(OnCyberwareDisable);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BodyPartEffectComponent, BodyPartComponent>();
        var now = _gameTiming.CurTime;
        while (query.MoveNext(out var uid, out var comp, out var part))
        {
            if (now < comp.NextUpdate || !comp.Active.Any() || part.Body is not { } body)
                continue;

            comp.NextUpdate = now + comp.Delay;
            if (EvilAddComponents(body, uid, comp.Active))
            {
                DirtyEntity(uid);
                DirtyEntity(body);
            }
        }
    }
    public void OnCyberwareEnable(EntityUid uid, SimpleCyberwareComponent component, CyberwareEnabledEvent ev)
    {
        if(!TryComp<CyberwareComponent>(uid, out var cyberwareComp))
            return;

        if (TryComp<BodyPartComponent>(cyberwareComp.Owner, out var bodyPartComp)
            && bodyPartComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var parentTarget))
        {
            Logger.Debug("enabling part cyberware: " + cyberwareComp.Owner);
            EnableCyberware(cyberwareComp, component, parentTarget);

            _cyberwareSystem.UpdateComplexityTotal(cyberwareComp, parentTarget, true);
        }

        else if (TryComp<OrganComponent>(cyberwareComp.Owner, out var organComp)
            && organComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(organComp.Body.Value, out var bodyTarget))
        {
            Logger.Debug("enabling organ cyberware: " + cyberwareComp.Owner);
            EnableCyberware(cyberwareComp, component, bodyTarget);

            _cyberwareSystem.UpdateComplexityTotal(cyberwareComp, bodyTarget, true);
        }


        DirtyEntity(uid);
    }

    public void OnCyberwareDisable(EntityUid uid, SimpleCyberwareComponent component, CyberwareDisabledEvent ev)
    {
        if(!TryComp<CyberwareComponent>(uid, out var cyberwareComp))
            return;

        if (TryComp<BodyPartComponent>(cyberwareComp.Owner, out var bodyPartComp)
            && bodyPartComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(bodyPartComp.Body.Value, out var parentTarget))
        {
            DisableCyberware(cyberwareComp, component, parentTarget);

            _cyberwareSystem.UpdateComplexityTotal(cyberwareComp, parentTarget, false);
        }

        else if (TryComp<OrganComponent>(cyberwareComp.Owner, out var organComp)
            && organComp.Body is not null
            && _cyberwareSystem.TryRootBodyFromOrgan(organComp.Body.Value, out var bodyTarget))
        {
            DisableCyberware(cyberwareComp, component, bodyTarget);

            _cyberwareSystem.UpdateComplexityTotal(cyberwareComp, bodyTarget, false);
        }


        DirtyEntity(uid);
    }

    private void EnableCyberware(CyberwareComponent cyberwareComp, SimpleCyberwareComponent comp, EntityUid target)
    {
        if(comp.AddParent is not null)
            AddComponents(target, comp.AddParent);

        if(comp.AddSelf is not null)
            AddComponents(cyberwareComp.Owner, comp.AddSelf);

        if(comp.RemoveParent is not null)
            RemoveComponents(target, comp.RemoveParent);

        if(comp.RemoveSelf is not null)
            RemoveComponents(cyberwareComp.Owner, comp.RemoveSelf);

        DirtyEntity(target);
        DirtyEntity(cyberwareComp.Owner);
    }

    private void DisableCyberware(CyberwareComponent cyberwareComp, SimpleCyberwareComponent comp, EntityUid target)
    {
        if(comp.AddParent is not null)
            RemoveComponents(target, comp.AddParent);

        if(comp.AddSelf is not null)
            RemoveComponents(cyberwareComp.Owner, comp.AddSelf);

        if(comp.RemoveParent is not null)
            AddComponents(target, comp.RemoveParent);

        if(comp.RemoveSelf is not null)
            AddComponents(cyberwareComp.Owner, comp.RemoveSelf);

        DirtyEntity(target);
        DirtyEntity(cyberwareComp.Owner);
    }

    private void AddComponents(EntityUid target,
        ComponentRegistry reg)
    {
        foreach (var (key, comp) in reg)
        {
            var compType = comp.Component.GetType();
            if (HasComp(target, compType))
                continue;

            var newComp = (Component) _serManager.CreateCopy(comp.Component, notNullableOverride: true);
            EntityManager.AddComponent(target, newComp, true);
            Logger.Debug("adding component " + newComp.ToString());
        }
    }

    private bool EvilAddComponents(EntityUid target,
        EntityUid self,
        ComponentRegistry reg,
        BodyPartEffectComponent? effectComp = null)
    {
        if (!Resolve(self, ref effectComp, logMissing: false))
            return false;

        bool changed = false;

        foreach (var (key, comp) in reg)
        {
            var compType = comp.Component.GetType();
            if (HasComp(target, compType))
                continue;

            var newComp = (Component) _serManager.CreateCopy(comp.Component, notNullableOverride: true);
            EntityManager.AddComponent(target, newComp, true);
            changed = true;
            effectComp.Active[key] = comp;
        }
        return changed;
    }

    private void RemoveComponents(EntityUid target,
        ComponentRegistry reg)
    {
        foreach (var (key, comp) in reg)
        {
            EntityManager.RemoveComponent(target, comp.Component.GetType());
            Logger.Debug("removing component " + comp.Component.GetType());
        }
    }
}
