using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;

using System.Web.Http;
using System.Web.Http.ModelBinding;

using Newtonsoft.Json;

using ChatServer.Dto.Output.Exceptions;

namespace ChatServer.Controllers
{
    public class ControllerBase : ApiController
    {
        protected class Result
        {
            [JsonProperty("data")]
            public object Data { get; set; }

            [JsonProperty("error")]
            public object Error { get; set; }
        }

        public HttpResponseMessage ProtectedExecuteAndWrapResult(Func<object> executor, object parameter)
        {
            if (parameter == null)
            {
                Result nullDtoResult = new Result();
                nullDtoResult.Error = new ValidationException("passed dto");
                return Request.CreateResponse(HttpStatusCode.BadRequest, nullDtoResult);
            }
            else
                return ProtectedExecuteAndWrapResult(executor);
        }

        public HttpResponseMessage ProtectedExecuteAndWrapResult(Func<object> executor)
        {
            Result result = new Result();

            ICollection<string> errors = new List<string>();
            foreach (KeyValuePair<string, ModelState> fieldState in ModelState)
                foreach (ModelError error in fieldState.Value.Errors)
                    errors.Add(error.ErrorMessage ?? error.Exception?.Message);

            if(errors.Count != 0)
            {
                result.Error = errors;
                Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }

            try
            {
                result.Data = executor();
                return Request.CreateResponse(result);
            }
            catch(ConflictException ex) { result.Error = ex; return Request.CreateResponse(HttpStatusCode.Conflict, result); }
            catch(NotFoundException ex) { result.Error = ex; return Request.CreateResponse(HttpStatusCode.NotFound, result); }
            catch (ValidationException ex) { result.Error = ex; return Request.CreateResponse(HttpStatusCode.BadRequest, result); }
            catch (CustomException ex) { result.Error = ex; return Request.CreateResponse(HttpStatusCode.BadRequest, result); }
        }
    }
}
