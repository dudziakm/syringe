using System.Collections.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Repositories
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
