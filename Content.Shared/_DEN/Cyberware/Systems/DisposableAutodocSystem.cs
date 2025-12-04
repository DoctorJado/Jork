using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Autodoc.Systems;
using Content.Shared._Shitmed.Medical.Surgery;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.Interaction.Events;
using Content.Shared.Prying.Components;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class DisposableAutodocSystem : EntitySystem
{
    [Dependency] private readonly SharedAutodocSystem _autodocSystem = default!;
    [Dependency] private readonly SharedSurgerySystem _surgerySystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DisposableAutodocComponent, GetVerbsEvent<ActivationVerb>>(OnAutodocActivateVerb);
        SubscribeLocalEvent<DisposableAutodocComponent, UseInHandEvent>(OnUseInHand);
    }

    private void OnAutodocActivateVerb(EntityUid uid, DisposableAutodocComponent component, GetVerbsEvent<ActivationVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        args.Verbs.Add(new ActivationVerb()
        {
            Text = "Activate Autodoc",
            Impact = LogImpact.Low,
            Act = () => TryActivateAutodoc(new Entity<DisposableAutodocComponent>(uid, component), args.User),
        });
    }

    private void OnUseInHand(Entity<DisposableAutodocComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        TryActivateAutodoc(ent, args.User);
    }
    private void TryActivateAutodoc(Entity<DisposableAutodocComponent> self, EntityUid user)
    {
        foreach (var step in self.Comp.SurgerySteps)
        {
            if (_autodocSystem.FindPart(user, step.Part, step.Symmetry) is not {} part)
                continue;
            StartSurgery(self, user, part, step.Surgery);
        }
    }

    private void StartSurgery(
        Entity<DisposableAutodocComponent> self,
        EntityUid patient,
        EntityUid part,
        EntProtoId surgery
    )
    {
        if (_surgerySystem.GetSingleton(surgery) is not {} surgeryStep)
            return;

        if (_surgerySystem.GetNextStep(patient, part, surgeryStep) is not {} pair)
            return;

        var nextSurgery = pair.Item1;
        var nextStep = nextSurgery.Comp.Steps[pair.Item2];

        if (!_surgerySystem.TryDoSurgeryStep(patient, part, self, MetaData(nextSurgery).EntityPrototype!.ID, nextStep))
            return;
    }
}
