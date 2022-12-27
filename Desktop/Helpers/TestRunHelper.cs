using NUnit.Framework;

namespace Helpers
{
    public static class TestRunHelper
    {
        public static string AppPackagePath => $"{TestContext.Parameters["msixPath"]}\\{AppPackage}";
        public static string AppPackage => $"{ConfigHelper.AppId}.msix";
    }
}