using System.Linq;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.BodyEffects;
using Content.Shared.Body.Part;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class SimpleCyberwareSystem : EntitySystem
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
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
            if (AddComponents(body, comp.Active))
            {
                DirtyEntity(uid);
                DirtyEntity(body);
            }
        }
    }
    public void OnCyberwareEnable(EntityUid uid, SimpleCyberwareComponent component, CyberwareEnabledEvent ev)
    {
        if(!TryComp<CyberwareComponent>(uid, out var cyberwareComp) || cyberwareComp.Parent is null)
            return;

        if(component.AddParent is not null)
            AddComponents(cyberwareComp.Parent.Value, component.AddParent);

        if(component.AddSelf is not null)
            AddComponents(cyberwareComp.Parent.Value, component.AddSelf);

        if(component.RemoveParent is not null)
            RemoveComponents(cyberwareComp.Parent.Value, component.RemoveParent);

        if(component.RemoveSelf is not null)
            RemoveComponents(cyberwareComp.Parent.Value, component.RemoveSelf);

        DirtyEntity(uid);
        DirtyEntity(cyberwareComp.Parent.Value);
    }

    public void OnCyberwareDisable(EntityUid uid, SimpleCyberwareComponent component, CyberwareDisabledEvent ev)
    {
        if(!TryComp<CyberwareComponent>(uid, out var cyberwareComp) || cyberwareComp.Parent is null)
            return;

        if(component.AddParent is not null)
            RemoveComponents(cyberwareComp.Parent.Value, component.AddParent);

        if(component.AddSelf is not null)
            RemoveComponents(cyberwareComp.Parent.Value, component.AddSelf);

        if(component.RemoveParent is not null)
            AddComponents(cyberwareComp.Parent.Value, component.RemoveParent);

        if(component.RemoveSelf is not null)
            AddComponents(cyberwareComp.Parent.Value, component.RemoveSelf);

        DirtyEntity(uid);
        DirtyEntity(cyberwareComp.Parent.Value);
    }

    private bool AddComponents(EntityUid target,
        ComponentRegistry reg,
        BodyPartEffectComponent? effectComp = null)
    {
        if (!Resolve(target, ref effectComp, logMissing: false))
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
        ComponentRegistry reg,
        BodyPartEffectComponent? effectComp = null)
    {
        if (!Resolve(target, ref effectComp, logMissing: false))
            return;

        foreach (var (key, comp) in reg)
        {
            RemComp(target, comp.Component.GetType());
            effectComp.Active.Remove(key);
        }
    }
}
