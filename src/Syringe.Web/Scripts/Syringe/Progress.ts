/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
module Syringe.Web {
    export class Progress {
        private proxy: Service.Api.Hubs.TaskMonitorHub;
        private signalRUrl: string;
        private totalCases: number;
        private completedCases: number;

        constructor(signalRUrl: string) {
            this.signalRUrl = signalRUrl;
        }

        monitor(taskId: number) {
            if (taskId === 0) {
                throw Error("Task ID was 0.");
            }

            $.connection.hub.logging = true;
            $.connection.hub.url = this.signalRUrl;

            this.proxy = $.connection.taskMonitorHub;

            this.proxy.client.onTaskCompleted = (taskInfo: Service.Api.Hubs.CompletedTaskInfo) => {
                ++this.completedCases;

                console.log(`Completed task ${taskInfo.CaseId} (${this.completedCases} of ${this.totalCases}).`);

                if (this.totalCases > 0) {
                    var percentage = (this.completedCases / this.totalCases) * 100;
                    $(".progress-bar").css("width", percentage + "%");
                    $(".progress-bar .sr-only").text(`${percentage}% Complete`);
                }

                var selector = `#case-${taskInfo.CaseId}`;
                var $selector = $(selector);

                // Url
                var $urlSelector = $(".case-result-url", $selector);
                $urlSelector.text(taskInfo.ActualUrl);

                // Change background color
                var resultClass = taskInfo.Success ? "panel-success" : "panel-warning";

                // Exceptions
                if (taskInfo.ExceptionMessage !== null) {
                    resultClass = "panel-danger";
                    $(".case-result-exception", $selector).removeClass("hidden");
                    $(".case-result-exception textarea", $selector).text(taskInfo.ExceptionMessage);
                    $("table tr.result-row", $selector).addClass("warning");
                }
                else {
                    // Show HTML/Raw buttons.
                    $(".view-html", $selector).removeClass("hidden");
                    $(".view-raw", $selector).removeClass("hidden");

                    for (var i = 0; i < taskInfo.Verifications.length; i++) {

                        var verificationItemClass = taskInfo.Verifications[i].Success ? "success" : "danger";

                        $("table tr.result-row:eq(" + i + ")", $selector).addClass(verificationItemClass);
                    }
                }

               

                $selector.addClass(resultClass);
            };

            $.connection.hub.start()
                .done(() => {
                    this.totalCases = 0;
                    this.completedCases = 0;
                    this.proxy.server.startMonitoringTask(taskId)
                        .done(taskState => {
                            this.totalCases = taskState.TotalCases;
                            console.log(`Started monitoring task ${taskId}. There are ${taskState.TotalCases} cases.`);
                        });
                });
        }
    }
}