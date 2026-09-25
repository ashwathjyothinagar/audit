var App;
(function (App) {

    var CompleteReportController = (function () {
        function CompleteReportController($log, $http, BASE_URI, ReportLookupService, $location) {
            var _this = this;

            _this.CrnId = 0;
            _this.Crns = [];
            _this.CompletedOrders = [];
            this.init = function () {

                this.getCrns().then(function (response) {
                    _this.Crns = response.data;
                });

                this.hours = ReportLookupService.getHours();
                this.minutes = ReportLookupService.getMinutes();
                this.periods = ReportLookupService.getPeriods();

                this.startDateHour = "00";
                this.startDateMinute = "00";

                this.endDateHour = "23";
                this.endDateMinute = "59";
            };

            this.getCrns = function () {
                var req = {
                    method: 'GET',
                    url: BASE_URI + "api/Crns"
                }

                return $http(req)
            };

            this.init();

            this.loadReport = function () {

                var req = {
                    method: 'GET',
                    url: BASE_URI + "Audits/GetCompletedOrders?startDateTime=" + ReportLookupService.getISODate(_this.startDate) + "T" + this.startDateHour + ":" + this.startDateMinute + "&endDateTime=" + ReportLookupService.getISODate(_this.endDate) + "T" + this.endDateHour + ":" + this.endDateMinute
                }

                $http(req).then(function (response) {
                    _this.CompletedOrders = response.data;
                });
            }


            this.dateOptions = {
                dateDisabled: false,
                formatYear: 'yy',
                maxDate: new Date(2220, 5, 22),
                minDate: new Date(1920, 5, 22),
                startingDay: 1
            };

            this.altInputFormats = ['M!/d!/yyyy'];

            this.openEndPopup = function () {
                this.endOpened = true;
            }

            this.openStartPopup = function () {
                this.startOpened = true;
            }
        }
        return CompleteReportController;
    }());
    CompleteReportController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService',
        '$location'
    ];


    angular
        .module('app')
        .controller('CompleteReportController', CompleteReportController);
})(App || (App = {}));
