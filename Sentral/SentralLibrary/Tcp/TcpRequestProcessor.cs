using Newtonsoft.Json;
using SentralLibrary.Services;
using SentralLibrary.Tcp.TcpRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public class TcpRequestProcessor
{
    private readonly IClientManager clientManager;
    private readonly IDatabaseService databaseService;

    public TcpRequestProcessor(IClientManager clientManager, IDatabaseService databaseService)
    {
        this.clientManager = clientManager;
        this.databaseService = databaseService;
    }

    public Response ProcessClientRequest(TcpClientData clientData, string requestString)
    {
        IRequest? request = JsonConvert.DeserializeObject<Request>(requestString);

        if (request == null )
        {
            return new(TcpRequestConstants.RequestInvalid, TcpRequestConstants.StatusFail, "request does not match any known patterns");
        }

        switch (request.RequestType)
        {
            case TcpRequestConstants.RequestAuthorization:
                AuthorizationRequest? authorizationRequest = JsonConvert.DeserializeObject<AuthorizationRequest>(requestString);
                return HandleAuthorizationRequest(clientData, authorizationRequest);

            case TcpRequestConstants.RequestAccess:
                AccessRequest? accessRequest = JsonConvert.DeserializeObject<AccessRequest>(requestString);
                return HandleAccessRequest(clientData, accessRequest);

            case TcpRequestConstants.RequestAlarmReport:
                AlarmReportRequest? alarmReportRequest = JsonConvert.DeserializeObject<AlarmReportRequest>(requestString);
                return HandleAlarmReportRequest(clientData, alarmReportRequest);

            default:
                return HandleUnknownRequest();
        }

    }

    private Response HandleAuthorizationRequest(TcpClientData clientData, AuthorizationRequest? request)
    {
        Response response = new();
        response.Action = TcpRequestConstants.RequestAuthorization;

        if (request == null)
        {
            response.Message = "request type does not match the request";
            response.Status = TcpRequestConstants.StatusFail;
        }
        else if (request.ClientId < 1 || request.ClientId > 99)
        {
            response.Message = "client id is outside range (1 - 99)";
            response.Status = TcpRequestConstants.StatusNotAccepted;
        }
        else if(clientManager.IsDuplicateClientId(request.ClientId))
        {
            response.Message = $"client id already exists ({request.ClientId})";
            response.Status = TcpRequestConstants.StatusNotAccepted;
        }
        else
        {
            clientData.IsAuthenticated = true;
            response.Message = "connection authorized";
            response.Status = TcpRequestConstants.StatusAccepted;
        }

        return response;
    }

    private Response HandleAccessRequest(TcpClientData clientData, AccessRequest? request)
    {
        if (request == null)
        {

        }

        return HandleUnknownRequest();
    }

    private Response HandleAlarmReportRequest(TcpClientData clientData, AlarmReportRequest? request)
    {
        if (request == null)
        {

        }

        return HandleUnknownRequest();
    }

    private static Response HandleUnknownRequest()
    {
        return new(TcpRequestConstants.RequestInvalid, TcpRequestConstants.StatusFail, "request does not match any known patterns");
    }

}
