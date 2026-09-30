using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EasyAbp.Abp.SettingUi.Validation
{
    /// <summary>
    /// Accepts only a port number (1-65535) for <see cref="SettingName"/> and records every value it is given.
    /// </summary>
    public class TestPortSettingUiValueValidator : ISettingUiValueValidator
    {
        public const string SettingName = "Test.Port";

        public List<(string SettingName, string Value)> ValidatedValues { get; } = new();

        public Task ValidateAsync(SettingUiValueValidationContext context)
        {
            ValidatedValues.Add((context.SettingDefinition.Name, context.Value));

            if (context.SettingDefinition.Name != SettingName)
            {
                return Task.CompletedTask;
            }

            if (!int.TryParse(context.Value, out var port) || port < 1 || port > 65535)
            {
                context.Errors.Add(new ValidationResult("The port must be a number between 1 and 65535.",
                    new[] { SettingName }));
            }

            return Task.CompletedTask;
        }
    }
}
