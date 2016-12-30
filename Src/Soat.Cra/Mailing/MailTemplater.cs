using Soat.Cra.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Soat.Cra.Mailing
{
    public class SimpleTemplater : ITemplater
    {
        private readonly string _templatePath;

        public SimpleTemplater()
        {
            _templatePath = ConfigurationManager.AppSettings["Mailing.Template"];
        }

        public string Template(Dictionary<string, string> models)
        {
            var content = File.ReadAllText(_templatePath);

            foreach (var model in models)
            {
                content = content.Replace("{{" + model.Key + "}}", model.Value);
            }

            return content;
        }
    }
}
