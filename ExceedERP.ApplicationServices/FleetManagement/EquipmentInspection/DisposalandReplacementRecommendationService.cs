using ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection
{
 public  class DisposalandReplacementRecommendationService :IDisposalandReplacementRecommendationService
    {

       private readonly UnitOfWork _uow;
        public DisposalandReplacementRecommendationService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(DisposalandReplacementRecommendation entity)
        {
            _uow.DisposalandReplacementRecommendationRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<DisposalandReplacementRecommendation> entityList)
        {
            _uow.DisposalandReplacementRecommendationRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(DisposalandReplacementRecommendation entity)
        {
            _uow.DisposalandReplacementRecommendationRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(DisposalandReplacementRecommendation entity)
        {
            _uow.DisposalandReplacementRecommendationRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public DisposalandReplacementRecommendation GetById(Guid id)
        {
         DisposalandReplacementRecommendation DisposalandReplacementRecommendation =   _uow.DisposalandReplacementRecommendationRepository.FindById(id);
         return DisposalandReplacementRecommendation;
        }

        public List<DisposalandReplacementRecommendation> GetAll()
        {
         List<DisposalandReplacementRecommendation> DisposalandReplacementRecommendation=   _uow.DisposalandReplacementRecommendationRepository.GetAll();
         return DisposalandReplacementRecommendation;
          
        }
    }
}
