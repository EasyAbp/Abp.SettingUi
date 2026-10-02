using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Settings
{
    /// <summary>
    /// The settings of <see cref="SettingUi.SettingUiSaveSemantics_Tests"/>, one card of each kind. The other test
    /// classes replace the setting definition manager, so they do not see these.
    /// </summary>
    public class SaveSemanticsSettingDefinitionProvider : SettingDefinitionProvider
    {
        public const string Group1 = "SaveSemantics";
        public const string Group2 = "Card";

        public const string Text = "Test.Save.Text";
        public const string Number = "Test.Save.Number";
        public const string Checkbox = "Test.Save.Checkbox";
        public const string Date = "Test.Save.Date";
        public const string DateTime = "Test.Save.DateTime";
        public const string MultiLine = "Test.Save.MultiLine";
        public const string Secret = "Test.Save.Secret";
        public const string KeyVaultSecret = "Test.Save.KeyVaultSecret";
        // In the default group, which DisableDefaultGroup hides.
        public const string Ungrouped = "Test.Save.Ungrouped";

        public const string MultiLineDefault = "line 1\nline 2\nline 3";

        public override void Define(ISettingDefinitionContext context)
        {
            Add(context, new SettingDefinition(Text, "default text"));
            Add(context, new SettingDefinition(Number, "5", new FixedLocalizableString("Save number")),
                SettingUiConst.Components.Number);
            Add(context, new SettingDefinition(Checkbox, "True"), SettingUiConst.Components.Checkbox);
            Add(context, new SettingDefinition(Date, "2026-01-15", new FixedLocalizableString("Save date")),
                SettingUiConst.Components.Date);
            Add(context, new SettingDefinition(DateTime, "2026-01-15T09:00:00.0000000Z",
                new FixedLocalizableString("Save date time")), SettingUiConst.Components.DateTime);
            // A type a host application adds with its own partial view, like a textarea.
            Add(context, new SettingDefinition(MultiLine, MultiLineDefault), "textArea");
            Add(context, new SettingDefinition(Secret, isEncrypted: true));
            Add(context, new SettingDefinition(KeyVaultSecret, isEncrypted: true));

            context.Add(new SettingDefinition(Ungrouped, "ungrouped", isEncrypted: true));
        }

        private static void Add(ISettingDefinitionContext context, SettingDefinition setting,
            string type = SettingUiConst.DefaultType)
        {
            context.Add(setting
                .WithProperty(SettingUiConst.Group1, Group1)
                .WithProperty(SettingUiConst.Group2, Group2)
                .WithProperty(SettingUiConst.Type, type));
        }
    }
}
