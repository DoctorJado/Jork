using Content.Shared._DEN.Cyberware.Systems;
using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem))]
public sealed partial class CyberwareSymmetryComponent : Component
{
    /// <summary>
    ///     The ID of the sister cyberware required for the entity to function
    /// </summary>
    [DataField, AutoNetworkedField]
    public string SisterID;

    /// <summary>
    ///     The ID to be compared to the sisterID on the other cybernetic
    /// </summary>
    [DataField, AutoNetworkedField]
    public string SelfID;
}
