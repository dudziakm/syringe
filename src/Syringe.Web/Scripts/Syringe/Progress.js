/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/signalr/signalr.d.ts" />
/// <reference path="../typings/Hubs.d.ts" />
var Syringe;
(function (Syringe) {
    var Web;
    (function (Web) {
        var Progress = (function () {
            function Progress(signalRUrl) {
                this._updatedIds = {};
                this.signalRUrl = signalRUrl;
            }
            Progress.prototype.monitor = function (taskId) {
                if (taskId === 0) {
                    throw Error("Task ID was 0.");
                }
                var self = this;
                $.connection.hub.logging = true;
                $.connection.hub.url = this.signalRUrl;
                this.proxy = $.connection.taskMonitorHub;
                this.proxy.client.onProgressUpdated = function (d) {
                    alert("I did a thing");
                };
                $.connection.hub.start({ jsonp: true }).done(function () {
                    self.proxy.server.startMonitoringTask(taskId);
                });
            };
            return Progress;
        })();
        Web.Progress = Progress;
    })(Web = Syringe.Web || (Syringe.Web = {}));
})(Syringe || (Syringe = {}));
//# sourceMappingURL=Progress.js.map