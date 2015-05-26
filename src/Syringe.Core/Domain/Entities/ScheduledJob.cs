using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Domain.Entities
{
	public class ScheduledJob
	{
		public Guid Id { get; set; }

		public string Crontab { get; set; }
		public Guid TeamId { get; set; }
		public string Description { get; set; }

		/// <summary>
		/// Should be translated into {root}/teamname/filename
		/// </summary>
		public string TestCaseFilename { get; set; }
	}
}
