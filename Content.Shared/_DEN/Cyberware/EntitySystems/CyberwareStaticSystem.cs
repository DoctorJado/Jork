using Content.Shared._DEN.Cyberware.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;


namespace Content.Shared._DEN.Cyberware.EntitySystems;


public sealed class CyberwareStaticSystem : EntitySystem
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareStaticComponent, CyberwareEnabledEvent>(OnCyberwareEnable);
        SubscribeLocalEvent<CyberwareStaticComponent, CyberwareDisabledEvent>(OnCyberwareDisable);
    }

    private void OnCyberwareEnable(EntityUid uid, CyberwareStaticComponent comp, CyberwareEnabledEvent e)
    {
        if(comp.AddSelf is not null)
            AddComponents(uid, comp.AddSelf);

        if(comp.AddBody is not null
            && TryComp<CyberwareComponent>(uid, out var cyberwareComp)
            && cyberwareComp.RootEntity is not null)
            AddComponents(cyberwareComp.RootEntity.Value, comp.AddBody);

        comp.IsEnabled = true;
    }

    private void OnCyberwareDisable(EntityUid uid, CyberwareStaticComponent comp, CyberwareDisabledEvent e)
    {
        if(comp.AddSelf is not null)
            RemoveComponents(uid, comp.AddSelf);

        if(comp.AddBody is not null
            && TryComp<CyberwareComponent>(uid, out var cyberwareComp)
            && cyberwareComp.RootEntity is not null)
            RemoveComponents(cyberwareComp.RootEntity.Value, comp.AddBody);

        comp.IsEnabled = false;
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
        }
    }

    private void RemoveComponents(EntityUid target,
        ComponentRegistry reg)
    {
        foreach (var (key, comp) in reg)
        {
            EntityManager.RemoveComponent(target, comp.Component.GetType());
        }
    }
}
