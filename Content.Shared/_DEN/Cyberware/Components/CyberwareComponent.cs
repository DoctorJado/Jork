using Content.Shared._DEN.Cyberware.Systems;
using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem))]
public sealed partial class CyberwareComponent : Component
{
    /// <summary>
    ///     The cyberware score of the entity- or how impactful it is towards your cyberware limit
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Complexity = 1;

    [AutoNetworkedField]
    public EntityUid? Parent;
}
