namespace EasyAbp.Abp.SettingUi.Options;

public class AbpSettingUiOptions
{
    /// <summary>
    /// If true, the default "Others" setting group (group1) will be unavailable, only grouped items can be used.
    /// </summary>
    public bool DisableDefaultGroup { get; set; }

    /// <summary>
    /// If true:
    /// 1. On the host side, global setting values will replace host-side setting values for management.
    /// 2. If the host setting value is different from the global setting value, it shows and manages the global one.
    /// </summary>
    public bool ManageGlobalSettingsOnHostSide { get; set; }
}