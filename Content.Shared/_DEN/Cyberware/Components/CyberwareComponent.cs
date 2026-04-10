using Content.Shared._DEN.Cyberware.EntitySystems;
using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem))]
public sealed partial class CyberwareComponent : Component
{
    /// <summary>
    /// The cyberware score of the entity- how impactful it is towards your cyberware limit
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ComplexityScore = 1;

    [DataField, AutoNetworkedField]
    public EntityUid? RootEntity;
}
