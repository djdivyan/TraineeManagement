using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ITraineeService
    {
        IEnumerable<Trainee> GetAll();
        TraineeResponse? GetById(int id);
        
        TraineeResponse Create(CreateTraineeRequest createTraineeRequest);
        TraineeResponse? Update(int id,UpdateTraineeRequest updateTraineeRequest);
        bool Delete(int id);

    } 
}