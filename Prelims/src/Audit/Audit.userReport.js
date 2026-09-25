var App;
(function (App) {

    var UserReportController = (function () {
        function UserReportController($log, $http, BASE_URI, ReportLookupService, $location) {
            var _this = this;

            _this.CrnId = 0;
            _this.Crns = [];
            _this.LegalVerificationdOrders = [];
            _this.userFilter = {};
            this.init = function () {

                this.getCrns().then(function (response) {
                    _this.Crns = response.data;
                });

                this.hours = ReportLookupService.getHours();
                this.minutes = ReportLookupService.getMinutes();
                this.periods = ReportLookupService.getPeriods();

                this.userFilter.startDateHour = "00";
                this.userFilter.startDateMinute = "00";

                this.userFilter.endDateHour = "23";
                this.userFilter.endDateMinute = "59";

                //if (localStorage) {
                //    var savedFilter = localStorage.getItem("UserReportFilter");

                //    if (savedFilter != null) {
                //        _this.userFilter = JSON.parse(savedFilter);
                //    }
                //}
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

                $('#startDateTime').val(ReportLookupService.getISODate(_this.userFilter.startDate) + "T" + this.userFilter.startDateHour + ":" + this.userFilter.startDateMinute);
                $('#endDateTime').val(ReportLookupService.getISODate(_this.userFilter.endDate) + "T" + this.userFilter.endDateHour + ":" + this.userFilter.endDateMinute);

                //localStorage.setItem("UserReportFilter", JSON.stringify(this.userFilter));
                searchForm.submit();
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
        return UserReportController;
    }());
    UserReportController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService',
        '$location'
    ];


    angular
        .module('app')
        .controller('UserReportController', UserReportController);
})(App || (App = {}));
