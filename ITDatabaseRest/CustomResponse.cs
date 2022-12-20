using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITDatabaseRest
{
    public class CustomResponse<T>
    {
        public T result;
        public Dictionary<String, String> relatedActions;
      public  CustomResponse(T value, Dictionary<String,String> relatedActions)
        {
            this.result = value;
            this.relatedActions = relatedActions;
        }
    }
}
