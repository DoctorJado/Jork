using Robust.Shared.GameStates;

namespace Content.Shared._DEN.Cyberware.Components;


/// <summary>
/// Component for cyberware if the cyberware can be completely disabled by EMPs
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CyberwareCanEmpComponent : Component
{
    /// <summary>
    ///     Is the cyberware currently disabled by an EMP?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Disabled = false;
}
