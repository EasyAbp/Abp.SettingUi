﻿namespace EasyAbp.Abp.SettingUi
{
    public class SettingUiConst
    {
        public const string Group1 = "Group1";
        public const string Group2 = "Group2";
        public const string Type = "Type";
        public const string Options = "Options";
        public const string DefaultGroup = "Others";
        public const string DefaultType = Components.Text;

        public const string SettingPropertiesFileFolder = "/SettingProperties";
        public const string FormNamePrefix = "Setting_";
        public const char OptionsSeparator = '|';

        /// <summary>
        /// Will use these types to determine which component to render for a setting.
        /// <code>Pages\Components\SettingUi\Partials\_ComponentsType.cshtml</code>
        /// </summary>
        public class Components
        {
            public const string Text = "text";
            public const string Number = "number";
            public const string Checkbox = "checkbox";
            public const string Date = "date";
            public const string DateTime = "dateTime";
            public const string Select = "select";
        }
    }
}