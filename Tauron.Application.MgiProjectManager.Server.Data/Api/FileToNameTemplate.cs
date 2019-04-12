namespace Tauron.Application.MgiProjectManager.Server.Data.Api
{
    public class FileToNameTemplate
    {
        private readonly string _baseId;

        public string TemplateHtml { get; set; }
        public string SubmitButtonContent { get; }

        public string BaseRowId { get; set; }

        public string ErrorCol { get; set; }

        public string ErrorColSpan { get; set; }

        public string FileNameSpan { get; set; }

        public string EditorId { get; set; }

        public string SubmitButton { get; set; }

        public FileToNameTemplate(string baseId, string templateHtml, string submitButtonContent)
        {
            TemplateHtml = templateHtml;
            SubmitButtonContent = submitButtonContent;
            _baseId = baseId;
            BaseRowId = Template.FileToNameTemplate.BaseRowId + baseId;
            ErrorCol = Template.FileToNameTemplate.ErrorCol + baseId;
            ErrorColSpan = Template.FileToNameTemplate.ErrorColSpan + baseId;
            FileNameSpan = Template.FileToNameTemplate.FileNameSpan + baseId;
            EditorId = Template.FileToNameTemplate.EditorId + baseId;
            SubmitButton = Template.FileToNameTemplate.SubmitButton + baseId;
        }

        public FileToNameTemplate()
        {
            
        }
    }
}