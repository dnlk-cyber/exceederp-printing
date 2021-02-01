using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
   public class TimeSheetService : ITimeSheetService
    {
       
        private readonly UnitOfWork _uow;
        public TimeSheetService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(TimeSheet entity)
        {
            _uow.TimeSheetRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<TimeSheet> entityList)
        {
            _uow.TimeSheetRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(TimeSheet entity)
        {
            _uow.TimeSheetRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(TimeSheet entity)
        {
            _uow.TimeSheetRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public TimeSheet GetById(Guid id)
        {
            TimeSheet TimeSheet = _uow.TimeSheetRepository.FindById(id);
            return TimeSheet;
        }

        public List<TimeSheet> GetAll()
        {
            List<TimeSheet> TimeSheet = _uow.TimeSheetRepository.GetAll();
            return TimeSheet;

        }
    }
}
  