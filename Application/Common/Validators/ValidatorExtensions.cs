using FluentValidation;
using System;
using System.Linq;

namespace Application.Common.Validators
{
    /// <summary>
    /// ValidatorExtensions.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Applies a set of rules to validate an object instance.
        /// </summary>
        /// <typeparam name="T">The type of <c>T</c></typeparam>
        /// <typeparam name="TProperty">The TProperty.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="validOptions">The valid options.</param>
        /// <returns>The <see cref="IRuleBuilderOptions{T, TProperty}"/>.</returns>
        public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params TProperty[] validOptions)
        {
            string formatted;
            if (validOptions == null || validOptions.Length == 0)
            {
                throw new ArgumentException("At least one valid option is expected", nameof(validOptions));
            }
            else if (validOptions.Length == 1)
            {
                formatted = validOptions[0].ToString();
            }
            else
            {
                formatted = $"{string.Join(", ", validOptions.Select(vo => vo.ToString()).ToArray(), 0, validOptions.Length - 1)} or {validOptions.Last()}";
            }

            return ruleBuilder
                .Must(validOptions.Contains).WithMessage($"{{PropertyName}} must be: {formatted}");
        }

        /// <summary>
        /// Validates password.
        /// </summary>
        /// <typeparam name="T">The type of T</typeparam>
        /// <param name="ruleBuilder">The rule builder</param>
        /// <returns>The <see cref="IRuleBuilderOptions{T, string}"/>.</returns>
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches("[A-Z]").WithMessage("Password must contain 1 uppercase character")
                .Matches("[a-z]").WithMessage("Password must contain 1 lowecase character")
                .Matches("[0-9]").WithMessage("Password must contain 1 number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain none alphanumeric");

            return options;
        }
    }
}
