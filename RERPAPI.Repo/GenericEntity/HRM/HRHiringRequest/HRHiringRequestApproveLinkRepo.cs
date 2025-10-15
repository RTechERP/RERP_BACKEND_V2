using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestApproveLinkRepo : GenericRepo<HRHiringRequestApproveLink>
    {
        public async Task CreateApprove(HRHiringRequest hrHiring,CurrentUser currentUser)
        {
            try
            {
                var approves = GetAll(x => x.HRHiringRequestID == hrHiring.ID && x.IsDeleted == false);
                if (approves.Count() <= 0)
                {
                    List<HRHiringRequestApproveLink> approveLinks = new List<HRHiringRequestApproveLink>()
                    {
                        new HRHiringRequestApproveLink()
                        {
                            STT = 1,
                            HRHiringRequestID = hrHiring.ID,
                            ApproveID = 0,
                            IsApprove = 0,
                            Step = 1,
                            StepName = "TBP xác nhận",
                            CreatedBy = currentUser.LoginName,
                            UpdatedBy = currentUser.LoginName
                        },
                        new HRHiringRequestApproveLink()
                        {
                            STT = 2,
                            HRHiringRequestID = hrHiring.ID,
                            ApproveID = 0,
                            IsApprove = 0,
                            Step = 2,
                            StepName = "HR xác nhận",
                            CreatedBy = currentUser.LoginName,
                            UpdatedBy = currentUser.LoginName
                        },
                              new HRHiringRequestApproveLink()
                        {
                            STT = 3,
                            HRHiringRequestID = hrHiring.ID,
                            ApproveID = 0,
                            IsApprove = 0,
                            Step = 3,
                            StepName = "TBP HR xác nhận",
                            CreatedBy = currentUser.LoginName,
                            UpdatedBy = currentUser.LoginName
                        },
                        new HRHiringRequestApproveLink()
                        {
                            STT = 4,
                            HRHiringRequestID = hrHiring.ID,
                            ApproveID = 0,
                            IsApprove = 0,
                            Step = 4,
                            StepName = "BGĐ xác nhận",
                            CreatedBy = currentUser.LoginName,
                            UpdatedBy = currentUser.LoginName
                        }
                    };


                   await  CreateRangeAsync(approveLinks);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<APIResponse> Approved(List<HRHiringRequestApproveLink> actionApproveds, CurrentUser currentUser)
        {
            try
            {
                APIResponse response = new APIResponse();
                foreach (var actionApproved in actionApproveds)
                {
                    var approves = GetAll(x => x.HRHiringRequestID == actionApproved.HRHiringRequestID && x.IsDeleted == false);
                    int nextStep = actionApproved.Step + 1;
                    switch (actionApproved.Step)
                    {
                        case 1: //Nếu là TBP
                            if (actionApproved.IsApprove == 1) //Nếu là duyệt
                            {
                                var approve = approves.FirstOrDefault(x => x.Step == actionApproved.Step) ?? new HRHiringRequestApproveLink();
                                approve.ApproveID = currentUser.EmployeeID;
                                approve.IsApprove = actionApproved.IsApprove;
                                approve.DateApprove = DateTime.Now;
                                approve.ReasonUnApprove = actionApproved.ReasonUnApprove;
                                approve.Note = actionApproved.Note;
                                approve.UpdatedBy = currentUser.LoginName;
                                await UpdateAsync(approve);
                                response = ApiResponseFactory.Success(null, "Duyệt thành công!");
                            }
                            else if (actionApproved.IsApprove == 2) //Nếu là hủy duyệt
                            {
                                // Kiểm tra HR đã hủy duyệt chưa
                                var approveHR = approves.FirstOrDefault(x => x.Step == nextStep) ?? new HRHiringRequestApproveLink();
                                if (approveHR.IsApprove == 1)
                                {
                                    response = ApiResponseFactory.Fail(null, "HR đã duyệt. Vui lòng hủy duyệt ở HR trước!");
                                }
                                else
                                {
                                    var approve = approves.FirstOrDefault(x => x.Step == actionApproved.Step) ?? new HRHiringRequestApproveLink();
                                    approve.ApproveID = currentUser.EmployeeID;
                                    approve.IsApprove = actionApproved.IsApprove;
                                    approve.DateApprove = DateTime.Now;
                                    approve.ReasonUnApprove = actionApproved.ReasonUnApprove;
                                    approve.Note = actionApproved.Note;
                                    approve.UpdatedBy = currentUser.LoginName;
                                    await UpdateAsync(approve);
                                    response = ApiResponseFactory.Success(null, "Hủy duyệt thành công!");
                                }
                            }
                            break;
                        case 2: //Nếu là HR
                            if (actionApproved.IsApprove == 1) //Nếu là duyệt
                            {
                                //Kiểm tra TBP đã duyệt chưa
                                var approveTBP = approves.FirstOrDefault(x => x.Step == actionApproved.Step - 1) ?? new HRHiringRequestApproveLink();
                                if (approveTBP.IsApprove != 1)
                                {
                                    response = ApiResponseFactory.Fail(null, "TBP chưa duyệt. Vui lòng duyệt ở TBP trước!");
                                }
                                else
                                {
                                    var approve = approves.FirstOrDefault(x => x.Step == actionApproved.Step) ?? new HRHiringRequestApproveLink();
                                    approve.ApproveID = currentUser.EmployeeID;
                                    approve.IsApprove = actionApproved.IsApprove;
                                    approve.DateApprove = DateTime.Now;
                                    approve.ReasonUnApprove = actionApproved.ReasonUnApprove;
                                    approve.Note = actionApproved.Note;
                                    approve.UpdatedBy = currentUser.LoginName;
                                    await UpdateAsync(approve);
                                    response = ApiResponseFactory.Success(approve, "Duyệt thành công!");
                                }

                            }
                            else if (actionApproved.IsApprove == 2) //Nếu là hủy duyệt
                            {
                                // Kiểm tra BGĐ đã hủy duyệt chưa
                                var approveHR = approves.FirstOrDefault(x => x.Step == nextStep) ?? new HRHiringRequestApproveLink();
                                if (approveHR.IsApprove == 1)
                                {
                                    response = ApiResponseFactory.Fail(null, "BGĐ đã duyệt. Vui lòng hủy duyệt ở BGĐ trước!");
                                }
                                else
                                {
                                    var approve = approves.FirstOrDefault(x => x.Step == actionApproved.Step) ?? new HRHiringRequestApproveLink();
                                    approve.ApproveID = currentUser.EmployeeID;
                                    approve.IsApprove = actionApproved.IsApprove;
                                    approve.DateApprove = DateTime.Now;
                                    approve.ReasonUnApprove = actionApproved.ReasonUnApprove;
                                    approve.Note = actionApproved.Note;
                                    approve.UpdatedBy = currentUser.LoginName;
                                    await UpdateAsync(approve);
                                    response = ApiResponseFactory.Success(null, "Hủy duyệt thành công!");
                                }
                            }
                            break;

                        case 3: //Nếu là BGĐ
                            if (actionApproved.IsApprove == 1) //Nếu là duyệt
                            {
                                //Kiểm tra HR đã duyệt chưa
                                var approveBGD = approves.FirstOrDefault(x => x.Step == actionApproved.Step - 1) ?? new HRHiringRequestApproveLink();
                                if (approveBGD.IsApprove != 1)
                                {
                                    response = ApiResponseFactory.Fail(null, "HR chưa duyệt. Vui lòng duyệt ở HR trước!");
                                }
                                else
                                {
                                    var approve = approves.FirstOrDefault(x => x.Step == actionApproved.Step) ?? new HRHiringRequestApproveLink();
                                    approve.ApproveID = currentUser.EmployeeID;
                                    approve.IsApprove = actionApproved.IsApprove;
                                    approve.DateApprove = DateTime.Now;
                                    approve.ReasonUnApprove = actionApproved.ReasonUnApprove;
                                    approve.Note = actionApproved.Note;
                                    approve.UpdatedBy = currentUser.LoginName;
                                    await UpdateAsync(approve);
                                    response = ApiResponseFactory.Success(approve, "Duyệt thành công!");
                                }

                            }
                            break;

                        default:
                            break;
                    }
                }

                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
