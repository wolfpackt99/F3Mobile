using F3.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F3.Business.Service
{
    public interface ISheetService
    {
        Task<List<WorkoutViewModel>> GetWorkouts();
        Task<IEnumerable<SocialCalendarViewModel>> GetSocialCalendars();
    }
}
