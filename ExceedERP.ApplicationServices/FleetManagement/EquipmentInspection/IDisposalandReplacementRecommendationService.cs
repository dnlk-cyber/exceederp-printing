using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection
{
   public interface IDisposalandReplacementRecommendationService
    {
        bool Insert(DisposalandReplacementRecommendation entity);
        bool InsertBatch(List<DisposalandReplacementRecommendation> entityList);
        bool Update(DisposalandReplacementRecommendation entity);

        bool Delete(DisposalandReplacementRecommendation entity);
        DisposalandReplacementRecommendation GetById(Guid id);
        List<DisposalandReplacementRecommendation> GetAll();

    }
}
