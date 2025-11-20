using Content.Shared.Damage;

namespace Content.Server._DEN.Cybernetics.Components;

[RegisterComponent]
public sealed partial class GorillaArmsComponent : Component
{
    [DataField("modifiers", required: true)]
    public DamageModifierSet UnarmedModifiers = default!;
}
