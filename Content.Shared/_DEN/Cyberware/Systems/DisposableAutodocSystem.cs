using Content.Shared._DEN.Cyberware.Components;
using Content.Shared._Shitmed.Autodoc.Systems;
using Content.Shared._Shitmed.Medical.Surgery;
using Content.Shared._Shitmed.Medical.Surgery.Steps;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.Interaction.Events;
using Content.Shared.Prying.Components;
using Content.Shared.Verbs;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;


namespace Content.Shared._DEN.Cyberware.Systems;


public sealed class DisposableAutodocSystem : EntitySystem
{
    [Dependency] private readonly SharedAutodocSystem _autodocSystem = default!;
    [Dependency] private readonly SharedSurgerySystem _surgerySystem = default!;
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DisposableAutodocComponent, GetVerbsEvent<ActivationVerb>>(OnAutodocActivateVerb);
        SubscribeLocalEvent<DisposableAutodocComponent, UseInHandEvent>(OnUseInHand);

        SubscribeLocalEvent<DisposableAutodocComponent, SurgeryStepEvent>(OnSurgeryStep);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if(_net.IsClient)
            return;

        var query = EntityQueryEnumerator<ActiveDisposableAutodocComponent, DisposableAutodocComponent>();
        var now = Timing.CurTime;

        while (query.MoveNext(out var uid, out var active, out var comp))
        {
            if (now < active.NextUpdate)
                continue;

            active.NextUpdate = now + comp.UpdateDelay;

            if (TryUpdateAutodoc(uid, comp, active))
            {
                RemCompDeferred<ActiveDisposableAutodocComponent>(uid);
                DirtyEntity(uid);
            }
        }
    }

    private void OnAutodocActivateVerb(EntityUid uid, DisposableAutodocComponent component, GetVerbsEvent<ActivationVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        args.Verbs.Add(new ActivationVerb()
        {
            Text = "Activate Autodoc",
            Impact = LogImpact.Low,
            Act = () => ActivateAutodoc(new Entity<DisposableAutodocComponent>(uid, component), args.User),
        });
    }

    private void OnUseInHand(Entity<DisposableAutodocComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        ActivateAutodoc(ent, args.User);
    }

    private bool TryUpdateAutodoc(EntityUid uid, DisposableAutodocComponent component, ActiveDisposableAutodocComponent user)
    {
        Logger.Debug("update autodoc");
        return TryActivateAutodoc(new Entity<DisposableAutodocComponent>(uid, component), user.User);
    }

    private void ActivateAutodoc(Entity<DisposableAutodocComponent> self, EntityUid user)
    {
        Logger.Debug("activate autodoc");
        if(HasComp<ActiveDisposableAutodocComponent>(self))
            return;

        var activeComp = EnsureComp<ActiveDisposableAutodocComponent>(self);
        activeComp.User = user;
        activeComp.NextUpdate = Timing.CurTime + self.Comp.UpdateDelay;
        self.Comp.CurrentStep = 0;

        TryActivateAutodoc(self, user);

        Dirty(self.Owner,  activeComp);
    }

    private bool TryActivateAutodoc(Entity<DisposableAutodocComponent> self, EntityUid user)
    {
        Logger.Debug("tryactivate autodoc");

        if(self.Comp.SurgerySteps.Count == self.Comp.CurrentStep)
            return true;

        if (self.Comp.Waiting)
            return false;

        var step = self.Comp.SurgerySteps[self.Comp.CurrentStep];

        if (_autodocSystem.FindPart(user, step.Part, step.Symmetry) is not {} part)
            return false;

        if (StartSurgery(self, user, part, step.Surgery))
        {
            self.Comp.Waiting = true;
            return false;
        }

        self.Comp.CurrentStep++;
        return false;
    }

    private bool StartSurgery(
        Entity<DisposableAutodocComponent> self,
        EntityUid patient,
        EntityUid part,
        EntProtoId surgery
    )
    {
        Logger.Debug("starting surgery step");
        if (_surgerySystem.GetSingleton(surgery) is not {} surgeryStep)
            return false;

        if (_surgerySystem.GetNextStep(patient, part, surgeryStep) is not {} pair)
            return false;

        var nextSurgery = pair.Item1;
        var nextStep = nextSurgery.Comp.Steps[pair.Item2];

        if (!_surgerySystem.TryDoSurgeryStep(patient, part, self, MetaData(nextSurgery).EntityPrototype!.ID, nextStep))
            return false;

        return true;
    }

    private void OnSurgeryStep(Entity<DisposableAutodocComponent> self, ref SurgeryStepEvent args)
    {
        Logger.Debug("surgery step");

        var repeatable = HasComp<SurgeryRepeatableStepComponent>(args.Step);
        if (args.Complete || !repeatable)
        {
            self.Comp.Waiting = false; // try the next autodoc or surgery step
            return;
        }
    }
}
