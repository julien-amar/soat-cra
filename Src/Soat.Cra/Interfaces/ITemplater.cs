using System.Collections.Generic;

namespace Soat.Cra.Interfaces
{
    public interface ITemplater
    {
        string Template(Dictionary<string, string> models);
    }
}