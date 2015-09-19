(function () {
    var jQueryElements = {
        addVerificationButton: $("#addVerification"),
        verificationDescription: $("#verificationDescription"),
        verificationRegex: $("#verificationRegex"),
        verificationType: $("#verificationType"),
        addParsedItemButton: $("#addParsedItem"),
        parseDescription: $("#parseDescription"),
        parseRegex: $("#parseRegex"),
        addHeaderItemButton: $("#addHeaderItem"),
        headerKey: $("#headerKey"),
        headerValue: $("#headerValue")
    };
    var elements = {
        removeRow: "#removeRow",
        formGroup: ".form-group"
    };
    function setupButtons() {
        jQueryElements.addVerificationButton.click(function (e) {
            e.preventDefault();
            var verificationItem = {
                Description: jQueryElements.verificationDescription.val(),
                Regex: jQueryElements.verificationRegex.val(),
                VerifyTypeValue: jQueryElements.verificationType.val()
            };
            $.get("/TestCase/AddVerification", verificationItem, function (data) {
                appendDataItem(jQueryElements.addVerificationButton, data, "Verifications");
                jQueryElements.verificationDescription.val('');
                jQueryElements.verificationRegex.val('');
            });
        });
        jQueryElements.addParsedItemButton.click(function (e) {
            e.preventDefault();
            var parseResponseItem = {
                Description: jQueryElements.parseDescription.val(),
                Regex: jQueryElements.parseRegex.val(),
            };
            $.get("/TestCase/AddParseResponseItem", parseResponseItem, function (data) {
                appendDataItem(jQueryElements.addParsedItemButton, data, "ParseResponses");
                jQueryElements.parseDescription.val('');
                jQueryElements.parseRegex.val('');
            });
        });
        jQueryElements.addHeaderItemButton.click(function (e) {
            e.preventDefault();
            var headerItem = {
                Key: jQueryElements.headerKey.val(),
                Value: jQueryElements.headerValue.val(),
            };
            $.get("/TestCase/AddHeaderItem", headerItem, function (data) {
                appendDataItem(jQueryElements.addHeaderItemButton, data, "Headers");
                jQueryElements.headerKey.val('');
                jQueryElements.headerValue.val('');
            });
        });
        $("body").on("click", elements.removeRow, function (e) {
            e.preventDefault();
            $(this).closest(elements.formGroup).remove();
        });
    }
    function appendDataItem(element, data, elementPrefix) {
        var currentRow = element.closest(elements.formGroup);
        var rowNumber = 0;
        var previousRow = currentRow.prev();
        //check if previous row exists then increase number
        if (previousRow.hasClass("form-group")) {
            var firstInputName = previousRow.find("input:first").attr("name");
            //get the last index number of the row and increment it by 1
            rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
        }
        var newData = data.replace(/name="/g, 'name="' + elementPrefix + '[' + rowNumber + '].');
        currentRow.before(newData);
    }
    $(document).ready(function () {
        setupButtons();
    });
}());
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
                        alert("An error occured - taskid was 0");
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