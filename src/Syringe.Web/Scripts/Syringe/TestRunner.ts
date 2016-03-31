/// <reference path="../typings/jquery/jquery.d.ts" />

module Syringe.Web {
    export class TestRunner {
        private intervalHandle: any;
        private lastTestId: number;
        private intervalTime = 500;

        start(filename: string) {
            this.bindStopButton();
            this.loadTests(filename);

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

        private loadTests(filename: string) {
            $.get("/json/GetTests", { "filename": filename })
                .done(function (data: TestFile) {
                    $.each(data.Tests, function (index, item) {
                        var html = "";
                        html = '<div class="panel" id="test-' + item.Id + '">';
                        html += '	<div class="panel-heading"><h3 class="panel-title">' + item.Id + " - " + item.ShortDescription + "</h3></div>";
                        html += '		<div class="panel-body">';
                        html += '			<div>';
                        html += '				<div class="pull-left test-result-url"></div>';
                        html += '				<div class="pull-right">';
                        html += '					<a class="view-html btn btn-primary" href="#">View HTML</a>';
                        html += '					<a class="view-raw btn btn-primary" href="#">View raw</a>';
                        html += '				</div>';
                        html += '			</div>';
                        html += '			<div class="test-result-errors">';
                        html += '				<div class="hidden test-result-exception"><h2 class="label label-danger">Error</h4><textarea></textarea></div>';
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
                .done(function (data: TaskDetails) {
                    $.each(data.Results, function (index, item: TestResult) {
                        var selector = "#test-" + item.Test.Id;

                        if (that._updatedIds[selector]) {
                            return;
                        }

                        that._updatedIds[selector] = true;

                        var cssClass = "";
                        var iframeTextArea = selector + " .test-result-html textarea";

                        // Url
                        var urlSelector = selector + " " + ".test-result-url";
                        $(urlSelector).text(item.ActualUrl);

                        // Add HTML into the hidden iframe
                        if (item.HttpResponse != null && $(iframeTextArea).text() === "") {
                            $(iframeTextArea).text(item.HttpResponse.Content);
                        }

                        $(selector + " a.view-html").click(function () {
                            var newWindow = window.open("", item.Test.Id.toString());
                            newWindow.document.write($(iframeTextArea).text());
                            $(newWindow.document).find("head").append('<base href="' + item.ActualUrl + '" />');
                        });

                        $(selector + " a.view-raw").click(function () {
                            var newWindow = window.open("", "plaintext-" + item.Test.Id.toString());
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
                            $(selector + " .test-result-exception").removeClass("hidden");
                            $(selector + " .test-result-exception textarea").text(item.ExceptionMessage);
                        }

                        $(selector).addClass(cssClass);
                    });

                    var percentage = (data.CurrentIndex / data.TotalTests) * 100;
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