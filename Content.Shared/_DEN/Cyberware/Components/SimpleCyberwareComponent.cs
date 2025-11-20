using Content.Shared._DEN.Cyberware.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(CyberwareSystem))]
public sealed partial class SimpleCyberwareComponent : Component
{
    [DataField, AutoNetworkedField]
    public ComponentRegistry? AddSelf;

    [DataField, AutoNetworkedField]
    public ComponentRegistry? AddParent;

    [DataField, AutoNetworkedField]
    public ComponentRegistry? RemoveSelf;

    [DataField, AutoNetworkedField]
    public ComponentRegistry? RemoveParent;
}
