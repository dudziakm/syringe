/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
module Syringe.Web
{
	export class Progress
	{
		private proxy: Service.Api.Hubs.TaskMonitorHub;
		private signalRUrl: string;
		private totalCases: number;
		private completedCases: number;

		constructor(signalRUrl: string)
		{
			this.signalRUrl = signalRUrl;
		}

		monitor(taskId: number)
		{
			if (taskId === 0)
			{
				throw Error("Task ID was 0.");
			}

			$.connection.hub.logging = true;
			$.connection.hub.url = this.signalRUrl;

			this.proxy = $.connection.taskMonitorHub;

			this.proxy.client.onTaskCompleted = (taskInfo: Service.Api.Hubs.CompletedTaskInfo) =>
			{
				++this.completedCases;

				console.log(`Completed task ${taskInfo.CaseId} (${this.completedCases} of ${this.totalCases}).`);

				if (this.totalCases > 0)
				{
					var percentage = (this.completedCases / this.totalCases) * 100;
					$(".progress-bar").css("width", percentage + "%");
				}

				var selector = `#case-${taskInfo.CaseId}`;
				var $selector = $(selector);

				// Change background color
				var resultClass = taskInfo.Success ? "panel-success" : "panel-warning";

				// Exceptions
				if (taskInfo.ExceptionMessage !== null)
				{
					resultClass = "panel-danger";
					$(`${selector} .case-result-exception`).removeClass("hidden");
					$(`${selector} .case-result-exception textarea`).text(taskInfo.ExceptionMessage);
				}

				$selector.addClass(resultClass);
			};

			$.connection.hub.start()
				.done(() =>
				{
					this.totalCases = 0;
					this.completedCases = 0;
					this.proxy.server.startMonitoringTask(taskId)
						.done(taskState =>
						{
							this.totalCases = taskState.TotalCases;
							console.log(`Started monitoring task ${taskId}. There are ${taskState.TotalCases} cases.`);
						});
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