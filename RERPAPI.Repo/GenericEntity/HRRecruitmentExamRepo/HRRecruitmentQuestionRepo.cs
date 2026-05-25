using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentQuestionRepo : GenericRepo<HRRecruitmentQuestion>
    {
        public HRRecruitmentQuestionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool CheckValidate(HRRecruitmentQuestionAnswersDTO item, out string message)
        {
            message = string.Empty;
            // 1. Kiểm tra nội dung câu hỏi (luôn luôn phải có)
            if (string.IsNullOrWhiteSpace(item.question.QuestionText))
            {
                message = "Vui lòng nhập nội dung câu hỏi!";
                return false;
            }
            // 2. Kiểm tra theo loại câu hỏi (1: Trắc nghiệm, 2: Tự luận)
            if (item.question.QuestionType == 1) // TRẮC NGHIỆM
            {
                if (item.answers == null || item.answers.Count == 0)
                {
                    message = "Câu hỏi trắc nghiệm phải có ít nhất một phương án trả lời!";
                    return false;
                }

                //if (item.answers.Count > 4)
                //{
                //    message = "Số lượng phương án trả lời không được vượt quá 4!";
                //    return false;
                //}

                bool hasRightAnswer = false;
                int index = 1;
                foreach (var a in item.answers)
                {
                    if (string.IsNullOrWhiteSpace(a.AnswersText))
                    {
                        message = $"Nội dung phương án thứ {index} không được để trống!";
                        return false;
                    }
                    if (a.IsRightAnswer)
                    {
                        hasRightAnswer = true;
                    }
                    index++;
                }

                if (!hasRightAnswer)
                {
                    message = "Vui lòng chọn ít nhất một đáp án đúng cho câu hỏi trắc nghiệm!";
                    return false;
                }
            }
            else if (item.question.QuestionType == 2) // TỰ LUẬN
            {
                // Đối với câu hỏi tự luận, có 2 trường hợp:
                // - Tự luận dài (chấm thủ công): item.answers sẽ là null hoặc rỗng.
                // - Tự luận có đáp án cố định (tự động chấm): item.answers sẽ có 1 bản ghi duy nhất.

                if (item.answers != null && item.answers.Count > 1)
                {
                    message = "Câu hỏi tự luận chỉ được có tối đa một đáp án đúng cố định (nếu có).\nĐể trống phần đáp án nếu đây là tự luận dài cần chấm thủ công.";
                    return false;
                }

                // Nếu có 1 đáp án, kiểm tra nội dung của nó
                if (item.answers != null && item.answers.Count == 1)
                {
                    var singleAnswer = item.answers.First();
                    if (string.IsNullOrWhiteSpace(singleAnswer.AnswersText))
                    {
                        message = "Nội dung đáp án đúng cho câu hỏi tự luận không được để trống!";
                        return false;
                    }
                    // Nếu đã cung cấp đáp án cho tự luận, nó phải được coi là đáp án đúng
                    if (!singleAnswer.IsRightAnswer)
                    {
                        singleAnswer.IsRightAnswer = true; // Tự động gán là đáp án đúng
                    }
                }
            }
            else
            {
                message = "Loại câu hỏi không hợp lệ!";
                return false;
            }


            return true;
        }
    }
}
