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
        /// The value about to be written, as it will be stored (line endings are normalized to <c>\n</c>, and a
        /// <c>dateTime</c> value is already converted to UTC). <c>null</c> when the box was cleared, which resets the
        /// setting so it inherits its value again.
        /// </summary>
        [CanBeNull]
        public string Value { get; }

        /// <summary>
        /// The setting properties the page uses, merged from the setting definition and the setting property files
        /// (<c>Group1</c>, <c>Group2</c>, <c>Type</c>, <c>Options</c>...). The properties of the setting definition
        /// when the context is created without them.
        /// </summary>
        [NotNull]
        public IReadOnlyDictionary<string, object> Properties { get; }

        /// <summary>
        /// The SettingUi type of the setting (<c>text</c>, <c>number</c>, <c>date</c>...), as the page renders it.
        /// </summary>
        [NotNull]
        public string Type =>
            Properties.TryGetValue(SettingUiConst.Type, out var type) && !string.IsNullOrWhiteSpace(type?.ToString())
                ? type.ToString()
                : SettingUiConst.DefaultType;

        /// <summary>
        /// The localized display name of the setting, to name it in a validation message.
        /// </summary>
        [NotNull]
        public string DisplayName { get; }

        /// <summary>
        /// The validation errors. Use a localized message, and the setting name as the member name.
        /// </summary>
        [NotNull]
        public List<ValidationResult> Errors { get; } = new();

        public SettingUiValueValidationContext([NotNull] SettingDefinition settingDefinition, [CanBeNull] string value)
            : this(settingDefinition, value, null, null)
        {
        }

        public SettingUiValueValidationContext([NotNull] SettingDefinition settingDefinition, [CanBeNull] string value,
            [CanBeNull] IReadOnlyDictionary<string, object> properties, [CanBeNull] string displayName)
        {
            SettingDefinition = Check.NotNull(settingDefinition, nameof(settingDefinition));
            Value = value;
            Properties = properties ?? settingDefinition.Properties;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? settingDefinition.Name : displayName;
        }
    }
}
