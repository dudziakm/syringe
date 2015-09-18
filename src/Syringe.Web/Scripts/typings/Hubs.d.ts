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
      * The hub implemented by Syringe.Web.Hubs.ProgressHub
      */
    progressHub : ProgressHub;
}
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region ProgressHub hub

interface ProgressHub {
    
    /**
      * This property lets you send messages to the ProgressHub hub.
      */
    server : ProgressHubServer;

    /**
      * The functions on this property should be replaced if you want to receive messages from the ProgressHub hub.
      */
    client : IProgressHubClient;
}

interface ProgressHubServer {

    /** 
      * Sends a "startMonitoringProgress" message to the ProgressHub hub.
      * Contract Documentation: ---
      * @param taskId {number} 
      * @return {JQueryPromise of void}
      */
    startMonitoringProgress(taskId : number) : JQueryPromise<void>
}

interface IProgressHubClient
{

    /**
      * Set this function with a "function(){}" to receive the "doSomething" message from the ProgressHub hub.
      * Contract Documentation: ---
      * @return {void}
      */
    doSomething : () => void;
}

//#endregion ProgressHub hub

//#endregion service contracts



////////////////////
// Data Contracts //
////////////////////
//#region data contracts

//#endregion data contracts

