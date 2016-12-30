using System;
using System.Collections.Generic;

namespace Soat.Cra.Model
{
	public class CRA
	{
        public int IdCol { get; set; }

        public Dictionary<int, Mission> Missions { get; set; }

        public DateTime Month { get; set; }
	}
}