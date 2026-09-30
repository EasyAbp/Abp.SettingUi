using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Validation
{
    /// <summary>
    /// The setting value an <see cref="ISettingUiValueValidator"/> validates, and the errors it reports.
    /// </summary>
    public class SettingUiValueValidationContext
    {
        /// <summary>
        /// The definition of the setting being saved.
        /// </summary>
        [NotNull]
        public SettingDefinition SettingDefinition { get; }

        /// <summary>
        /// The value about to be written, as it will be stored (a <c>dateTime</c> value is already converted to UTC).
        /// Can be empty, or <c>null</c> when the client posts null, which clears the value like a reset.
        /// </summary>
        [CanBeNull]
        public string Value { get; }

        /// <summary>
        /// The validation errors. Use a localized message, and the setting name as the member name.
        /// </summary>
        [NotNull]
        public List<ValidationResult> Errors { get; } = new();

        public SettingUiValueValidationContext([NotNull] SettingDefinition settingDefinition, [CanBeNull] string value)
        {
            SettingDefinition = Check.NotNull(settingDefinition, nameof(settingDefinition));
            Value = value;
        }
    }
}
