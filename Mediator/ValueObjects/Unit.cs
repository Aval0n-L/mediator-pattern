using System;

namespace Mediator.ValueObjects;

/// <summary>
/// Custom void type, because <see cref="Void"/> is not a valid return type in C#
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit _value = new();
    public static ref readonly Unit Value => ref _value;
    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";
    public static bool operator ==(Unit left, Unit right) => true;
    public static bool operator !=(Unit left, Unit right) => false;
}
