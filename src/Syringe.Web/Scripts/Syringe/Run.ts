/// <reference path="../typings/jquery/jquery.d.ts" />

module Syringe.Web
{
	export class TestCaseRunner
	{
		private intervalHandle : any;
		private lastCaseId: number;
		private intervalTime = 500;

		start(filename: string)
		{
			this.bindStopButton();
			this.loadCases(filename);

			var that = this;
			$.post("/json/run", { filename: filename })
				.done(function (data)
			{
				if (data.taskId === 0)
				{
					alert("An error occured - taskid was 0");
					return;
				}

				that.intervalHandle = setInterval(function()
				{
					that.updateProgress(data.taskId);
				}, that.intervalTime);
			});
		}

		private bindStopButton()
		{
			$("#stopbutton").click(function (){
				clearTimeout(this._intervalHandle);
			});
		}

		private loadCases(filename: string)
		{
			$.get("/json/GetCases", { "filename": filename })
				.done(function(data)
			{
					$.each(data.TestCases, function(index, item)
					{
						var html = '<div class="case-result panel" id="case-' +item.Id+'">';
						html += item.Id + " - " + item.ShortDescription;
						html += '<span class="case-result-url"></span>';
						html += "</div>";

						$("#running-items").append(html);
					});
				});
		}

		updateProgress(taskId)
		{
			var that = this;

			$.get("/json/GetProgress", { "taskId": taskId })
				.done(function(data)
				{
					$.each(data.Results, function(index, item)
					{
						var selector = "#case-" + item.TestCase.Id;
						var cssClass = "";

						if (item.Success === true)
							cssClass = "passed";
						else if (item.Success === false)
							cssClass = "failed";

						if (item.ExceptionMessage !== null)
							cssClass = "error";

						$(selector).addClass(cssClass);

						var urlSelector = selector + " " + ".case-result-url";
						$(urlSelector).text(" - "+item.ActualUrl);
					});

					var percentage = (data.CurrentIndex / data.TotalCases) * 100;
					$(".progress-bar").css("width", percentage + "%");
					$("#progress-text").html(data.Status);

					if (data.Status === "RanToCompletion")
					{
						clearTimeout(that.intervalHandle);
						console.log("stopped");
						return;
					}
				});
		}
	}
}