using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Common.ValueObjects
{
   public sealed class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email address cannot be empty.", nameof(email));

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email address format.", nameof(email));

        return new EmailAddress(email.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) =>
        obj is EmailAddress other && Equals(other);

    public bool Equals(EmailAddress? other) =>
        other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(EmailAddress email) => email.Value;

    public static bool operator ==(EmailAddress? left, EmailAddress? right) =>
        Equals(left, right);

    public static bool operator !=(EmailAddress? left, EmailAddress? right) =>
        !Equals(left, right);
}
}
