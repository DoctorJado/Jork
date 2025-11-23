using Content.Shared._DEN.Cyberware.Systems;
using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem), typeof(CyberwareCoprocessorSystem))]
public sealed partial class CyberwareCapableComponent : Component
{
    [DataField, AutoNetworkedField]
    public int CurrentUsage = 0;

    [DataField, AutoNetworkedField]
    public EntityUid CoprocessorUid;
}
