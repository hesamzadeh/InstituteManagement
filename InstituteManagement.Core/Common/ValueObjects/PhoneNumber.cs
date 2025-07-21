using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InstituteManagement.Core.Common.ValueObjects
{
    public sealed class PhoneNumber : IEquatable<PhoneNumber>
    {
        private static readonly Regex PhoneRegex = new(
            @"^\+?[0-9\s\-]{6,20}$", // Allows international numbers
            RegexOptions.Compiled
        );

        public string Number { get; }
        public string? Label { get; }

        private PhoneNumber(string number, string? label = null)
        {
            Number = number;
            Label = label;
        }

        public static PhoneNumber Create(string number, string? label = null)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Phone number cannot be empty.", nameof(number));

            number = number.Trim();

            if (!PhoneRegex.IsMatch(number))
                throw new ArgumentException("Invalid phone number format.", nameof(number));

            if (label?.Length > 50)
                throw new ArgumentException("Label is too long (max 50 characters).", nameof(label));

            return new PhoneNumber(number, label?.Trim());
        }

        public override string ToString() => $"{Label}: {Number}";

        public override bool Equals(object? obj) =>
            obj is PhoneNumber other && Equals(other);

        public bool Equals(PhoneNumber? other) =>
            other is not null && Number == other.Number && Label == other.Label;

        public override int GetHashCode() =>
            HashCode.Combine(Number, Label);

        public static bool operator ==(PhoneNumber? left, PhoneNumber? right) =>
            Equals(left, right);

        public static bool operator !=(PhoneNumber? left, PhoneNumber? right) =>
            !Equals(left, right);
    }
}
