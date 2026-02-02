using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class PhasedAllocationPersonRepo : GenericRepo<PhasedAllocationPerson>
    {
        private PhasedAllocationPersonDetailRepo _phaseDetailRepo;
        public PhasedAllocationPersonRepo(CurrentUser currentUser , PhasedAllocationPersonDetailRepo phaseDetailRepo) : base(currentUser)
        {
            _phaseDetailRepo = phaseDetailRepo;
        }
     
        public async Task UpdatePhaseFromFoodOrder(List<EmployeeFoodOrder> foodOrders)
        {
            try
            {
                foreach (var item in foodOrders)
                {
                    string code = item.DateOrder.Value.ToString("ddMMyyyy");
                    var phase = GetAll(x => x.YearValue == item.DateOrder.Value.Year
                                    && x.MontValue == item.DateOrder.Value.Month
                                    && x.Code == code
                                 
                                    && x.IsDeleted != true).FirstOrDefault() ?? new PhasedAllocationPerson();
                    if (phase.ID <= 0)
                    {
                        phase.TypeAllocation = 3;
                        phase.Code = code;
                        phase.ContentAllocation = $"Cấp phát cơm ca ngày {item.DateOrder.Value.ToString("dd/MM/yyyy")}";
                        phase.YearValue = item.DateOrder.Value.Year;
                        phase.MontValue = item.DateOrder.Value.Month;
                        phase.StatusAllocation = 0;

                        await CreateAsync(phase);
                    }
                    
                    //Tìm kiếm detai
                    var detail = _phaseDetailRepo.GetAll(x => x.PhasedAllocationPersonID == phase.ID && x.EmployeeID == item.EmployeeID && x.IsDeleted != true)
                                                 .FirstOrDefault() ?? new PhasedAllocationPersonDetail();

                    if (detail.ID <= 0&&item.IsApproved==true && item.Location != 2)
                    {
                        detail.PhasedAllocationPersonID = phase.ID;
                        detail.EmployeeID = item.EmployeeID;
                        detail.StatusReceive = 0;
                        detail.Quantity = item.Quantity;
                        detail.UnitName = "Suất";
                        detail.ContentReceive = "";

                        await _phaseDetailRepo.CreateAsync(detail);
                    }
                    else if(item.IsApproved==false&& detail.ID>0)
                    {
                        detail.IsDeleted = true;
                        await _phaseDetailRepo.UpdateAsync(detail);
                    }    

                  

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
