namespace Tauron.MgiProjectManager
{
    public class FilsConfig
    {
        public string NameExpression { get; set; }

        public string CaseRange { get; set; }
    }

    public class AppSettings
    {
        public SmtpConfig SmtpConfig { get; set; }

        public FilsConfig FilsConfig { get; set; }

    }
}