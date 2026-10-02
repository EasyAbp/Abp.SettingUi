using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace EasyAbp.Abp.SettingUi.Settings
{
    /// <summary>
    /// The AbpSettings table in memory, for tests that use the real <see cref="ISettingManager"/> and
    /// <see cref="ISettingProvider"/>. <see cref="Writes"/> records every write, also one that changes nothing.
    /// </summary>
    public class InMemorySettingManagementStore : ISettingManagementStore
    {
        public List<SettingRow> Rows { get; } = new();

        public List<string> Writes { get; } = new();

        public Task<string> GetOrNullAsync(string name, string providerName, string providerKey)
        {
            return Task.FromResult(Find(name, providerName, providerKey)?.Value);
        }

        public Task<List<SettingValue>> GetListAsync(string providerName, string providerKey)
        {
            return Task.FromResult(Rows
                .Where(row => row.ProviderName == providerName && row.ProviderKey == providerKey)
                .Select(row => new SettingValue(row.Name, row.Value))
                .ToList());
        }

        public Task<List<SettingValue>> GetListAsync(string[] names, string providerName, string providerKey)
        {
            return Task.FromResult(names
                .Select(name => new SettingValue(name, Find(name, providerName, providerKey)?.Value))
                .ToList());
        }

        public Task SetAsync(string name, string value, string providerName, string providerKey)
        {
            Writes.Add($"Set {name} {providerName} {providerKey}");

            var row = Find(name, providerName, providerKey);
            if (row == null)
            {
                Rows.Add(new SettingRow(name, value, providerName, providerKey));
            }
            else
            {
                row.Value = value;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(string name, string providerName, string providerKey)
        {
            Writes.Add($"Delete {name} {providerName} {providerKey}");

            Rows.RemoveAll(row => row.Name == name && row.ProviderName == providerName && row.ProviderKey == providerKey);

            return Task.CompletedTask;
        }

        public SettingRow Find(string name, string providerName, string providerKey)
        {
            return Rows.FirstOrDefault(row =>
                row.Name == name && row.ProviderName == providerName && row.ProviderKey == providerKey);
        }

        public class SettingRow
        {
            public string Name { get; }
            public string Value { get; set; }
            public string ProviderName { get; }
            public string ProviderKey { get; }

            public SettingRow(string name, string value, string providerName, string providerKey)
            {
                Name = name;
                Value = value;
                ProviderName = providerName;
                ProviderKey = providerKey;
            }

            public override string ToString()
            {
                return $"{Name} {ProviderName} {ProviderKey} = {Value}";
            }
        }
    }
}
