using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
  public class SparePartService :ISparePartService
    {
    

         
       private readonly UnitOfWork _uow;
        public SparePartService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(SparePart entity)
        {
            _uow.SparePartRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<SparePart> entityList)
        {
            _uow.SparePartRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(SparePart entity)
        {
            _uow.SparePartRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(SparePart entity)
        {
            _uow.SparePartRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public SparePart GetById(Guid id)
        {
         SparePart SparePart =   _uow.SparePartRepository.FindById(id);
         return SparePart;
        }

        public List<SparePart> GetAll()
        {
         List<SparePart> SparePart=   _uow.SparePartRepository.GetAll();
         return SparePart;
          
        }
    }
}
