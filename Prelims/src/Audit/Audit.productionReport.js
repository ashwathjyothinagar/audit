var App;
(function (App) {

    var ProductionReportController = (function () {
        function ProductionReportController($log, $http, BASE_URI, ReportLookupService) {
            var _this = this;

            _this.CrnId = 0;
            _this.tasks = [{ "Id": -1, "Name": "ALL" }];
            this.init = function () {

                this.getTasks().then(function (response) {
                    _.forEach(response.data, function (taskItem) {
                        _this.tasks.push(taskItem);
                    });

                    _this.taskId = "-1";
                });

                this.hours = ReportLookupService.getHours();
                this.minutes = ReportLookupService.getMinutes();
                this.periods = ReportLookupService.getPeriods();

                this.startDateHour = "00";
                this.startDateMinute = "00";

                this.endDateHour = "23";
                this.endDateMinute = "59";
            };

            this.getTasks = function () {
                var req = {
                    method: 'GET',
                    url: BASE_URI + "Audits/GetAllTasks"
                }

                return $http(req)
            };

            this.init();

            this.loadReport = function () {
                var invisibleLink = document.createElement('a');
                $(invisibleLink).attr("href", BASE_URI + "Audits/ExportProductionReport?taskId=" + _this.taskId + "&startDateTime=" + ReportLookupService.getISODate(_this.startDate) + "T" + this.startDateHour + ":" + this.startDateMinute + "&endDateTime=" + ReportLookupService.getISODate(_this.endDate) + "T" + this.endDateHour + ":" + this.endDateMinute);
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
        return ProductionReportController;
    }());
    ProductionReportController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService'
    ];


    angular
        .module('app')
        .controller('ProductionReportController', ProductionReportController);
})(App || (App = {}));