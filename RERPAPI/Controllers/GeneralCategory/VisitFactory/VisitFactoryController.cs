using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;
using System.Linq;
using System;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VisitFactoryController : ControllerBase
	{
		VisitFactoryRepo visitFactoryRepo = new VisitFactoryRepo();
		VisitFactoryDetailRepo visitFactoryDetailRepo = new VisitFactoryDetailRepo();
		//EmployeeRepo employeeRepo = new EmployeeRepo();
		//EmployeeSendEmail


		[HttpGet("getall")]
		public IActionResult GetAll()
		{
			try
			{
				List<VisitFactory> items = visitFactoryRepo.GetAll(x => !x.IsDeleted);
				return Ok(new { status = 1, data = items });
			}
			catch (Exception ex)
			{
				return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
			}
		}

		[HttpGet("getbyid")]
		public IActionResult GetByID(int id)
		{
			try
			{
				var item = visitFactoryRepo.GetByID(id);
				if (item == null || item.ID <= 0) return Ok(new { status = 1, data = (VisitFactory?)null });



				// Attach details (lọc bằng LINQ với kiểu tường minh)
				item.VisitFactoryDetails = visitFactoryDetailRepo
					.GetAll()
					.Where((VisitFactoryDetail d) => d.VisitFactoryID == id && !d.IsDeleted)
					.ToList();
				return Ok(new { status = 1, data = item });
			}
			catch (Exception ex)
			{
				return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
			}
		}

		[HttpPost("save")]
		public async Task<IActionResult> Save([FromBody] VisitFactoryDTO request)
		{
			try
			{
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
                
				// Validate cơ bản
                if (request.DateVisit == default)
					return BadRequest(new { status = 0, message = "Ngày thăm không hợp lệ." });
				if (string.IsNullOrWhiteSpace(request.Purpose))
					return BadRequest(new { status = 0, message = "Vui lòng nhập Mục đích." });
				if (string.IsNullOrWhiteSpace(request.Company))
					return BadRequest(new { status = 0, message = "Vui lòng nhập Công ty." });
				if (request.TotalPeople < 1)
					return BadRequest(new { status = 0, message = "Số người phải >= 1." });

				// Convert DTO to Entity
				var entity = new VisitFactory
				{
					ID = request.Id,
					DateVisit = request.DateVisit,
					DateStart = request.DateStart,
					DateEnd = request.DateEnd,
					Purpose = request.Purpose,
					Note = request.Note,
					EmployeeID = _currentUser.EmployeeID,
					Company = request.Company,
					GuestTypeID = request.GuestTypeId,
					TotalPeople = request.TotalPeople,
					IsReceive = request.IsReceive,
					//EmployeeReceive = request.EmployeeReceive,
					EmployeeReceive = 558,
					CreatedBy = _currentUser.LoginName,
					CreatedDate = DateTime.Now,
					UpdatedBy = _currentUser.LoginName,
					UpdatedDate = DateTime.Now,
					IsDeleted = false
				};

				// Kiểm tra chồng chéo thời gian theo khung giờ (nếu có DateStart/DateEnd)
				var start = entity.DateStart ?? entity.DateVisit;
				var end = entity.DateEnd ?? entity.DateVisit;
				if (end < start)
					return BadRequest(new { status = 0, message = "Khoảng thời gian không hợp lệ (End < Start)." });

				var existing = visitFactoryRepo.GetAll(x => !x.IsDeleted);
				// Không so sánh với chính nó khi cập nhật
				var conflict = existing
					.Where(v => v.ID != entity.ID)
					.Where(v =>
					{
						var vStart = v.DateStart ?? v.DateVisit;
						var vEnd = v.DateEnd ?? v.DateVisit;
						// Nếu có chỉ định người nhận, kiểm tra trùng theo người nhận để tránh double-book
						var sameReceiver = !entity.EmployeeReceive.HasValue || v.EmployeeReceive == entity.EmployeeReceive;
						return sameReceiver && vStart <= end && start <= vEnd;
					})
					.FirstOrDefault();
				if (conflict != null)
				{
					return BadRequest(new { status = 0, message = "Khung giờ đã có đăng ký trùng. Vui lòng chọn khung khác hoặc người nhận khác." });
				}
				bool isCreate = entity.ID <= 0;
				if (isCreate)
				{
					await visitFactoryRepo.CreateAsync(entity);
				}
				else
				{
                    entity.UpdatedBy = _currentUser.LoginName;
                    entity.UpdatedDate = DateTime.Now;
					 await visitFactoryRepo.UpdateAsync(entity);
				}

				// Upsert chi tiết nếu có gửi kèm
				var details = request.VisitFactoryDetails?.ToList() ?? new List<VisitFactoryDetailDTO>();
				if (entity.ID > 0 && details.Any())
				{
					// Xóa các detail không còn trong danh sách
					var existingDetails = visitFactoryDetailRepo.GetAll(x => x.VisitFactoryID == entity.ID && !x.IsDeleted);
					var toDelete = existingDetails.Where(ed => !details.Any(d => d.Id == ed.ID)).ToList();
					foreach (var del in toDelete)
					{
                        visitFactoryDetailRepo.Delete(del.ID);
                    }

                    // Upsert từng detail hiện tại
                    foreach (var d in details)
					{
						var detailEntity = new VisitFactoryDetail
						{
							ID = d.Id,
							VisitFactoryID = entity.ID,
							EmployeeID = d.EmployeeId,
							FullName = d.FullName,
							Company = d.Company,
							Position = d.Position,
							Phone = d.Phone,
							Email = d.Email,
							CreatedBy = d.CreatedBy,
							CreatedDate = d.CreatedDate,
							UpdatedBy = d.UpdatedBy,
							UpdatedDate = d.UpdatedDate,
							IsDeleted = false
						};
						
						if (d.Id <= 0) await visitFactoryDetailRepo.CreateAsync(detailEntity);
						else await visitFactoryDetailRepo.UpdateAsync( detailEntity);
					}
				}

				// Gửi email thông báo khi tạo mới
				//if (isCreate)
				//{
				//	var configured = Config.VisitFactoryNotifyEmails;
				//	var recipients = (configured != null && configured.Length > 0)
				//		? configured
				//		: new string[] { "nguyentuan.dang@rtc.edu.vn", "admin11@rtc.edu.vn" };
				//	var subject = $"[RTC] Đăng ký thăm nhà máy - {entity.Company} - {entity.DateVisit:dd/MM/yyyy}";
				//	var body = $@"<p>Có đăng ký thăm nhà máy mới:</p>
				//		<ul>
				//			<li>Ngày thăm: {entity.DateVisit:dd/MM/yyyy HH:mm}</li>
				//			<li>Công ty: {entity.Company}</li>
				//			<li>Mục đích: {entity.Purpose}</li>
				//			<li>Số người: {entity.TotalPeople}</li>
				//			<li>Ghi chú: {entity.Note}</li>
				//		</ul>";
				//	_ = EmailHelper.SendAsync(recipients, subject, body);
				//}

				 await visitFactoryRepo.SendEmail(entity);

				// Convert Entity back to DTO for response
				var responseDto = new VisitFactoryDTO
				{
					Id = entity.ID,
					DateVisit = entity.DateVisit,
					DateStart = entity.DateStart,
					DateEnd = entity.DateEnd,
					Purpose = entity.Purpose,
					Note = entity.Note,
					EmployeeId = entity.EmployeeID,
					Company = entity.Company,
					GuestTypeId = entity.GuestTypeID,
					TotalPeople = entity.TotalPeople,
					IsReceive = entity.IsReceive,
					EmployeeReceive = entity.EmployeeReceive,
					CreatedBy = entity.CreatedBy,
					CreatedDate = entity.CreatedDate,
					UpdatedBy = entity.UpdatedBy,
					UpdatedDate = entity.UpdatedDate,
					IsDeleted = entity.IsDeleted,
					VisitFactoryDetails = details
				};

				return Ok(new { status = 1, data = responseDto });
			}
			catch (Exception ex)
			{
				return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
			}
		}

		[HttpPost("delete/{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				var item = visitFactoryRepo.GetByID(id);
				if (item == null || item.ID <= 0) return Ok(new { status = 1, message = "Not found" });
				item.IsDeleted = true;
				visitFactoryRepo.Update(item);

				// Xóa mềm toàn bộ detail liên quan
				var relatedDetails = visitFactoryDetailRepo.GetAll(x => x.VisitFactoryID == id && !x.IsDeleted);
				foreach (var d in relatedDetails)
				{
					d.IsDeleted = true;
					visitFactoryDetailRepo.Update(d);
				}
				return Ok(new { status = 1, message = "Deleted" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
			}
		}

		[HttpGet("employee-list")]
        public IActionResult getAllEmployee(int status = 0, int departmentId = 0, string keyword = "", int id = 0)
        {
            try
            {
                var resultLists = SQLHelper<dynamic>.ProcedureToList(
                    "spGetEmployee",
                    new string[] { "@Status", "@DepartmentID", "@Keyword", "@ID" },
                    new object[] { status, departmentId, keyword ?? "", id }
                );

                var employeeList = SQLHelper<dynamic>.GetListData(resultLists, 0);

                return Ok(ApiResponseFactory.Success(employeeList, "Lấy danh sách Employee thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
            }
        }
    }
}
