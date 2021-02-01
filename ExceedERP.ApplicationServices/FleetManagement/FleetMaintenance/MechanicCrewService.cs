using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public class MechanicCrewService :IMechanicCrewService
    {


          
       private readonly UnitOfWork _uow;
        public MechanicCrewService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(MechanicCrew entity)
        {
            _uow.MechanicCrewRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<MechanicCrew> entityList)
        {
            _uow.MechanicCrewRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(MechanicCrew entity)
        {
            _uow.MechanicCrewRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(MechanicCrew entity)
        {
            _uow.MechanicCrewRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public MechanicCrew GetById(Guid id)
        {
         MechanicCrew MechanicCrew =   _uow.MechanicCrewRepository.FindById(id);
         return MechanicCrew;
        }

        public List<MechanicCrew> GetAll()
        {
         List<MechanicCrew> MechanicCrew=   _uow.MechanicCrewRepository.GetAll();
         return MechanicCrew;
          
        }
    }
}
