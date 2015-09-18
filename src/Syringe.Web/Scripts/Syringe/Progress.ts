/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />

module Syringe.Web
{
	export class Progress
    {
        private hub: any;

        private proxy: any;

        monitor(taskId: number) {
            $.connection.hub.logging = true;
            $.connection.hub.url = "/signalr";

            this.proxy = $.connection.progressHub;

            this.proxy.client.doSomething = function (d) {
                alert("I did a thing");
            };

            var self = this;
            $.connection.hub.start().done(function () {
                self.proxy.server.sendProgress();
            });

            //$.connection.hub.start().done(init);
			if (taskId === 0) {
				throw Error("Task ID was 0.");
            }
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