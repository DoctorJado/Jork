using Content.Shared._Shitmed.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DisposableAutodocComponent : Component
{
    /// <summary>
    /// The ID of the surgery to perform.
    /// </summary>
    [DataField(required: true)]
    public List<DisposableAutodocStep> SurgerySteps;

    [DataField, AutoNetworkedField]
    public bool Waiting = false;

    [DataField, AutoNetworkedField]
    public int CurrentStep = 0;

    /// <summary>
    /// How long to wait between each update check.
    /// </summary>
    [DataField]
    public TimeSpan UpdateDelay = TimeSpan.FromSeconds(0.5);
}

[DataRecord]
public sealed partial class DisposableAutodocStep
{
    [DataField(required: true)]
    public EntProtoId<SurgeryComponent> Surgery;

    [DataField(required: true)]
    public BodyPartType Part;

    [DataField]
    public BodyPartSymmetry? Symmetry;
}
