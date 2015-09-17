using System.Collections.Generic;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories
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
