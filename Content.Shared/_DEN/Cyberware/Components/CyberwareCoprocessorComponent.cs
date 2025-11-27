using Content.Shared._DEN.Cyberware.Systems;
using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem), typeof(CyberwareCoprocessorSystem))]
public sealed partial class CyberwareCoprocessorComponent : Component
{
    /// <summary>
    ///     The tolerance before negative effects start occuring
    /// </summary>
    [DataField, AutoNetworkedField]
    public int SafeTolerance;

    /// <summary>
    ///     The tolerance before the bad shit starts happening
    /// </summary>
    [DataField, AutoNetworkedField]
    public int DangerTolerance;

    /// <summary>
    ///     The tolerance when the really bad shit happens
    /// </summary>
    [DataField, AutoNetworkedField]
    public int MaximumTolerance;
}
