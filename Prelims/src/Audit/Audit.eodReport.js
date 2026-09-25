var App;
(function (App) {

    var EodReportController = (function () {
        function EodReportController($log, $http, BASE_URI, ReportLookupService) {
            var _this = this;

            _this.CrnId = 0;
            _this.Crns = [];
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
                var invisibleLink = document.createElement('a');
                var groupName = $('#groupName').val();
                var crnId = $('#CrnId').val();
                $(invisibleLink).attr("href", BASE_URI + "Audits/ExportEOD?startDateTime=" + ReportLookupService.getISODate(_this.startDate) + "T" + this.startDateHour + ":" + this.startDateMinute + "&endDateTime=" + ReportLookupService.getISODate(_this.endDate) + "T" + this.endDateHour + ":" + this.endDateMinute + "&groupName=" + groupName + "&crnId=" + crnId);
                document.body.appendChild(invisibleLink);
                invisibleLink.click();
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
        return EodReportController;
    }());
    EodReportController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService'
    ];


    angular
        .module('app')
        .controller('EodReportController', EodReportController);
})(App || (App = {}));