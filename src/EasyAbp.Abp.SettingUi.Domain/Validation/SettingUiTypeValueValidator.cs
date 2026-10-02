using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Abp.SettingUi.Validation
{
    /// <summary>
    /// Checks that a value fits the SettingUi type of its setting, so a <c>number</c>, <c>date</c> or
    /// <c>dateTime</c> setting cannot be saved with a value its consumers fail to parse. A <c>null</c> value (a reset)
    /// always passes. Registered by default; override its methods to change what a type accepts.
    /// </summary>
    [ExposeServices(typeof(ISettingUiValueValidator), typeof(SettingUiTypeValueValidator))]
    public class SettingUiTypeValueValidator : ISettingUiValueValidator, ITransientDependency
    {
        protected IStringLocalizer<SettingUiResource> L { get; }

        public SettingUiTypeValueValidator(IStringLocalizer<SettingUiResource> localizer)
        {
            L = localizer;
        }

        public virtual Task ValidateAsync(SettingUiValueValidationContext context)
        {
            if (context.Value == null)
            {
                return Task.CompletedTask;
            }

            string errorMessage = null;
            if (IsType(context, SettingUiConst.Components.Number))
            {
                if (!IsValidNumber(context.Value))
                {
                    errorMessage = L["Validation:Number", context.DisplayName];
                }
            }
            else if (IsType(context, SettingUiConst.Components.Date))
            {
                if (!IsValidDateTime(context.Value))
                {
                    errorMessage = L["Validation:Date", context.DisplayName];
                }
            }
            else if (IsType(context, SettingUiConst.Components.DateTime))
            {
                if (!IsValidDateTime(context.Value))
                {
                    errorMessage = L["Validation:DateTime", context.DisplayName];
                }
            }

            if (errorMessage != null)
            {
                context.Errors.Add(new ValidationResult(errorMessage, new[] { context.SettingDefinition.Name }));
            }

            return Task.CompletedTask;
        }

        protected virtual bool IsType(SettingUiValueValidationContext context, string type)
        {
            return string.Equals(context.Type, type, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// A whole number in the invariant culture: the <c>number</c> input has no <c>step</c>, so the browser accepts
        /// only whole numbers, and settings read as numbers are almost always read with <c>GetAsync&lt;int&gt;</c>.
        /// </summary>
        protected virtual bool IsValidNumber(string value)
        {
            return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// A date, or a date and time, in the invariant culture (the ISO 8601 format the page stores) or in the
        /// current culture (the format <c>SetSettingValuesAsync</c> also accepts for a <c>dateTime</c> value).
        /// </summary>
        protected virtual bool IsValidDateTime(string value)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _) ||
                   DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out _);
        }
    }
}
