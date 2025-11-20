using Content.Shared._DEN.Cyberware.Systems;
using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem))]
public sealed partial class CyberwareCoprocessorComponent : Component
{
    [DataField, AutoNetworkedField]
    public int SafeTolerance;

    [DataField, AutoNetworkedField]
    public int DangerTolerance;

    [DataField, AutoNetworkedField]
    public int MaximumTolerance;
}
