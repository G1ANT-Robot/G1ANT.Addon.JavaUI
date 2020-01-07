using G1ANT.Language;

namespace G1ANT.Addon.JavaUI.Services
{
    public class SettingsService : ISettingsService
    {
        public string GetUserDocsAddonFolder() => AbstractSettingsContainer.Instance.UserDocsAddonFolder.FullName;
    }
}
