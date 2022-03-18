using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models
{
    [DataContract]
    public class Response<T>
    {
        [DataMember(Order = 1)] public T Data { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public bool IsSuccess { get; set; }
        [DataMember(Order = 4)] public bool IsRetriable { get; set; }
        [DataMember(Order = 5)] public int StatusCode { get; set; }

        public static Response<T> Success(T data)
        {
            return new Response<T>()
            {
                Data = data,
                IsSuccess = true
            };
        }

        public static Response<T> Error(string errorMessage, int statusCode = 500)
        {
            return new Response<T>()
            {
                Data = default,
                ErrorMessage = errorMessage,
                IsSuccess = false,
                IsRetriable = false,
                StatusCode = statusCode,
            };
        }

        public static Response<T> TechnicalError(string errorMessage)
        {
            return new Response<T>()
            {
                Data = default,
                ErrorMessage = errorMessage,
                IsSuccess = false,
                IsRetriable = true,
                StatusCode = 500,
            };
        }
    }
}