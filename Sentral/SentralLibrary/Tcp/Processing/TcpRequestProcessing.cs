using Newtonsoft.Json;
using SentralLibrary.Database;
using SentralLibrary.Database.DataClasses;
using SentralLibrary.Database.Services;
using SentralLibrary.Tcp.DataClasses;
using SentralLibrary.Tcp.TcpRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.Processing;
public class TcpRequestProcessing
{
    private readonly IClientManager clientManager;
    private readonly IDatabaseService databaseService;

    public TcpRequestProcessing(IClientManager clientManager, IDatabaseService databaseService)
    {
        this.clientManager = clientManager;
        this.databaseService = databaseService;
    }

    public Response ProcessClientRequest(TcpClientData clientData, string requestString)
    {
        IRequest? request = JsonConvert.DeserializeObject<Request>(requestString);

        if (request == null)
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
        if (request == null)
        {
            return HandleUnknownRequest();
        }

        Response response = new();
        response.Action = TcpRequestConstants.RequestAuthorization;

        if (request.ClientId < 1 || request.ClientId > 99)
        {
            response.Message = "client id is outside range (1 - 99)";
            response.Status = TcpRequestConstants.StatusNotAccepted;
        }
        else if (clientManager.IsDuplicateClientId(request.ClientId))
        {
            response.Message = $"client id already exists ({request.ClientId})";
            response.Status = TcpRequestConstants.StatusNotAccepted;
        }
        else
        {
            clientData.IsAuthenticated = true;
            clientData.ClientId = request.ClientId;
            response.Message = "connection authorized";
            response.Status = TcpRequestConstants.StatusAccepted;
        }

        return response;
    }

    private Response HandleAccessRequest(TcpClientData clientData, AccessRequest? request)
    {
        Response response = new();
        response.Action = TcpRequestConstants.RequestAccess;

        if (request == null || request.CardId == null || request.Pin == null)
        {
            return HandleUnknownRequest();
        }

        if (!clientData.IsAuthenticated)
        {
            return HandleClientNotValidated();
        }

        UserDetailedData? user = databaseService.GetUserById(request.CardId);

        bool accessGranted = databaseService.ValidateUser(request.CardId, request.Pin);
        bool validTime = false;

        if (user != null)
        {
            validTime = user.StartValidityTime < request.Time && request.Time < user.EndValidityTime;
        }

        if (!accessGranted)
        {
            response.Status = TcpRequestConstants.StatusNotAccepted;
            response.Message = "card id and pin code not accepted";
        }
        else if (!validTime)
        {
            response.Status = TcpRequestConstants.StatusNotAccepted;
            response.Message = "card is invalid";
        }
        else
        {
            response.Status = TcpRequestConstants.StatusAccepted;
            response.Message = "card id and pin code accepted";
        }

        AccessLogData accessLogData = new()
        {
            CardId = request.CardId,
            DoorNumber = request.DoorNumber,
            Time = request.Time,
            AccessGranted = accessGranted
        };

        bool logged = databaseService.LogAccess(accessLogData);

        if (!logged)
        {
            response.Status = TcpRequestConstants.StatusFail;
            response.Message = "failed to log data to database";
        }

        return response;
    }

    private Response HandleAlarmReportRequest(TcpClientData clientData, AlarmReportRequest? request)
    {
        if (request == null)
        {
            return HandleUnknownRequest();
        }

        if (!clientData.IsAuthenticated)
        {
            return HandleClientNotValidated();
        }

        AlarmLogData alarm = new()
        {
            AlarmType = request.AlarmType,
            Time = request.Time,
            DoorNumber = clientData.ClientId
        };

        bool logged = databaseService.LogAlarm(alarm);

        Response response = new();
        if (!logged)
        {
            response.Status = TcpRequestConstants.StatusFail;
            response.Action = TcpRequestConstants.RequestAlarmReport;
            response.Message = "failed to log data to database";
        }
        else
        {
            response.Status = TcpRequestConstants.StatusAccepted;
            response.Action = TcpRequestConstants.RequestAlarmReport;
            response.Message = "logged data to database";
        }

        return response;
    }

    private static Response HandleClientNotValidated()
    {
        return new(TcpRequestConstants.RequestInvalid, TcpRequestConstants.StatusNotAccepted, "client is not validated");
    }

    private static Response HandleUnknownRequest()
    {
        return new(TcpRequestConstants.RequestInvalid, TcpRequestConstants.StatusFail, "request does not match any known patterns");
    }

}
