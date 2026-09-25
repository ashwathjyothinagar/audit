var App;
(function (App) {

    var AuditHoldController = (function () {
        function AuditHoldController($log, $http, BASE_URI, ReportLookupService) {
            var _this = this;

            _this.holdReasons = [];
            this.init = function () {

                this.getHoldReasons().then(function (response) {
                    _this.holdReasons = response.data;
                });
            };

            this.getHoldReasons = function () {
                var req = {
                    method: 'GET',
                    url: BASE_URI + "AuditHoldReasons/GetAll"
                }

                return $http(req)
            };

            this.onReasonChange = function () {
                this.holdReason = this.drpReason;
            }

            this.init();
        }
        return AuditHoldController;
    }());
    AuditHoldController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService'
    ];


    angular
        .module('app')
        .controller('AuditHoldController', AuditHoldController);
})(App || (App = {}));