using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class CyberwareStaticComponent : Component
{
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? AddSelf;

    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? AddBody;
}
