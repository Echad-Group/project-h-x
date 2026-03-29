namespace ThirdPartyServices
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            Status = false;
            Message = string.Empty;
        }

        public ResponseModel(bool s, string m)
        {
            Status = s;
            Message = m;
        }

        public bool Status { get; set; }
        public string Message { get; set; }

    }
}