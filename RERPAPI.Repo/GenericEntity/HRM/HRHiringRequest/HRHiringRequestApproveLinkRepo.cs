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
        public HRHiringRequestApproveLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

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
                APIResponse response = ApiResponseFactory.Success(null, "Thao tác thành công!");
                foreach (var actionApproved in actionApproveds)
                {
                    var approves = GetAll(x => x.HRHiringRequestID == actionApproved.HRHiringRequestID && x.IsDeleted == false);
                    int nextStep = actionApproved.Step + 1;

                    // Lấy bản ghi hiện tại hoặc khởi tạo mới (Upsert logic - CHỐNG LỖI ID 0 NOT FOUND)
                    var approve = approves.FirstOrDefault(x => x.Step == actionApproved.Step);
                    bool isNew = false;
                    if (approve == null)
                    {
                        isNew = true;
                        approve = new HRHiringRequestApproveLink
                        {
                            HRHiringRequestID = actionApproved.HRHiringRequestID,
                            Step = actionApproved.Step,
                            StepName = GetStepName(actionApproved.Step),
                            CreatedBy = currentUser.LoginName,
                            IsDeleted = false
                        };
                    }

                    // 1=Duyệt, 2=Không duyệt, 0=Hủy duyệt
                    if (actionApproved.IsApprove == 1 || actionApproved.IsApprove == 2)
                    {
                        var next = approves.FirstOrDefault(x => x.Step == nextStep);
                        if (next != null && next.IsApprove == 1)
                        {
                            response = ApiResponseFactory.Fail(null, "Cấp sau đã duyệt. Bạn không thể thay đổi trạng thái ở bước này!");
                            continue;
                        }

                        if (actionApproved.Step > 1)
                        {
                            var prev = approves.FirstOrDefault(x => x.Step == actionApproved.Step - 1);
                            if (prev == null || prev.IsApprove != 1)
                            {
                                response = ApiResponseFactory.Fail(null, "Cấp trước chưa duyệt. Vui lòng kiểm tra lại!");
                                continue;
                            }
                        }
                        await SaveApproval(approve, actionApproved, currentUser, isNew);
                    }
                    else if (actionApproved.IsApprove == 0) // Hủy duyệt
                    {
                        var next = approves.FirstOrDefault(x => x.Step == nextStep);
                        if (next != null && next.IsApprove == 1)
                        {
                            response = ApiResponseFactory.Fail(null, "Cấp sau đã duyệt. Bạn không thể hủy duyệt ở bước này!");
                            continue;
                        }
                        await SaveApproval(approve, actionApproved, currentUser, isNew);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task SaveApproval(HRHiringRequestApproveLink approve, HRHiringRequestApproveLink action, CurrentUser currentUser, bool isNew)
        {
            approve.ApproveID = currentUser.EmployeeID;
            approve.IsApprove = action.IsApprove;
            approve.DateApprove = (action.IsApprove == 1 || action.IsApprove == 2) ? DateTime.Now : (DateTime?)null;
            approve.ReasonUnApprove = action.ReasonUnApprove;
            approve.Note = action.Note;
            approve.UpdatedBy = currentUser.LoginName;
            approve.UpdatedDate = DateTime.Now;

            if (isNew) await CreateAsync(approve);
            else await UpdateAsync(approve);
        }

        private string GetStepName(int step)
        {
            return step switch
            {
                1 => "TBP xác nhận",
                2 => "HR xác nhận",
                3 => "TBP HR xác nhận",
                4 => "BGĐ xác nhận",
                _ => $"Bước {step}"
            };
        }
    }
}
