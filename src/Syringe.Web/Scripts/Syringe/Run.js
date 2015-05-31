/// <reference path="../typings/jquery/jquery.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var TestCaseRunner = (function () {
            function TestCaseRunner() {
                this.intervalTime = 25; // needs a very low number for exceptions.
            }
            TestCaseRunner.prototype.start = function (filename) {
                this.bindStopButton();
                this.loadCases(filename);
                var that = this;
                $.post("/json/run", { filename: filename }).done(function (data) {
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
                $.get("/json/GetCases", { "filename": filename }).done(function (data) {
                    $.each(data.TestCases, function (index, item) {
                        var html = "<div style='border:1px solid black' id='case" + item.Id + "'>";
                        html += item.Id + " - " + item.ShortDescription;
                        html += "</div>";
                        $("#items").append(html);
                    });
                });
            };
            TestCaseRunner.prototype.updateProgress = function (taskId) {
                var that = this;
                $.get("/json/GetProgress", { "taskId": taskId }).done(function (data) {
                    // New case has started, set the colour of the last one.
                    if (data.LastResult !== null && that.lastCaseId !== data.LastResult.TestCase.Id) {
                        var backgroundColor = "green";
                        if (data.LastResult.ExceptionMessage !== null)
                            backgroundColor = "red";
                        $("#case" + data.LastResult.TestCase.Id).css("background-color", backgroundColor);
                        that.lastCaseId = data.LastResult.TestCase.Id;
                    }
                    var percentage = (data.Count / data.TotalCases) * 100;
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
//# sourceMappingURL=Run.js.map