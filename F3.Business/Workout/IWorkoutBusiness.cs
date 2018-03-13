using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutModel = F3.ViewModels.WorkoutViewModel;
namespace F3.Business.Workout
{
    public interface IWorkoutBusiness
    {
        Task<List<WorkoutModel>> GetMasterList();
    }
}
