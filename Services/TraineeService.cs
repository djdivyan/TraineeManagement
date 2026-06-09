using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    class TraineeService : ITraineeService
    {
        private static readonly List<Trainee> Trainees = [];
        private static int index = 0;


         public IEnumerable<Trainee> GetAll()
        {
            return Trainees;
        }

        public TraineeResponse? GetById(int id)
        {
            var trainee = Trainees.FirstOrDefault(t => t.Id == id);
            if (trainee is null)
            {
                return null;
            }
            return MapToResponse(trainee);
        }

        public static TraineeResponse? MapToResponse(Trainee trainee)
        {
            return new TraineeResponse
            {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email, 
            TechStack = trainee.TechStack, 
            Status = trainee.Status,
            CreatedDate = trainee.CreatedDate, 
            UpdatedDate = trainee.UpdatedDate
            };
        }



        public bool Delete(int id)
        {
            var trainee = Trainees.FirstOrDefault(t => t.Id == id);;
            if(trainee is null)
                return false;

            Trainees.Remove(trainee);

            return true;
        }

       
        public TraineeResponse Create(CreateTraineeRequest createTraineeRequest)
        {
            var trainee = new Trainee
            {
            Id = index++,
            FirstName = createTraineeRequest.FirstName,
            LastName = createTraineeRequest.LastName,
            Email = createTraineeRequest.Email, 
            TechStack = createTraineeRequest.TechStack, 
            Status = createTraineeRequest.Status,
            CreatedDate = DateTime.Now, 
            UpdatedDate = DateTime.Now
            };

            Trainees.Add(trainee);
            return MapToResponse(trainee);
        }

        public TraineeResponse? Update(int id, UpdateTraineeRequest updateTraineeRequest)
        {
            var trainee = Trainees.FirstOrDefault(t => t.Id == updateTraineeRequest.Id );
            if(trainee is null) return null;
            trainee.FirstName = updateTraineeRequest.FirstName;
            trainee.LastName = updateTraineeRequest.LastName;
            trainee.Email = updateTraineeRequest.Email;
            trainee.TechStack = updateTraineeRequest.TechStack; 
            trainee.Status = updateTraineeRequest.Status;
            trainee.UpdatedDate = DateTime.Now;

            return MapToResponse(trainee);
        }
    }
}