using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CyberwareAddActionComponent : Component
{
    /// <summary>
    /// Used where you want the cyberware to grant the owner an instant action.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public EntProtoId? CyberwareAction;

    [DataField, AutoNetworkedField]
    public EntityUid? Action;

    /// <summary>
    /// The entity this cyberware is inside
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public EntityUid? TargetEntity;

    /// <summary>
    /// The popup text that appears when the cyberware is toggled on
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? CyberwareEnabledPopupText;

    /// <summary>
    /// The popup text that appears when the cyberware is toggled off
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? CyberwareDisabledPopupText;

    /// <summary>
    /// Is the cyberware currently applying the Add* fields?
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public bool EnabledState = false;

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

/// <summary>
/// Event for toggling add/remove of the generic fields
/// </summary>
public sealed partial class ActivateActionToggleGenericCyberwareEvent : InstantActionEvent { }
