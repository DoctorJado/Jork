using Robust.Shared.GameStates;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CyberwareManagerComponent : Component
{
    [DataField, AutoNetworkedField]
    public int CurrentComplexity = 0;

    [DataField, AutoNetworkedField]
    public EntityUid? CoprocessorUid;

    [DataField, AutoNetworkedField]
    public List<EntityUid> InstalledCyberware = new();
}
