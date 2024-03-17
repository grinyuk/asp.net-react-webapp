using Core.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Web2PnK.Models
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }
        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
    public class ErrorResponseModel
    {
        public string Status { get; }

        public ValidationError[] Errors { get;}

        public ErrorResponseModel(ModelStateDictionary modelState)
        {
            Status = "Validation Failed";
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToArray();
        }

        public ErrorResponseModel(ResponseType message, string error)
        {
            Status = message.ToString();
            Errors = new ValidationError[] { new ValidationError("error", error) };
                    
        }
    }
}
