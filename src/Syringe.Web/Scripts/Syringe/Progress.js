/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var Progress = (function () {
            function Progress() {
                this._updatedIds = {};
            }
            Progress.prototype.monitor = function (taskId) {
                if (taskId === 0) {
                    throw Error("Task ID was 0.");
                }
                var self = this;
                $.connection.hub.logging = true;
                $.connection.hub.url = "/signalr";
                this.proxy = $.connection.progressHub;
                this.proxy.client.doSomething = function (d) {
                    alert("I did a thing");
                };
                $.connection.hub.start().done(function () {
                    self.proxy.server.startMonitoringProgress(taskId);
                });
            };
            return Progress;
        })();
        Web.Progress = Progress;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
//# sourceMappingURL=Progress.js.map