using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class InsuranceService : IInsuranceService
    {

        private readonly UnitOfWork _uow;
        public InsuranceService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(FleetInsurance entity)
        {
            _uow.InsuranceRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<FleetInsurance> entityList)
        {
            _uow.InsuranceRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(FleetInsurance entity)
        {
            _uow.InsuranceRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(FleetInsurance entity)
        {
            _uow.InsuranceRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public FleetInsurance GetById(Guid id)
        {
            FleetInsurance Insurance = _uow.InsuranceRepository.FindById(id);
            return Insurance;
        }

        public List<FleetInsurance> GetAll()
        {
            List<FleetInsurance> Insurance = _uow.InsuranceRepository.GetAll();
            return Insurance;

        }
    }
}