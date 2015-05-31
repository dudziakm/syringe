/// <reference path="../typings/jquery/jquery.d.ts" />

module Syringe.Web
{
	export class TestCaseRunner
	{
		private intervalHandle : any;
		private lastCaseId: number;
		private intervalTime = 25; // needs a very low number for exceptions.

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
						var html = "<div style='border:1px solid black' id='case" +item.Id+"'>";
						html += item.Id + " - " +item.ShortDescription;
						html += "</div>";

						$("#items").append(html);
					});
				});
		}

		updateProgress(taskId)
		{
			var that = this;

			$.get("/json/GetProgress", { "taskId": taskId })
				.done(function(data)
				{
					// New case has started, set the colour of the last one.
					if (data.LastResult !== null && that.lastCaseId !== data.LastResult.TestCase.Id)
					{
						var backgroundColor = "green";

						if (data.LastResult.ExceptionMessage !== null)
							backgroundColor = "red";

						$("#case" + data.LastResult.TestCase.Id).css("background-color", backgroundColor);
						that.lastCaseId = data.LastResult.TestCase.Id;
					}

					var percentage = (data.Count / data.TotalCases) * 100;
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