using System.Threading.Tasks;

namespace EasyAbp.Abp.SettingUi.Validation
{
    /// <summary>
    /// Validates a setting value before the setting page saves it.
    /// <para>
    /// Register implementations in the dependency injection container (for example with
    /// <c>ITransientDependency</c> and <c>[ExposeServices(typeof(ISettingUiValueValidator))]</c>). Every registered
    /// validator is called for every value <c>ISettingUiAppService.SetSettingValuesAsync</c> is about to write, so a
    /// validator should return without adding errors for settings it does not handle.
    /// </para>
    /// <para>
    /// Add a <see cref="System.ComponentModel.DataAnnotations.ValidationResult"/> with a localized message to
    /// <see cref="SettingUiValueValidationContext.Errors"/> for each problem. After all values of the request are
    /// validated, the errors of all settings are thrown together as one <c>AbpValidationException</c> and nothing is
    /// saved; the page shows the messages to the user. A validator can also throw a <c>UserFriendlyException</c> or
    /// <c>BusinessException</c> itself, which likewise aborts the save before anything is written.
    /// </para>
    /// <para>
    /// Resetting settings (<c>ISettingUiAppService.ResetSettingValuesAsync</c>) is not validated: it restores the
    /// default value of the setting definition, or the value inherited from a lower setting provider.
    /// </para>
    /// </summary>
    public interface ISettingUiValueValidator
    {
        Task ValidateAsync(SettingUiValueValidationContext context);
    }
}
