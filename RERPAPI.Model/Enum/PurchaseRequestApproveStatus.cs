namespace RERPAPI.Model.Enum
{
    public enum PurchaseRequestApproveStatus
    {
        RequestApprove = 1,// 1:Yêu cầu duyệt
        CancelRequestApprove = 2,//2:Hủy yêu cầu duyệt

        BGDApprove = 3,// 3:BGĐ duyệt
        BGDCancelApprove = 4,//4:BGĐ hủy duyệt

        TBPApprove = 5,//5:TBP duyệt
        TBPCancelApprove = 6,//6:TBP hủy duyệt

        Completed = 7,//7:hoàn thành
        CancelCompleted = 8,//8:Hủy hoàn thành

        CheckOrder = 9,//9:Check đặt hàng
        CancelCheckOrder = 10,//10:Hủy check đặt hàng

        SaveData = 11//11: Lưu dữ liệu
    }
}
