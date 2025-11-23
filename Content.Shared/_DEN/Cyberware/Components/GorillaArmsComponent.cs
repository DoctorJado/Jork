using Content.Shared.Damage;

namespace Content.Shared._DEN.Cyberware.Components;

[RegisterComponent]
public sealed partial class GorillaArmsComponent : Component
{
    [DataField("modifiers", required: true)]
    public DamageModifierSet UnarmedModifiers = default!;
}
