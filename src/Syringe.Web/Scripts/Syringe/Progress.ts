/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />

module Syringe.Web {
    export class Progress {
        private proxy: any;
        private signalRUrl: string;

        constructor(signalRUrl: string) {
            this.signalRUrl = signalRUrl;
        }

        monitor(taskId: number) {

            if (taskId === 0) {
                throw Error("Task ID was 0.");
            }

            var self = this;
            $.connection.hub.logging = true;
            $.connection.hub.url = this.signalRUrl;

            this.proxy = $.connection.taskMonitorHub;

            this.proxy.client.onProgressUpdated = function (d) {
                alert("I did a thing");
            };

            $.connection.hub.start({ jsonp: true }).done(function () {
                self.proxy.server.startMonitoringTask(taskId);
            });
        }

        _updatedIds = {};

        //updateProgress(taskId)
        //{
        //	var that = this;

        //	$.get("/json/GetProgress", { "taskId": taskId })
        //		.done(function (data)
        //	{
        //		$.each(data.Results, function (index, item: TestCaseResult)
        //		{
        //			var selector = "#case-" + item.TestCase.Id;

        //			if (that._updatedIds[selector])
        //			{
        //				return;
        //			}

        //			that._updatedIds[selector] = true;

        //			var cssClass = "";
        //			var iframeTextArea = selector + " .case-result-html textarea";

        //			// Url
        //			var urlSelector = selector + " " + ".case-result-url";
        //			$(urlSelector).text(item.ActualUrl);

        //			// Add HTML into the hidden iframe
        //			if (item.HttpResponse != null && $(iframeTextArea).text() === "")
        //			{
        //				$(iframeTextArea).text(item.HttpResponse.Content);
        //			}

        //			$(selector + " a.view-html").click(function ()
        //			{
        //				var newWindow = window.open("", item.TestCase.Id.toString());
        //				newWindow.document.write($(iframeTextArea).text());
        //				$(newWindow.document).find("head").append('<base href="' + item.ActualUrl + '" />');
        //			});

        //			$(selector + " a.view-raw").click(function ()
        //			{
        //				var newWindow = window.open("", "plaintext-" +item.TestCase.Id.toString());
        //				newWindow.document.write("<PLAINTEXT>" +$(iframeTextArea).text());
        //			});

	
        //			// Change background color
        //			if (item.Success === true)
        //			{
        //				cssClass = "panel-success";
        //			}
        //			else if (item.Success === false)
        //			{
        //				cssClass = "panel-warning";
        //			}

        //			// Exceptions
        //			if (item.ExceptionMessage !== null)
        //			{
        //				cssClass = "panel-danger";
        //				$(selector + " .case-result-exception").removeClass("hidden");
        //				$(selector + " .case-result-exception textarea").text(item.ExceptionMessage);
        //			}

        //			$(selector).addClass(cssClass);
        //		});

        //		var percentage = (data.CurrentIndex / data.TotalCases) * 100;
        //		$(".progress-bar").css("width", percentage + "%");
        //		$("#progress-text").html(data.Status);

        //		if (data.Status === "RanToCompletion")
        //		{
        //			clearTimeout(that.intervalHandle);
        //			console.log("stopped");
        //			return;
        //		}
        //	});
        //}
    }
}