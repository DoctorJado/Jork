using Content.Shared._DEN.Cyberware.Systems;
using Content.Shared._Shitmed.Medical.Surgery.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(CyberwareSystem))]
public sealed partial class SimpleCyberwareComponent : Component
{
    /// <summary>
    ///     While the cybernetic is active, add these components to itself
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? AddSelf;

    /// <summary>
    ///     While the cybernetic is active, add these components to its parent (usually the body)
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? AddParent;

    /// <summary>
    ///     While the cybernetic is active, remove these components from itself
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? RemoveSelf;

    /// <summary>
    ///     While the cybernetic is active, remove these components from its parent (usually the body)
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? RemoveParent;
}
