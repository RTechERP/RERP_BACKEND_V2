using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderLogRepo : GenericRepo<PaymentOrderLog>
    {
        CurrentUser _currentUser;
        PaymentOrderApproveFollowRepo _followRepo;
        PaymentOrderTypeRepo _typeRepo;
        public PaymentOrderLogRepo(CurrentUser currentUser, PaymentOrderApproveFollowRepo followRepo,PaymentOrderTypeRepo typeRepo) : base(currentUser)
        {
            _currentUser = currentUser;
            _followRepo = followRepo;
            _typeRepo = typeRepo;
        }


        public async Task<int> Create(PaymentOrderDTO payment)
        {
            try
            {
                var logs = GetAll(x => x.PaymentOrderID == payment.ID && x.IsDeleted != true);

                //Xóa hết ds chi tiết
                foreach (var item in logs)
                {
                    item.IsDeleted = true;
                    await UpdateAsync(item);
                }
                logs.Clear();
                //Insert lại 1 list mới
                if (payment.IsDelete == false)
                {
                    //Get danh sách các bước duyệt
                    var paymentOrderType = _typeRepo.GetByID(TextUtils.ToInt32(payment.PaymentOrderTypeID));
                    int followType = 1;
                    if (paymentOrderType.IsIgnoreHR == true) followType = 2;
                    else if (payment.IsSpecialOrder == true) followType = 3;
                    var follows = _followRepo.GetAll(x => x.FollowType == followType && x.IsDeleted != true)
                                             .OrderBy(x => x.Step)
                                             .ToList();

                    var stepTBP = follows.FirstOrDefault(x => x.Code == "TBP") ?? new PaymentOrderApproveFollow();
                    var stepBGĐ = follows.FirstOrDefault(x => x.Code == "BGD") ?? new PaymentOrderApproveFollow();

                    foreach (var f in follows)
                    {
                        var log = new PaymentOrderLog();
                        log.PaymentOrderID = payment.ID;
                        log.Step = f.Step;
                        log.StepName = f.StepName;
                        log.IsApproved = 0;
                        log.EmployeeID = f.ApproverID;
                        if (log.Step == 1) log.EmployeeID = _currentUser.EmployeeID;
                        else if (log.Step == stepTBP.Step) log.EmployeeID = payment.ApprovedTBPID;
                        else if (log.Step == stepBGĐ.Step) log.EmployeeID = payment.ApprovedBGDID;

                        log.EmployeeApproveActualID = 0;
                        log.ReasonCancel = "";
                        log.ContentLog = "";
                        log.IsRequestAppendFileAC = false;
                        log.ReasonRequestAppendFileAC = "";

                        log.IsRequestAppendFileHR = false;
                        log.ReasonRequestAppendFileHR = "";
                        logs.Add(log);
                    }
                    
                }

                return await CreateRangeAsync(logs);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
