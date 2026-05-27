using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIEvaluationPointRepo:GenericRepo<KPIEvaluationPoint>
    {
        public KPIEvaluationPointRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> UpdateWithNullAsync(KPIEvaluationPoint item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var entity = await db.KPIEvaluationPoints.FindAsync(item.ID);
                if (entity == null)
                    throw new Exception($"Entity with ID {item.ID} not found");

                db.Entry(entity).CurrentValues.SetValues(item);

                var updatedDateProp = db.Entry(entity).Property(e => e.UpdatedDate);
                if (updatedDateProp != null)
                {
                    updatedDateProp.CurrentValue = DateTime.Now;
                    updatedDateProp.IsModified = true;
                }

                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating KPIEvaluationPoint: {ex.Message}", ex);
            }
        }
    }
}
