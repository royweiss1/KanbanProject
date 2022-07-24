using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.BusinessLayer;


public class Response
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; } // if error does not occur, Errormessage will be set null.
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Object ReturnValue { get;}
    
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool ErrorOccured { get => ErrorMessage != null; }

    
    public Response(string errormessage, Object returnValue)
    {
        ErrorMessage = errormessage;
        ReturnValue = returnValue;
        

    }
   
   
}