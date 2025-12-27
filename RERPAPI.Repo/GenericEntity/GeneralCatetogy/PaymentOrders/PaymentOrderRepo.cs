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
    public class PaymentOrderRepo : GenericRepo<PaymentOrder>
    {
        string[] PREFIX_CODES = new string[] { "", "ĐNTU", "ĐNTT" };
        public PaymentOrderRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetCode(PaymentOrder payment)
        {
            try
            {
                string code = string.Empty;
                int stt = 0;
                string prefixCode = "ĐNTTĐB";
                if (payment.IsSpecialOrder == true)
                {
                    var payments = GetAll(x => x.IsSpecialOrder == payment.IsSpecialOrder && x.IsDelete != true
                                           && x.DateOrder.Value.Year == payment.DateOrder.Value.Year
                                           && x.DateOrder.Value.Month == payment.DateOrder.Value.Month
                                           && x.DateOrder.Value.Day == payment.DateOrder.Value.Day)
                                   .Select(x => new
                                   {
                                       Code = x.Code,
                                       STT = string.IsNullOrWhiteSpace(x.Code) ? 0 : TextUtils.ToInt32(x.Code.Substring(x.Code.Length - 4))
                                   }).ToList();

                    if (payments.Count() > 0) stt = payments.Max(x => x.STT);
                }
                else
                {
                    var payments = GetAll(x => x.TypeOrder == payment.TypeOrder && x.IsDelete != true
                                            && x.DateOrder.Value.Year == payment.DateOrder.Value.Year
                                            && x.DateOrder.Value.Month == payment.DateOrder.Value.Month
                                            && x.DateOrder.Value.Day == payment.DateOrder.Value.Day)
                                    .Select(x => new
                                    {
                                        Code = x.Code,
                                        STT = string.IsNullOrWhiteSpace(x.Code) ? 0 : TextUtils.ToInt32(x.Code.Substring(x.Code.Length - 4))
                                    }).ToList();

                    if (payments.Count() > 0) stt = payments.Max(x => x.STT);
                    prefixCode = PREFIX_CODES[TextUtils.ToInt32(payment.TypeOrder)];
                }

                stt++;
                string sttText = stt.ToString().PadLeft(4,'0');

                code = prefixCode + payment.DateOrder.Value.ToString("yyyyMMdd") + sttText;
                return code;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public APIResponse Validate(PaymentOrderDTO payment)
        {

            try
            {
                var response = ApiResponseFactory.Success(null, "");

                if (payment.TypeOrder <= 0 && payment.IsSpecialOrder == false)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng chọn Loại đề nghị!");
                }

                if (payment.TypeOrder == 1 && !payment.DatePayment.HasValue)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng chọn Nhập thời gian thanh quyết toán!");
                }

                if (payment.PaymentOrderTypeID <= 0 && payment.IsSpecialOrder == false)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng chọn Loại nội dung đề nghị!");
                }

                if (!payment.DateOrder.HasValue)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Ngày đề nghị!");
                }

                //if (payment.)
                //{
                //    response = ApiResponseFactory.Fail(null, "Vui lòng chọn TBP duyệt!");
                //}

                if (string.IsNullOrWhiteSpace(payment.ReasonOrder))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Lý do!");
                }

                if (string.IsNullOrWhiteSpace(payment.ReceiverInfo) && payment.IsSpecialOrder == false)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Thông tin người nhận tiền!");
                }

                if (payment.TypePayment == 1)
                {
                    if (payment.TypeBankTransfer <= 0)
                    {
                        response = ApiResponseFactory.Fail(null, "Vui lòng chọn Hình thức chuyển khoản!");
                    }

                    if (string.IsNullOrWhiteSpace(payment.AccountNumber))
                    {
                        response = ApiResponseFactory.Fail(null, "Vui lòng nhập Số tài khoản!");
                    }

                    if (string.IsNullOrWhiteSpace(payment.Bank))
                    {
                        response = ApiResponseFactory.Fail(null, "Vui lòng nhập Ngân hàng!");
                    }

                    if (string.IsNullOrWhiteSpace(payment.ContentBankTransfer))
                    {
                        response = ApiResponseFactory.Fail(null, "Vui lòng nhập Nội dung chuyển khoản!");
                    }
                }

                if (string.IsNullOrWhiteSpace(payment.Unit))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Loại tiền!");
                }

                //Check chi tiết thanh toán
                foreach (var detail in payment.PaymentOrderDetails)
                {
                    if (string.IsNullOrWhiteSpace(detail.ContentPayment))
                    {
                        string message = payment.IsSpecialOrder == true ? "Đối tượng nhận COM" : "Nội dung thanh toán";
                        response = ApiResponseFactory.Fail(null, $"Vui lòng nhập {message} dòng [{detail.STT}]!");
                        break;
                    }

                    if (payment.TypeOrder == 2)
                    {
                        if (detail.TotalMoney == 0 && detail.ParentID == 2)
                        {
                            string message = payment.IsSpecialOrder == true ? "Số tiền\n" : "Thành tiền\n";
                            response = ApiResponseFactory.Fail(null, $"Vui lòng nhập {message} dòng {detail.STT}.{detail.ContentPayment}!");
                            break;
                        }
                    }
                    else
                    {
                        if (detail.TotalMoney == 0)
                        {
                            string message = payment.IsSpecialOrder == true ? "Số tiền\n" : "Thành tiền\n";
                            response = ApiResponseFactory.Fail(null, $"Vui lòng nhập {message} dòng {detail.STT}.{detail.ContentPayment}!");
                            break;
                        }
                    }
                    
                }

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
