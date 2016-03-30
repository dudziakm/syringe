/// <reference path="../typings/jquery/jquery.d.ts" />

module Syringe.Web {
    export class TestCaseRunner {
        private intervalHandle: any;
        private lastCaseId: number;
        private intervalTime = 500;

        start(filename: string) {
            this.bindStopButton();
            this.loadCases(filename);

            var that = this;
            $.post("/json/run", { filename: filename })
                .done(function (data) {
                    if (data.taskId === 0) {
                        alert("An error occurred - taskid was 0");
                        return;
                    }

                    that.intervalHandle = setInterval(function () {
                        that.updateProgress(data.taskId);
                    }, that.intervalTime);
                });
        }

        private bindStopButton() {
            $("#stopbutton").click(function () {
                clearTimeout(this._intervalHandle);
            });
        }

        private loadCases(filename: string) {
            $.get("/json/GetTests", { "filename": filename })
                .done(function (data) {
                    $.each(data.TestCases, function (index, item) {
                        var html = "";
                        html = '<div class="panel" id="case-' + item.Id + '">';
                        html += '	<div class="panel-heading"><h3 class="panel-title">' + item.Id + " - " + item.ShortDescription + "</h3></div>";
                        html += '		<div class="panel-body">';
                        html += '			<div>';
                        html += '				<div class="pull-left case-result-url"></div>';
                        html += '				<div class="pull-right">';
                        html += '					<a class="view-html btn btn-primary" href="#">View HTML</a>';
                        html += '					<a class="view-raw btn btn-primary" href="#">View raw</a>';
                        html += '				</div>';
                        html += '			</div>';
                        html += '			<div class="case-result-errors">';
                        html += '				<div class="hidden case-result-exception"><h2 class="label label-danger">Error</h4><textarea></textarea></div>';
                        html += '			</div>';
                        html += "		</div>";
                        html += "	</div>";
                        html += "</div>";

                        $("#running-items").append(html);
                    });
                });
        }

        _updatedIds = {};

        updateProgress(taskId) {
            var that = this;

            $.get("/json/GetProgress", { "taskId": taskId })
                .done(function (data) {
                    $.each(data.Results, function (index, item: TestCaseResult) {
                        var selector = "#case-" + item.TestCase.Id;

                        if (that._updatedIds[selector]) {
                            return;
                        }

                        that._updatedIds[selector] = true;

                        var cssClass = "";
                        var iframeTextArea = selector + " .case-result-html textarea";

                        // Url
                        var urlSelector = selector + " " + ".case-result-url";
                        $(urlSelector).text(item.ActualUrl);

                        // Add HTML into the hidden iframe
                        if (item.HttpResponse != null && $(iframeTextArea).text() === "") {
                            $(iframeTextArea).text(item.HttpResponse.Content);
                        }

                        $(selector + " a.view-html").click(function () {
                            var newWindow = window.open("", item.TestCase.Id.toString());
                            newWindow.document.write($(iframeTextArea).text());
                            $(newWindow.document).find("head").append('<base href="' + item.ActualUrl + '" />');
                        });

                        $(selector + " a.view-raw").click(function () {
                            var newWindow = window.open("", "plaintext-" + item.TestCase.Id.toString());
                            newWindow.document.write("<PLAINTEXT>" + $(iframeTextArea).text());
                        });

	
                        // Change background color
                        if (item.Success === true) {
                            cssClass = "panel-success";
                        }
                        else if (item.Success === false) {
                            cssClass = "panel-danger";
                        }

                        // Exceptions
                        if (item.ExceptionMessage !== null) {
                            $(selector + " .case-result-exception").removeClass("hidden");
                            $(selector + " .case-result-exception textarea").text(item.ExceptionMessage);
                        }

                        $(selector).addClass(cssClass);
                    });

                    var percentage = (data.CurrentIndex / data.TotalCases) * 100;
                    $(".progress-bar").css("width", percentage + "%");
                    $("#progress-text").html(data.Status);

                    if (data.Status === "RanToCompletion") {
                        clearTimeout(that.intervalHandle);
                        console.log("stopped");
                        return;
                    }
                });
        }
    }
}