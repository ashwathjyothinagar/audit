var App;
(function (App) {

    var HourlyTaskReportController = (function () {
        function HourlyTaskReportController($log, $http, BASE_URI, ReportLookupService, $location) {
            var _this = this;

            _this.CrnId = 0;
            _this.Crns = [];
            _this.HourlyTaskdOrders = [];
            _this.headerColumns = [];

            this.init = function () {

                this.getCrns().then(function (response) {
                    _this.Crns = response.data;
                });

                this.hours = ReportLookupService.getHours();
                this.minutes = ReportLookupService.getMinutes();
                this.periods = ReportLookupService.getPeriods();

                this.startDateHour = "08";
                this.startDateMinute = "00";

                this.endDateHour = "18";
                this.endDateMinute = "00";
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
                    url: BASE_URI + "Audits/GetHourlyTaskdOrders?startDateTime=" + ReportLookupService.getISODate(_this.startDate) + "T" + this.startDateHour + ":" + this.startDateMinute + "&endDateTime=" + ReportLookupService.getISODate(_this.endDate) + "T" + this.endDateHour + ":" + this.endDateMinute + "&location=" + ($("#location").val() == "" ? "ALL" : $("#location").val() + "&taskId=" + (this.taskId ? this.taskId : ""))
                        + "&crnId=" + ($("#CrnId").val() == "" ? "0" : $("#CrnId").val()) + "&groupName=" + ($("#groupName").val() == "" ? "" : $("#groupName").val()) + "&selectedOffices=" + ($("#SelectedOffices").val() == "" ? "" : $("#SelectedOffices").val())
                }

                $http(req).then(function (response) {
                    _this.HourlyTaskdOrders = JSON.parse(response.data);

                    _this.headerColumns = [];
                    if (_this.HourlyTaskdOrders && _this.HourlyTaskdOrders.length > 0) {
                        for (var key in _this.HourlyTaskdOrders[0]) {
                            _this.headerColumns.push(key);
                        }
                    }
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
        return HourlyTaskReportController;
    }());
    HourlyTaskReportController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService',
        '$location'
    ];


    angular
        .module('app')
        .controller('HourlyTaskReportController', HourlyTaskReportController);
})(App || (App = {}));
