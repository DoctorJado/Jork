using Content.Shared._DEN.Cyberware.Components;


namespace Content.Shared._DEN.Cyberware;

/// <summary>
/// Raised on the player whenever a piece of cyberware is installed with a complexity score
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareComplexityTotalChange(Entity<CyberwareManagerComponent> Cyberware);

/// <summary>
/// Raised on the player and cyberware when it is installed
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareInstalledEvent(Entity<CyberwareComponent> Cyberware);

/// <summary>
/// Raised on the player and cyberware when it is installed
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareRemovedEvent(Entity<CyberwareComponent> Cyberware);

/// <summary>
/// Raised on a cyberware (bodypart or organ) if CyberwareAttemptEnabledEvent() is not cancelled, to enable a cyberware
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareEnabledEvent();

/// <summary>
/// Raised on a cyberware (bodypart or organ) to enable a cyberware
/// </summary>
[ByRefEvent]
public sealed class CyberwareAttemptEnabledEvent() : CancellableEntityEventArgs;

/// <summary>
/// Raised on a cyberware (bodypart or organ) if CyberwareAttemptDisabledEvent() is not cancelled, to disable a cyberware
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareDisabledEvent();

/// <summary>
/// Raised on a cyberware (bodypart or organ) to disable a cyberware
/// </summary>
[ByRefEvent]
public sealed class CyberwareAttemptDisabledEvent() : CancellableEntityEventArgs;

/// <summary>
/// Raised on the player when they have a coprocessor installed through shitmed
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareCoprocessorInstalledEvent(Entity<CyberwareCoprocessorComponent> Cyberware);

/// <summary>
/// Raised on the player when they have a coprocessor removed through shitmed
/// </summary>
[ByRefEvent]
public readonly record struct CyberwareCoprocessorRemovedEvent(Entity<CyberwareCoprocessorComponent> Cyberware);
