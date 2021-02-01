using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class EducationallevelService : IEducationallevelService
    {


        
  private readonly UnitOfWork _uow;
        public EducationallevelService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Educationallevel entity)
        {
            _uow.EducationallevelRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Educationallevel> entityList)
        {
            _uow.EducationallevelRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool AddOrUpdate(Educationallevel entity)
        {
            _uow.EducationallevelRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Educationallevel entity)
        {
            _uow.EducationallevelRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Educationallevel GetById(Guid id)
        {
         Educationallevel Educationallevel =   _uow.EducationallevelRepository.FindById(id);
         return Educationallevel;
        }

        public List<Educationallevel> GetAll()
        {
         List<Educationallevel> Educationallevel=   _uow.EducationallevelRepository.GetAll();
         return Educationallevel;
          
        }


        public bool Update(Educationallevel entity)
        {
            throw new NotImplementedException();
        }
    }
}
