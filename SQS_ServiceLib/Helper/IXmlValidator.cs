namespace SQS_ServiceLib.Helper
{
    public interface IXmlValidator
    {
        IEnumerable<string> ValidateXml(string xmlFilePath, string xsdFilePath, string? schema = null);

        IEnumerable<string> ValidateXml(Stream xmlStream, string xsdFilePath, string? schema = null);
    }
}
