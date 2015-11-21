$(document).ready(function () {
    var rowsToAdd = [
        { $Button: $("#addVerification"), URL: "/TestCase/AddVerification", Prefix: "Verifications" },
        { $Button: $("#addParsedItem"), URL: "/TestCase/AddParseResponseItem", Prefix: "ParseResponses" },
        { $Button: $("#addHeaderItem"), URL: "/TestCase/AddHeaderItem", Prefix: "Headers" },
        { $Button: $("#addVariableItem"), URL: "/TestFile/AddVariableItem", Prefix: "Variables" }
    ];
    var rowHandler = new RowHandler(rowsToAdd);
    rowHandler.setupButtons();
    $("body").on("click", "#removeRow", function (e) {
        e.preventDefault();
        $(this).closest(".form-group").remove();
    });
});
/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var Progress = (function () {
            function Progress(signalRUrl) {
                this.signalRUrl = signalRUrl;
            }
            Progress.prototype.monitor = function (taskId) {
                var _this = this;
                if (taskId === 0) {
                    throw Error("Task ID was 0.");
                }
                $.connection.hub.logging = true;
                $.connection.hub.url = this.signalRUrl;
                this.proxy = $.connection.taskMonitorHub;
                this.proxy.client.onTaskCompleted = function (taskInfo) {
                    ++_this.completedCases;
                    console.log("Completed task " + taskInfo.CaseId + " (" + _this.completedCases + " of " + _this.totalCases + ").");
                    if (_this.totalCases > 0) {
                        var percentage = (_this.completedCases / _this.totalCases) * 100;
                        $(".progress-bar").css("width", percentage + "%");
                        $(".progress-bar .sr-only").text(percentage + "% Complete");
                    }
                    var selector = "#case-" + taskInfo.CaseId;
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
                    .done(function () {
                    _this.totalCases = 0;
                    _this.completedCases = 0;
                    _this.proxy.server.startMonitoringTask(taskId)
                        .done(function (taskState) {
                        _this.totalCases = taskState.TotalCases;
                        console.log("Started monitoring task " + taskId + ". There are " + taskState.TotalCases + " cases.");
                    });
                });
            };
            return Progress;
        })();
        Web.Progress = Progress;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
/// <reference path="../typings/jquery/jquery.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var TestCaseRunner = (function () {
            function TestCaseRunner() {
                this.intervalTime = 500;
                this._updatedIds = {};
            }
            TestCaseRunner.prototype.start = function (filename) {
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
            };
            TestCaseRunner.prototype.bindStopButton = function () {
                $("#stopbutton").click(function () {
                    clearTimeout(this._intervalHandle);
                });
            };
            TestCaseRunner.prototype.loadCases = function (filename) {
                $.get("/json/GetCases", { "filename": filename })
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
                        html += '				<div class="hidden case-result-html"><textarea style="display:none"></textarea></span>';
                        html += '			</div>';
                        html += "		</div>";
                        html += "	</div>";
                        html += "</div>";
                        $("#running-items").append(html);
                    });
                });
            };
            TestCaseRunner.prototype.updateProgress = function (taskId) {
                var that = this;
                $.get("/json/GetProgress", { "taskId": taskId })
                    .done(function (data) {
                    $.each(data.Results, function (index, item) {
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
                            cssClass = "panel-warning";
                        }
                        // Exceptions
                        if (item.ExceptionMessage !== null) {
                            cssClass = "panel-danger";
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
            };
            return TestCaseRunner;
        })();
        Web.TestCaseRunner = TestCaseRunner;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
var RowHandler = (function () {
    function RowHandler(i) {
        var _this = this;
        this.addRow = function (testCase) {
            $.get(testCase.URL, function (html) {
                var panelBody = testCase.$Button.parent().next();
                var formGroup = panelBody.find(".form-group:last-child"), rowNumber = 0;
                if (formGroup.length !== 0) {
                    var firstInputName = formGroup.find("input:first").attr("name");
                    // get the last index number of the row and increment it by 1
                    rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
                }
                // replace the name value with the correct prefix and row number so it can be posted to the server 
                var newHtml = html.replace(/name="/g, "name=\"" + testCase.Prefix + "[" + rowNumber + "].");
                panelBody.append(newHtml);
            });
        };
        this.getData = function (e) {
            e.preventDefault();
            _this.addRow(e.data.testCase);
        };
        this.setupButtons = function () {
            for (var i = 0; i < _this.rowsToAdd.length; i++) {
                var iTestCase = _this.rowsToAdd[i];
                iTestCase.$Button.on("click", { testCase: iTestCase }, _this.getData);
            }
        };
        this.rowsToAdd = i;
    }
    return RowHandler;
})();
var VerifyType;
(function (VerifyType) {
    VerifyType[VerifyType["Negative"] = 0] = "Negative";
    VerifyType[VerifyType["Positive"] = 1] = "Positive";
})(VerifyType || (VerifyType = {}));
/// <reference path="../typings/jquery/jquery.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var ViewResult = (function () {
            function ViewResult() {
                this.init();
            }
            ViewResult.prototype.init = function () {
                $("#tests-passed-count").on("click", function () {
                    $.each($(".test-failed"), function (count, item) {
                        $(item).hide(250);
                    });
                    $.each($(".test-passed"), function (count, item) {
                        $(item).show(400);
                    });
                });
                $("#all-tests-count").on("click", function () {
                    $.each($(".test-failed, .test-passed"), function (count, item) {
                        $(item).show(400);
                    });
                });
                $("#tests-failed-count").on("click", function () {
                    $.each($(".test-passed"), function (count, item) {
                        $(item).hide(250);
                    });
                    $.each($(".test-failed"), function (count, item) {
                        $(item).show(400);
                    });
                });
            };
            return ViewResult;
        })();
        Web.ViewResult = ViewResult;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
new Syringe.Web.ViewResult();
//# sourceMappingURL=Syringe.js.map