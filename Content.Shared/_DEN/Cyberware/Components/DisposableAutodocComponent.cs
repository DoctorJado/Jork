using Content.Shared._Shitmed.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;


namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class DisposableAutodocComponent : Component
{
    /// <summary>
    /// The ID of the surgery to perform.
    /// </summary>
    [DataField(required: true)]
    public List<DisposableAutodocStep> SurgerySteps;
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
