using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models
{
    [DataContract]
    public class Response<T>
    {
        [DataMember(Order = 1)] public T Data { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public bool IsSuccess { get; set; }

        public static Response<T> Success(T data)
        {
            return new Response<T>()
            {
                Data = data,
                IsSuccess = true
            };
        }

        public static Response<T> Error(string errorMessage)
        {
            return new Response<T>()
            {
                ErrorMessage = errorMessage,
                IsSuccess = false
            };
        }
    }
}