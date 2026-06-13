using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIEvaluationPointRepo : GenericRepo<KPIEvaluationPoint>
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

        public async Task<int> UpdateRangeWithNullAsync(List<KPIEvaluationPoint> items)
        {
            if (items == null || !items.Any())
                return 0;

            try
            {
                var ids = items.Select(x => x.ID).ToList();
                var entities = await db.KPIEvaluationPoints.Where(x => ids.Contains(x.ID)).ToListAsync();
                var entitiesDict = entities.ToDictionary(x => x.ID);

                foreach (var item in items)
                {
                    if (entitiesDict.TryGetValue(item.ID, out var entity))
                    {
                        db.Entry(entity).CurrentValues.SetValues(item);
                        var updatedDateProp = db.Entry(entity).Property(e => e.UpdatedDate);
                        if (updatedDateProp != null)
                        {
                            updatedDateProp.CurrentValue = DateTime.Now;
                            updatedDateProp.IsModified = true;
                        }
                    }
                }

                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating range of KPIEvaluationPoint: {ex.Message}", ex);
            }
        }
    }
}