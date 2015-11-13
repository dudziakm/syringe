using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories
{
	public interface IScheduledJobRepository
	{
		Task AddJobAsync(ScheduledJob job);
		Task UpdateJobAsync(ScheduledJob job);
		Task DeleteJobAsync(ScheduledJob job);
		void Wipe();

		IEnumerable<ScheduledJob> GetAll();
		IEnumerable<ScheduledJob> GetForTeam(Team team);
	}
}
