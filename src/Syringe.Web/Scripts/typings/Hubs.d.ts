// Get signalr.d.ts.ts from https://github.com/borisyankov/DefinitelyTyped (or delete the reference)
/// <reference path="signalr/signalr.d.ts" />
/// <reference path="jquery/jquery.d.ts" />

////////////////////
// available hubs //
////////////////////
//#region available hubs

interface SignalR
{

    /**
      * The hub implemented by Syringe.Service.Api.Hubs.TaskMonitorHub
      */
    taskMonitorHub : Syringe.Service.Api.Hubs.TaskMonitorHub;
}
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region TaskMonitorHub hub

declare module Syringe.Service.Api.Hubs {
interface TaskMonitorHub {
    
    /**
      * This property lets you send messages to the TaskMonitorHub hub.
      */
    server : Syringe.Service.Api.Hubs.TaskMonitorHubServer;

    /**
      * The functions on this property should be replaced if you want to receive messages from the TaskMonitorHub hub.
      */
    client : Syringe.Service.Api.Hubs.ITaskMonitorHubClient;
}

interface TaskMonitorHubServer {

    /** 
      * Sends a "startMonitoringTask" message to the TaskMonitorHub hub.
      * Contract Documentation: ---
      * @param taskId {number} 
      * @return {JQueryPromise of Syringe.Service.Api.Hubs.TaskState}
      */
    startMonitoringTask(taskId : number) : JQueryPromise<Syringe.Service.Api.Hubs.TaskState>;
}
} // end module

declare module Syringe.Service.Api.Hubs
{
interface ITaskMonitorHubClient
{

    /**
      * Set this function with a "function(taskInfo : Syringe.Service.Api.Hubs.CompletedTaskInfo){}" to receive the "onTaskCompleted" message from the TaskMonitorHub hub.
      * Contract Documentation: ---
      * @param taskInfo {Syringe.Service.Api.Hubs.CompletedTaskInfo} 
      * @return {void}
      */
    onTaskCompleted : (taskInfo : Syringe.Service.Api.Hubs.CompletedTaskInfo) => void;
}
}

//#endregion TaskMonitorHub hub

//#endregion service contracts



////////////////////
// Data Contracts //
////////////////////
//#region data contracts


/**
  * Data contract for Syringe.Service.Api.Hubs.CompletedTaskInfo
  */
declare module Syringe.Service.Api.Hubs {
interface CompletedTaskInfo {
    ActualUrl : string;
    ResultId : System.Guid;
    Success : boolean;
    HttpResponse : Syringe.Core.Http.HttpResponse;
    CaseId : number;
    ExceptionMessage : string;
}
} // end module


/**
  * Data contract for Syringe.Core.Http.HttpResponse
  */
declare module Syringe.Core.Http {
interface HttpResponse {
    StatusCode : System.Net.HttpStatusCode;
    Content : string;
    Headers : System.Collections.Generic.KeyValuePair_String_String_[];
    ResponseTime : System.TimeSpan;
}
} // end module


/**
  * Data contract for System.TimeSpan
  */
declare module System {
interface TimeSpan {
    Ticks : number;
    Days : number;
    Hours : number;
    Milliseconds : number;
    Minutes : number;
    Seconds : number;
    TotalDays : number;
    TotalHours : number;
    TotalMilliseconds : number;
    TotalMinutes : number;
    TotalSeconds : number;
}
} // end module


/**
  * Data contract for System.Collections.Generic.KeyValuePair`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
  */
declare module System.Collections.Generic {
interface KeyValuePair_String_String_ {
    Key : string;
    Value : string;
}
} // end module


/**
  * Data contract for System.Guid
  */
declare module System {
interface Guid {
}
} // end module


/**
  * Data contract for Syringe.Service.Api.Hubs.TaskState
  */
declare module Syringe.Service.Api.Hubs {
interface TaskState {
    TotalCases : number;
}
} // end module



/**
  * Data contract for System.Net.HttpStatusCode
  */
declare module System.Net {
enum HttpStatusCode {
    Continue = 100,
    SwitchingProtocols = 101,
    OK = 200,
    Created = 201,
    Accepted = 202,
    NonAuthoritativeInformation = 203,
    NoContent = 204,
    ResetContent = 205,
    PartialContent = 206,
    MultipleChoices = 300,
    Ambiguous = 300,
    MovedPermanently = 301,
    Moved = 301,
    Found = 302,
    Redirect = 302,
    SeeOther = 303,
    RedirectMethod = 303,
    NotModified = 304,
    UseProxy = 305,
    Unused = 306,
    TemporaryRedirect = 307,
    RedirectKeepVerb = 307,
    BadRequest = 400,
    Unauthorized = 401,
    PaymentRequired = 402,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    NotAcceptable = 406,
    ProxyAuthenticationRequired = 407,
    RequestTimeout = 408,
    Conflict = 409,
    Gone = 410,
    LengthRequired = 411,
    PreconditionFailed = 412,
    RequestEntityTooLarge = 413,
    RequestUriTooLong = 414,
    UnsupportedMediaType = 415,
    RequestedRangeNotSatisfiable = 416,
    ExpectationFailed = 417,
    UpgradeRequired = 426,
    InternalServerError = 500,
    NotImplemented = 501,
    BadGateway = 502,
    ServiceUnavailable = 503,
    GatewayTimeout = 504,
    HttpVersionNotSupported = 505,
}
} // end module

//#endregion data contracts

