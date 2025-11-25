using Content.Server.Body.Components;
using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._DEN.Cyberware.Systems;


namespace Content.Server._DEN.Cyberware.Systems;


public sealed class CyberwareAddActionServerSystem : CyberwareAddActionSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CyberwareAddActionComponent, ActivateActionCyberwareEvent>(OnCyberwareAction);
    }

    private void OnCyberwareAction(
        EntityUid uid,
        CyberwareAddActionComponent component,
        ActivateActionCyberwareEvent ev
    )
    {
        AddComp(ev.Performer, new CyberwareComponent(), true);
        DirtyEntity(ev.Performer);

        if(TryComp<MetabolizerComponent>(uid, out var metabolizer))
            Logger.Debug("massive poopie shit.... " + metabolizer.MaxReagentsProcessable);

    }
}
