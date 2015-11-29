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
    HttpResponseInfo : Syringe.Core.Http.HttpResponseInfo;
    CaseId : System.Guid;
    ExceptionMessage : string;
    Verifications : Syringe.Core.TestCases.VerificationItem[];
}
} // end module


/**
  * Data contract for Syringe.Core.TestCases.VerificationItem
  */
declare module Syringe.Core.TestCases {
interface VerificationItem {
    Description : string;
    Regex : string;
    TransformedRegex : string;
    Success : boolean;
    VerifyType : Syringe.Core.TestCases.VerifyType;
}
} // end module


/**
  * Data contract for Syringe.Core.Http.HttpResponseInfo
  */
declare module Syringe.Core.Http {
interface HttpResponseInfo {
    Response : RestSharp.IRestResponse;
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
  * Data contract for RestSharp.IRestResponse
  */
declare module RestSharp {
interface IRestResponse {
    Request : RestSharp.IRestRequest;
    ContentType : string;
    ContentLength : number;
    ContentEncoding : string;
    Content : string;
    StatusCode : System.Net.HttpStatusCode;
    StatusDescription : string;
    RawBytes : System.Byte[];
    ResponseUri : System.Uri;
    Server : string;
    Cookies : System.Collections.Generic.IList_RestResponseCookie_;
    Headers : System.Collections.Generic.IList_Parameter_;
    ResponseStatus : RestSharp.ResponseStatus;
    ErrorMessage : string;
    ErrorException : System.Exception;
}
} // end module


/**
  * Data contract for System.Exception
  */
declare module System {
interface Exception {
    Message : string;
    Data : System.Collections.IDictionary;
    InnerException : System.Exception;
    TargetSite : System.Reflection.MethodBase;
    StackTrace : string;
    HelpLink : string;
    Source : string;
    HResult : number;
}
} // end module


/**
  * Data contract for System.Reflection.MethodBase
  */
declare module System.Reflection {
interface MethodBase {
    MethodImplementationFlags : System.Reflection.MethodImplAttributes;
    MethodHandle : System.RuntimeMethodHandle;
    Attributes : System.Reflection.MethodAttributes;
    CallingConvention : System.Reflection.CallingConventions;
    IsGenericMethodDefinition : boolean;
    ContainsGenericParameters : boolean;
    IsGenericMethod : boolean;
    IsSecurityCritical : boolean;
    IsSecuritySafeCritical : boolean;
    IsSecurityTransparent : boolean;
    IsPublic : boolean;
    IsPrivate : boolean;
    IsFamily : boolean;
    IsAssembly : boolean;
    IsFamilyAndAssembly : boolean;
    IsFamilyOrAssembly : boolean;
    IsStatic : boolean;
    IsFinal : boolean;
    IsVirtual : boolean;
    IsHideBySig : boolean;
    IsAbstract : boolean;
    IsSpecialName : boolean;
    IsConstructor : boolean;
}
} // end module


/**
  * Data contract for System.RuntimeMethodHandle
  */
declare module System {
interface RuntimeMethodHandle {
    Value : System.IntPtr;
}
} // end module


/**
  * Data contract for System.IntPtr
  */
declare module System {
interface IntPtr {
}
} // end module


/**
  * Data contract for System.Collections.IDictionary
  */
declare module System.Collections {
interface IDictionary {
    Item : System.Object;
    Keys : System.Collections.ICollection;
    Values : System.Collections.ICollection;
    IsReadOnly : boolean;
    IsFixedSize : boolean;
}
} // end module


/**
  * Data contract for System.Collections.ICollection
  */
declare module System.Collections {
interface ICollection {
    Count : number;
    SyncRoot : System.Object;
    IsSynchronized : boolean;
}
} // end module


/**
  * Data contract for System.Object
  */
declare module System {
interface Object {
}
} // end module


/**
  * Data contract for System.Collections.Generic.IList`1[[RestSharp.Parameter, RestSharp, Version=105.1.0.0, Culture=neutral, PublicKeyToken=null]]
  */
declare module System.Collections.Generic {
interface IList_Parameter_ {
    Item : RestSharp.Parameter;
}
} // end module


/**
  * Data contract for RestSharp.Parameter
  */
declare module RestSharp {
interface Parameter {
    Name : string;
    Value : System.Object;
    Type : RestSharp.ParameterType;
}
} // end module


/**
  * Data contract for System.Collections.Generic.IList`1[[RestSharp.RestResponseCookie, RestSharp, Version=105.1.0.0, Culture=neutral, PublicKeyToken=null]]
  */
declare module System.Collections.Generic {
interface IList_RestResponseCookie_ {
    Item : RestSharp.RestResponseCookie;
}
} // end module


/**
  * Data contract for RestSharp.RestResponseCookie
  */
declare module RestSharp {
interface RestResponseCookie {
    Comment : string;
    CommentUri : System.Uri;
    Discard : boolean;
    Domain : string;
    Expired : boolean;
    Expires : Date;
    HttpOnly : boolean;
    Name : string;
    Path : string;
    Port : string;
    Secure : boolean;
    TimeStamp : Date;
    Value : string;
    Version : number;
}
} // end module


/**
  * Data contract for System.Uri
  */
declare module System {
interface Uri {
    AbsolutePath : string;
    AbsoluteUri : string;
    LocalPath : string;
    Authority : string;
    HostNameType : System.UriHostNameType;
    IsDefaultPort : boolean;
    IsFile : boolean;
    IsLoopback : boolean;
    PathAndQuery : string;
    Segments : string[];
    IsUnc : boolean;
    Host : string;
    Port : number;
    Query : string;
    Fragment : string;
    Scheme : string;
    OriginalString : string;
    DnsSafeHost : string;
    IdnHost : string;
    IsAbsoluteUri : boolean;
    UserEscaped : boolean;
    UserInfo : string;
}
} // end module


/**
  * Data contract for System.Byte
  */
declare module System {
interface Byte {
}
} // end module


/**
  * Data contract for RestSharp.IRestRequest
  */
declare module RestSharp {
interface IRestRequest {
    AlwaysMultipartFormData : boolean;
    JsonSerializer : RestSharp.Serializers.ISerializer;
    XmlSerializer : RestSharp.Serializers.ISerializer;
    ResponseWriter : System.Action_Stream_;
    Parameters : RestSharp.Parameter[];
    Files : RestSharp.FileParameter[];
    Method : RestSharp.Method;
    Resource : string;
    RequestFormat : RestSharp.DataFormat;
    RootElement : string;
    DateFormat : string;
    XmlNamespace : string;
    Credentials : System.Net.ICredentials;
    Timeout : number;
    ReadWriteTimeout : number;
    Attempts : number;
    UseDefaultCredentials : boolean;
    OnBeforeDeserialization : System.Action_IRestResponse_;
}
} // end module


/**
  * Data contract for System.Action`1[[RestSharp.IRestResponse, RestSharp, Version=105.1.0.0, Culture=neutral, PublicKeyToken=null]]
  */
declare module System {
interface Action_IRestResponse_ {
}
} // end module


/**
  * Data contract for System.Net.ICredentials
  */
declare module System.Net {
interface ICredentials {
}
} // end module


/**
  * Data contract for RestSharp.FileParameter
  */
declare module RestSharp {
interface FileParameter {
    ContentLength : number;
    Writer : System.Action_Stream_;
    FileName : string;
    ContentType : string;
    Name : string;
}
} // end module


/**
  * Data contract for System.Action`1[[System.IO.Stream, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
  */
declare module System {
interface Action_Stream_ {
}
} // end module


/**
  * Data contract for RestSharp.Serializers.ISerializer
  */
declare module RestSharp.Serializers {
interface ISerializer {
    RootElement : string;
    Namespace : string;
    DateFormat : string;
    ContentType : string;
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
  * Data contract for RestSharp.DataFormat
  */
declare module RestSharp {
enum DataFormat {
    Json = 0,
    Xml = 1,
}
} // end module


/**
  * Data contract for RestSharp.Method
  */
declare module RestSharp {
enum Method {
    GET = 0,
    POST = 1,
    PUT = 2,
    DELETE = 3,
    HEAD = 4,
    OPTIONS = 5,
    PATCH = 6,
    MERGE = 7,
}
} // end module


/**
  * Data contract for System.UriHostNameType
  */
declare module System {
enum UriHostNameType {
    Unknown = 0,
    Basic = 1,
    Dns = 2,
    IPv4 = 3,
    IPv6 = 4,
}
} // end module


/**
  * Data contract for RestSharp.ParameterType
  */
declare module RestSharp {
enum ParameterType {
    Cookie = 0,
    GetOrPost = 1,
    UrlSegment = 2,
    HttpHeader = 3,
    RequestBody = 4,
    QueryString = 5,
}
} // end module


/**
  * Data contract for System.Reflection.CallingConventions
  */
declare module System.Reflection {
enum CallingConventions {
    Standard = 1,
    VarArgs = 2,
    Any = 3,
    HasThis = 32,
    ExplicitThis = 64,
}
} // end module


/**
  * Data contract for System.Reflection.MethodAttributes
  */
declare module System.Reflection {
enum MethodAttributes {
    ReuseSlot = 0,
    PrivateScope = 0,
    Private = 1,
    FamANDAssem = 2,
    Assembly = 3,
    Family = 4,
    FamORAssem = 5,
    Public = 6,
    MemberAccessMask = 7,
    UnmanagedExport = 8,
    Static = 16,
    Final = 32,
    Virtual = 64,
    HideBySig = 128,
    NewSlot = 256,
    VtableLayoutMask = 256,
    CheckAccessOnOverride = 512,
    Abstract = 1024,
    SpecialName = 2048,
    RTSpecialName = 4096,
    PinvokeImpl = 8192,
    HasSecurity = 16384,
    RequireSecObject = 32768,
    ReservedMask = 53248,
}
} // end module


/**
  * Data contract for System.Reflection.MethodImplAttributes
  */
declare module System.Reflection {
enum MethodImplAttributes {
    Managed = 0,
    IL = 0,
    Native = 1,
    OPTIL = 2,
    Runtime = 3,
    CodeTypeMask = 3,
    Unmanaged = 4,
    ManagedMask = 4,
    NoInlining = 8,
    ForwardRef = 16,
    Synchronized = 32,
    NoOptimization = 64,
    PreserveSig = 128,
    AggressiveInlining = 256,
    InternalCall = 4096,
    MaxMethodImplVal = 65535,
}
} // end module


/**
  * Data contract for RestSharp.ResponseStatus
  */
declare module RestSharp {
enum ResponseStatus {
    None = 0,
    Completed = 1,
    Error = 2,
    TimedOut = 3,
    Aborted = 4,
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


/**
  * Data contract for Syringe.Core.TestCases.VerifyType
  */
declare module Syringe.Core.TestCases {
enum VerifyType {
    Negative = 0,
    Positive = 1,
}
} // end module

//#endregion data contracts

