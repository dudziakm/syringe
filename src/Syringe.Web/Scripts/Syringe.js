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
            TestCaseRunner.prototype.start = function (taskId) {
                if (taskId === 0) {
                    throw Error("Task ID was 0.");
                }
                this.bindStopButton();
                var that = this;
                that.intervalHandle = setInterval(function () {
                    that.updateProgress(taskId);
                }, that.intervalTime);
            };
            TestCaseRunner.prototype.bindStopButton = function () {
                var self = this;
                $("#stopbutton").click(function () {
                    clearTimeout(self.intervalHandle);
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
//# sourceMappingURL=Syringe.js.map