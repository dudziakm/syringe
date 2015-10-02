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
    taskMonitorHub : TaskMonitorHub;
}
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region TaskMonitorHub hub

interface TaskMonitorHub {
    
    /**
      * This property lets you send messages to the TaskMonitorHub hub.
      */
    server : TaskMonitorHubServer;

    /**
      * The functions on this property should be replaced if you want to receive messages from the TaskMonitorHub hub.
      */
    client : ITaskMonitorHubClient;
}

interface TaskMonitorHubServer {

    /** 
      * Sends a "startMonitoringTask" message to the TaskMonitorHub hub.
      * Contract Documentation: ---
      * @param taskId {number} 
      * @return {JQueryPromise of void}
      */
    startMonitoringTask(taskId : number) : JQueryPromise<void>
}

interface ITaskMonitorHubClient
{

    /**
      * Set this function with a "function(taskId : number){}" to receive the "onProgressUpdated" message from the TaskMonitorHub hub.
      * Contract Documentation: ---
      * @param taskId {number} 
      * @return {void}
      */
    onProgressUpdated : (taskId : number) => void;
}

//#endregion TaskMonitorHub hub

//#endregion service contracts



////////////////////
// Data Contracts //
////////////////////
//#region data contracts

//#endregion data contracts

