using SQLitePCL.Test.VS.WP81.SL.Resources;

namespace SQLitePCL.Test.VS.WP81.SL
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}