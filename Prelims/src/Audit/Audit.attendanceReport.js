var App;
(function (App) {

    var AttendanceReportController = (function () {
        function AttendanceReportController($log, $http, BASE_URI, ReportLookupService, $location) {
            var _this = this;

            _this.userFilter = {};
            _this.users = [];
            _this.filteredUsers = [];
            _this.selectedUserId = '';

            // Multi-select reporting manager state
            _this.selectedManagerIds = {};   // { [managerId]: true/false }
            _this.allManagersSelected = false;
            _this.managerDropdownOpen = false;

            this.init = function () {
                this.hours = ReportLookupService.getHours();
                this.minutes = ReportLookupService.getMinutes();

                this.userFilter.startDateHour = "00";
                this.userFilter.startDateMinute = "00";

                this.userFilter.endDateHour = "23";
                this.userFilter.endDateMinute = "59";

                _this.loadUsers();
            };

            this.loadUsers = function () {
                $http.get(BASE_URI + 'Audits/GetUsersForAttendance').then(function (response) {
                    _this.users = response.data || [];
                    _this.filteredUsers = _this.users;
                });
                $http.get(BASE_URI + 'Audits/GetReportingManagers').then(function (response) {
                    _this.reportingManagers = response.data || [];
                });
            };

            // ---- Multi-select helper methods ----

            _this.toggleManagerDropdown = function ($event) {
                $event.stopPropagation();
                _this.managerDropdownOpen = !_this.managerDropdownOpen;
            };

            _this.isManagerSelected = function (id) {
                return !!_this.selectedManagerIds[id];
            };

            _this.getSelectedManagerIdList = function () {
                return Object.keys(_this.selectedManagerIds).filter(function (k) {
                    return _this.selectedManagerIds[k];
                });
            };

            _this.selectedManagerLabel = function () {
                var selected = _this.getSelectedManagerIdList();
                if (selected.length === 0) return 'ALL';
                if (_this.allManagersSelected) return 'ALL';
                var names = selected.map(function (id) {
                    var m = (_this.reportingManagers || []).find(function (r) { return String(r.id) === String(id); });
                    return m ? m.name : id;
                });
                if (names.length <= 2) return names.join(', ');
                return names.slice(0, 2).join(', ') + ' (+' + (names.length - 2) + ' more)';
            };

            _this.onManagerChange = function () {
                // Sync the "ALL" checkbox state
                var allSelected = (_this.reportingManagers || []).every(function (m) {
                    return !!_this.selectedManagerIds[m.id];
                });
                _this.allManagersSelected = allSelected && (_this.reportingManagers || []).length > 0;
                _this.filterUsers();
            };

            _this.toggleAllManagers = function () {
                if (_this.allManagersSelected) {
                    (_this.reportingManagers || []).forEach(function (m) {
                        _this.selectedManagerIds[m.id] = true;
                    });
                } else {
                    _this.selectedManagerIds = {};
                }
                _this.filterUsers();
            };

            // ---- Filter users based on location and selected managers ----

            this.filterUsers = function () {
                var loc = _this.userFilter.location;
                var selectedIds = _this.getSelectedManagerIdList();

                _this.filteredUsers = _this.users.filter(function (u) {
                    var matchLoc = !loc || u.location === loc;
                    var matchMgr = selectedIds.length === 0 || selectedIds.some(function (id) {
                        return String(u.reportingManagerId) === String(id);
                    });
                    return matchLoc && matchMgr;
                });
                // Reset selected user when filter changes
                _this.selectedUserId = '';
                _this.userSearchText = '';
            };

            this.init();

            this.loadReport = function () {
                $('#startDateTime').val(ReportLookupService.getISODate(_this.userFilter.startDate) + "T" + this.userFilter.startDateHour + ":" + this.userFilter.startDateMinute);
                $('#endDateTime').val(ReportLookupService.getISODate(_this.userFilter.endDate) + "T" + this.userFilter.endDateHour + ":" + this.userFilter.endDateMinute);
                $('#selectedUserId').val(_this.selectedUserId || '');
                // Pass comma-separated manager IDs (empty string means ALL)
                var selectedIds = _this.getSelectedManagerIdList();
                $('#reportingManagerIds').val(selectedIds.join(','));
                $('#LOCATION').val(_this.userFilter.location || '');
                attendanceSearchForm.submit();
            };

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
            };

            this.openStartPopup = function () {
                this.startOpened = true;
            };
        }
        return AttendanceReportController;
    }());
    AttendanceReportController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        'ReportLookupService',
        '$location'
    ];


    angular
        .module('app')
        .controller('AttendanceReportController', AttendanceReportController);
})(App || (App = {}));
