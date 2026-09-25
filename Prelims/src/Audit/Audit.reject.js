var App;
(function (App) {

    var AuditRejectController = (function () {
        function AuditRejectController($log, $http, BASE_URI, ReportLookupService) {
            var _this = this;

            _this.rejectReasons = [];
            this.init = function () {

                this.getRejectReasons().then(function (response) {
                    _this.rejectReasons = response.data;
                });
            };

            this.getRejectReasons = function () {
                var req = {
                    method: 'GET',
                    url: BASE_URI + "AuditRejectReasons/GetAll"
                }

                return $http(req)
            };

            this.onReasonChange = function () {
                this.rejectReason = this.drpReason;
            }

            this.init();
        }
        return AuditRejectController;
    }());
    AuditRejectController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService'
    ];


    angular
        .module('app')
        .controller('AuditRejectController', AuditRejectController);
})(App || (App = {}));