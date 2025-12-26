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
        //PaymentOrderRepo _paymentOrderRepo;
        public PaymentOrderLogRepo(CurrentUser currentUser, PaymentOrderApproveFollowRepo followRepo, PaymentOrderTypeRepo typeRepo) : base(currentUser)
        {
            _currentUser = currentUser;
            _followRepo = followRepo;
            _typeRepo = typeRepo;
            //_paymentOrderRepo = paymentOrderRepo;
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
                        if (log.Step == 1)
                        {
                            log.EmployeeID = _currentUser.EmployeeID;
                            log.DateApproved = DateTime.Now;
                            log.IsApproved = 1;
                        }
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


        public async Task<int> Appoved(List<PaymentOrderDTO> payments)
        {
            try
            {
                string reasonRequestAppendFileHR = string.Empty;
                string reasonCancel = string.Empty;
                string reasonCancel_1 = string.Empty;
                bool isRequestAppendFileHR = false;

                foreach (var item in payments)
                {
                    int actionStep = 0;
                    string statusText = "";

                    if (item.ID <= 0) continue;

                    //int currentStep = item.Step;
                    //int paymentOrderTypeID = item.PaymentOrderTypeID ?? 0;
                    var paymentOrderType = _typeRepo.GetByID(item.PaymentOrderTypeID ?? 0);

                    if (item.IsSpecialOrder == true)
                    {
                        if (item.Action.ButtonActionGroup == "btnTBP") actionStep = 2;
                        else if (item.Action.ButtonActionGroup == "btnHR")
                        {

                        }
                        else if (item.Action.ButtonActionGroup == "btnKTT") actionStep = 3;
                        else if (item.Action.ButtonActionGroup == "btnBGĐ") actionStep = 4;
                        else if (item.Action.ButtonActionGroup == "btnKTTT")
                        {
                            //if (item.Action.ButtonActionName == "btnReceiveDocument" || item.Action.ButtonActionName == "btnUnApproveDocument") return ApiResponseFactory.Fail(null,"");
                            if (item.Action.ButtonActionName == "btnReceiveDocument" || item.Action.ButtonActionName == "btnUnApproveDocument") return 0;
                            else if (item.Action.ButtonActionName == "btnIsPayment" || item.Action.ButtonActionName == "btnUnPayment") actionStep = 5;
                        }
                    }
                    else if (paymentOrderType.IsIgnoreHR == true)
                    {
                        if (item.Action.ButtonActionGroup == "btnTBP") actionStep = 2;
                        else if (item.Action.ButtonActionGroup == "btnHR")
                        {

                        }
                        else if (item.Action.ButtonActionGroup == "btnKTTT")
                        {
                            if (item.Action.ButtonActionName == "btnApproveDocument" || item.Action.ButtonActionName == "btnUnApproveDocument" || item.Action.ButtonActionName == "btnUpdateDocument") actionStep = 3;
                            else if (item.Action.ButtonActionName == "btnReceiveDocument" || item.Action.ButtonActionName == "btnUnReceiveDocument") actionStep = 6;
                            else if (item.Action.ButtonActionName == "btnIsPayment" || item.Action.ButtonActionName == "btnUnPayment") actionStep = 7;
                        }
                        else if (item.Action.ButtonActionGroup == "btnKTT") actionStep = 4;
                        else if (item.Action.ButtonActionGroup == "btnBGĐ") actionStep = 5;
                    }
                    else
                    {
                        if (item.Action.ButtonActionGroup == "btnTBP") actionStep = 2;
                        else if (item.Action.ButtonActionGroup == "btnHR")
                        {
                            //actionStep = 3;
                            //if (buttonName == "btnApproveDocumentHR" || buttonName == "btnUnApproveDocumentHR") actionStep = 3;
                            // lee min khooi update 03/10/2024
                            if (item.Action.ButtonActionName == "btnApproveDocumentHR" || item.Action.ButtonActionName == "btnUnApproveDocumentHR" || item.Action.ButtonActionName == "btnHRUpdateDocument") actionStep = 3;
                            else if (item.Action.ButtonActionName == "btnApproveHR" || item.Action.ButtonActionName == "btnUnApproveHR") actionStep = 4;
                        }
                        else if (item.Action.ButtonActionGroup == "btnKTTT")
                        {
                            if (item.Action.ButtonActionName == "btnApproveDocument" || item.Action.ButtonActionName == "btnUnApproveDocument" || item.Action.ButtonActionName == "btnUpdateDocument") actionStep = 5;
                            else if (item.Action.ButtonActionName == "btnReceiveDocument" || item.Action.ButtonActionName == "btnUnReceiveDocument") actionStep = 8;
                            else if (item.Action.ButtonActionName == "btnIsPayment" || item.Action.ButtonActionName == "btnUnPayment") actionStep = 9;
                        }
                        else if (item.Action.ButtonActionGroup == "btnKTT") actionStep = 6;
                        else if (item.Action.ButtonActionGroup == "btnBGĐ") actionStep = 7;
                    }

                    if (actionStep == 0)
                    {
                        //return ApiResponseFactory.Fail(null, $"Đề nghị [{item.Code}] không cần dropdownButtonText.Trim().ToLower() buttonText.Trim().ToLower()!");
                        return 0;
                    }

                    if (actionStep == 2) //Kiểm tra đề nghị có phải của tbp đó hay ko
                    {
                        var logDb = GetAll(x => x.IsDeleted != true
                                            && x.PaymentOrderID == item.ID
                                            && x.Step == actionStep).FirstOrDefault() ?? new PaymentOrderLog();

                        if (logDb.EmployeeID != _currentUser.EmployeeID)
                        {
                            //ApiResponseFactory.Fail(null, $"Bạn không thể {statusText} đề nghị [{item.Code}]!");
                            continue;
                        }
                    }

                    //int currentApproved = item.PaymentOrderLog.IsApproved ?? 0;
                    if (actionStep == item.Step)
                    {
                        if (item.PaymentOrderLog.IsApproved == 1 && item.CurrentApproved == 2 && item.Action.ButtonActionGroup != "btnBGĐ")
                        {
                            ApiResponseFactory.Fail(null, $"Bạn không thể {statusText} đề nghị [{item.Code}]!");
                            continue;
                        }
                        else if (item.PaymentOrderLog.IsApproved == 1 && item.CurrentApproved == 1)
                        {
                            ApiResponseFactory.Fail(null, $"Đề nghị [{item.Code}] đã được duyệt!");
                            continue;
                        }
                    }
                    else if (actionStep < item.Step)
                    {
                        if (item.PaymentOrderLog.IsApproved != 1)
                        {
                            ApiResponseFactory.Fail(null, "Vui lòng huỷ duyệt ở bước sau trước!", "Thông báo");
                            continue;
                        }
                    }
                    else
                    {

                        if (item.CurrentApproved != 1)
                        {
                            //MessageBox.Show($"Bạn không thể {statusText}!", "Thông báo");
                            continue;
                        }
                        else if ((actionStep - item.Step) == 1) //Ở ngay bước sau
                        {
                            if (item.PaymentOrderLog.IsApproved == 1 && item.CurrentApproved == 2 && item.Action.ButtonActionGroup != "btnBGĐ")
                            {
                                //MessageBox.Show("Bạn không thể huỷ duyệt!", "Thông báo");
                                continue;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("Vui lòng duyệt ở bước trước!", "Thông báo");
                            continue;
                        }
                    }


                    //Get quy trình duyệt
                    var log = GetAll(x => x.PaymentOrderID == item.ID && x.Step == actionStep).FirstOrDefault() ?? new PaymentOrderLog();
                    if (item.PaymentOrderLog.IsApproved == 2)
                    {
                        if (string.IsNullOrWhiteSpace(item.ReasonCancel))
                        {
                            ApiResponseFactory.Fail(null, "Vui lòng nhập Lý do hủy!", "Thông báo");
                            continue;
                        }

                        log.DateApproved = DateTime.Now;
                        log.IsApproved = item.PaymentOrderLog.IsApproved;
                        log.EmployeeApproveActualID = _currentUser.EmployeeID;
                        log.ReasonCancel += $"{DateTime.Now.ToString("dd/MM/yyyy")}: " + item.ReasonCancel + "\n";
                        log.ContentLog += $"{DateTime.Now.ToString("dd/MM/yyyy")}: {_currentUser.FullName} {item.Action.ButtonActionText}\n";

                        await UpdateAsync(log);
                    }
                    else
                    {
                        log.DateApproved = DateTime.Now;
                        log.IsApproved = item.PaymentOrderLog.IsApproved;
                        log.EmployeeApproveActualID = _currentUser.EmployeeID;
                        log.ReasonCancel ="";
                        log.ContentLog += $"{DateTime.Now.ToString("dd/MM/yyyy")}: {_currentUser.FullName} {item.Action.ButtonActionText}\n";

                        if (item.Action.ButtonActionName == "btnApproveDocument" || item.Action.ButtonActionName == "btnApproveKT" || item.Action.ButtonActionName == "btnUpdateDocument")
                        {
                            //item.AccountingNote += item.

                            if (item.Action.ButtonActionName == "btnUpdateDocument")
                            {
                                //var paymentOrder = _paymentOrderRepo.GetByID(item.ID);
                                //paymentOrder.AccountingNote += item.AccountingNote + "\n";
                                //await _paymentOrderRepo.UpdateAsync(paymentOrder);
                            }

                            log.DateApproved = DateTime.Now;
                            log.IsApproved = item.PaymentOrderLog.IsApproved;
                            log.EmployeeApproveActualID = _currentUser.EmployeeID;
                            log.ReasonCancel = "";
                            log.ContentLog += $"{DateTime.Now.ToString("dd/MM/yyyy")}: {_currentUser.FullName} {item.Action.ButtonActionText}\n";
                            log.IsRequestAppendFileAC = item.PaymentOrderLog.IsApproved == 3;
                            log.ReasonRequestAppendFileAC = item.AccountingNote;
                        }
                        else
                        {
                            if (item.Action.ButtonActionName == "btnHRUpdateDocument" && !isRequestAppendFileHR)
                            {
                                log.ReasonRequestAppendFileHR = reasonRequestAppendFileHR;
                                log.IsRequestAppendFileHR = isRequestAppendFileHR;
                                log.DateApproved = DateTime.Now;
                                log.IsApproved = item.PaymentOrderLog.IsApproved;
                                log.EmployeeApproveActualID = _currentUser.EmployeeID;
                                log.ReasonCancel = "";
                                log.ContentLog += $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}: {_currentUser.FullName} {item.Action.ButtonActionText}\n";

                                if (item.Action.ButtonActionName == "btnUpdateDocument") log.IsRequestAppendFileAC = true;
                            }
                        }
                        await UpdateAsync(log);

                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
