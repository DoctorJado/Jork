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

    /// <summary>
    /// NOTE: is an indicator, not a control variable
    /// </summary>
    [DataField]
    public bool IsEnabled = false;
}
