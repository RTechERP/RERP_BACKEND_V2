using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeFoodOrderRepo : GenericRepo<EmployeeFoodOrder>
    {
        private PhasedAllocationPersonRepo phaseRepo;
        private PhasedAllocationPersonDetailRepo phaseDetailRepo;
        public EmployeeFoodOrderRepo(CurrentUser currentUser, PhasedAllocationPersonRepo phaseRepo, PhasedAllocationPersonDetailRepo phaseDetailRepo) : base(currentUser)
        {
            this.phaseRepo = phaseRepo;
            this.phaseDetailRepo = phaseDetailRepo;
        }
       
        public  bool AddPhaseAllocation(EmployeeFoodOrder model)
        {
            var phaseCode = phaseRepo.GenPhaseAllocationCode(model.DateOrder);
            var phaseCodeExist = phaseRepo.GetAll(x => x.Code == phaseCode&&x.IsDeleted!=true);
            PhasedAllocationPerson phase = new PhasedAllocationPerson();
            if (phaseCodeExist.Count==0)
            {
                phase.Code = phaseCode;
                phase.STT = phaseRepo.GetAll().Max(x => x.STT) + 1;
                phase.TypeAllocation = 3;
                phase.ContentAllocation = "Danh sách nhận cơm ngày " + model.DateOrder?.ToString("dd/MM/yyyy");
                phase.YearValue = model.DateOrder.Value.Year;
                phase.MontValue = model.DateOrder.Value.Month;
                phase.StatusAllocation = 0;
                phaseRepo.Create(phase);
                var detailExist = phaseDetailRepo.GetAll(x => x.EmployeeID == model.EmployeeID && x.PhasedAllocationPersonID == phase.ID);
                if(detailExist.Count==0)
                {
                    AddPhaseAllocationDetail(model, phase.ID);
                }              
            }
            else
            {
                return false;
            }    
            return true;
        }
        public bool AddPhaseAllocationDetail(EmployeeFoodOrder model,int PhasedAllocationPersonID)
            {
            
            PhasedAllocationPersonDetail phaseDetail = new PhasedAllocationPersonDetail();

            phaseDetail.PhasedAllocationPersonID = PhasedAllocationPersonID;
            phaseDetail.EmployeeID = model.EmployeeID;
            phaseDetail.DateReceive = model.DateOrder;
            phaseDetail.StatusReceive = 0;
            phaseDetail.Quantity = model.Quantity;
            phaseDetail.UnitName = "Xuất";
            phaseDetail.ContentReceive= "Đăng ký nhận cơm ngày " + model.DateOrder?.ToString("dd/MM/yyyy");
            phaseDetailRepo.Create(phaseDetail);
            
           
            return true;
        }


    }
}
