using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, int code, string message)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        public T Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
