using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Repository
{
	public interface IScheduledJobRepository
	{
		void AddJob(ScheduledJob job);
		void UpdateJob(ScheduledJob job);
		void DeleteJob(ScheduledJob job);

		IEnumerable<ScheduledJob> GetAll();
		IEnumerable<ScheduledJob> GetForTeam(Team team);
	}
}
