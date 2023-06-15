using System.Xml;
using System.Xml.Schema;

namespace SQS_ServiceLib.Helper
{
    public class XmlValidator : IXmlValidator
    {
        #region XML Validation with repect to XSD

        /// <summary>
        /// Validates XML against XSD file
        /// </summary>
        /// <param name="xmlFilePath">XML file path</param>
        /// <param name="xsdFilePath">XSD file path</param>
        /// <param name="schema">Namspace / scema for the XML</param>
        /// <returns>Collection of error result</returns>
        public IEnumerable<string> ValidateXml(string xmlFilePath, string xsdFilePath, string? schema = null)
        {
            var errors = new List<string>();
            try
            {
                var settings = GetXsdToXmlSettings(xsdFilePath, schema);
                ReadXml(xmlFilePath, settings.Settings);
                errors.AddRange(settings.Errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error in validating XML: {ex.Message}");
            }

            return errors;
        }

        /// <summary>
        /// Validates XML against XSD file
        /// </summary>
        /// <param name="xmlStream">XML file stream</param>
        /// <param name="xsdFilePath">XSD file path</param>
        /// <param name="schema">Namspace / scema for the XML</param>
        /// <returns>Collection of error result</returns>
        public IEnumerable<string> ValidateXml(Stream xmlStream, string xsdFilePath, string? schema = null)
        {
            var errors = new List<string>();
            try
            {
                var settings = GetXsdToXmlSettings(xsdFilePath, schema);
                ReadXml(xmlStream, settings.Settings);
                errors.AddRange(settings.Errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error in validating XML: {ex.Message}");
            }

            return errors;
        }

        private dynamic GetXsdToXmlSettings(string xsdFilePath, string? schema = null)
        {
            var errors = new List<string>();
            XmlReaderSettings settings = new();
            settings.Schemas.Add(schema, xsdFilePath);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += (object? sender, ValidationEventArgs e) =>
            {
                if (e?.Severity == XmlSeverityType.Error)
                {
                    errors.Add(e.Message);
                }
            };
            return new { Settings = settings, Errors = errors };
        }

        private void ReadXml(string xmlFilePath, XmlReaderSettings settings)
        {
            using var reder = XmlReader.Create(xmlFilePath, settings);
            while (reder.Read()) { }
        }

        private void ReadXml(Stream xmlStream, XmlReaderSettings settings)
        {
            using var reder = XmlReader.Create(xmlStream, settings);
            while (reder.Read()) { }
        }

        #endregion

    }
}
