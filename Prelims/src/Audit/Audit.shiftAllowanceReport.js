var App;
(function (App) {
    'use strict';

    var ShiftAllowanceReportController = (function () {
        function ShiftAllowanceReportController($log, $http, BASE_URI, ReportLookupService) {
            var _this = this;

            _this.userFilter      = {};
            _this.users           = [];
            _this.filteredUsers   = [];
            _this.selectedUserId  = '';
            _this.userSearchText  = '';
            _this.showUserDropdown = false;

            _this.reportData  = [];
            _this.hasData     = false;
            _this.searched    = false;
            _this.loading     = false;
            _this.periodLabel = '';

            // Modal state
            _this.modalTitle    = '';
            _this.modalEmployee = '';
            _this.modalDates    = [];

            // ── Init ──────────────────────────────────────────────────────
            this.init = function () {
                _this.hours   = ReportLookupService.getHours();
                _this.minutes = ReportLookupService.getMinutes();

                // Default to first day of current month → today, both in IST (UTC+5:30)
                var nowIst = new Date(new Date().toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));
                _this.userFilter.startDate       = new Date(nowIst.getFullYear(), nowIst.getMonth(), 1);
                _this.userFilter.startDateHour   = '00';
                _this.userFilter.startDateMinute = '00';
                _this.userFilter.endDate         = new Date(nowIst.getFullYear(), nowIst.getMonth(), nowIst.getDate());
                _this.userFilter.endDateHour     = '23';
                _this.userFilter.endDateMinute   = '59';

                _this.dateOptions = {
                    dateDisabled:  false,
                    formatYear:    'yy',
                    maxDate:       new Date(2220, 5, 22),
                    minDate:       new Date(1920, 5, 22),
                    startingDay:   1
                };
                _this.altInputFormats = ['M!/d!/yyyy'];

                _this.loadUsers();
            };

            // ── User list ─────────────────────────────────────────────────
            this.loadUsers = function () {
                $http.get(BASE_URI + 'Audits/GetUsersForAttendance').then(function (response) {
                    _this.users         = response.data || [];
                    _this.filteredUsers = _this.users;
                });
            };

            this.filterUsersByLocation = function () {
                var loc = _this.userFilter.location;
                if (!loc) {
                    _this.filteredUsers = _this.users;
                } else {
                    _this.filteredUsers = _this.users.filter(function (u) {
                        return u.location === loc;
                    });
                }
                _this.selectedUserId = '';
            };

            // ── Date pickers ──────────────────────────────────────────────
            this.openStartPopup = function () { _this.startOpened = true; };
            this.openEndPopup   = function () { _this.endOpened   = true; };

            // ── Load report ───────────────────────────────────────────────
            this.loadReport = function () {
                var startIso = ReportLookupService.getISODate(_this.userFilter.startDate) +
                               'T' + _this.userFilter.startDateHour + ':' + _this.userFilter.startDateMinute;
                var endIso   = ReportLookupService.getISODate(_this.userFilter.endDate) +
                               'T' + _this.userFilter.endDateHour   + ':' + _this.userFilter.endDateMinute;

                var params = {
                    startDateTime:  startIso,
                    endDateTime:    endIso,
                    location:       _this.userFilter.location   || '',
                    selectedUserId: _this.selectedUserId        || ''
                };

                _this.loading = true;
                _this.searched = false;
                _this.hasData  = false;

                // Build period label for table header
                var sd = _this.userFilter.startDate;
                var ed = _this.userFilter.endDate;
                var monthNames = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
                if (sd && ed) {
                    _this.periodLabel = monthNames[sd.getMonth()] + "'" + String(sd.getFullYear()).slice(-2) +
                        ' – ' + monthNames[ed.getMonth()] + "'" + String(ed.getFullYear()).slice(-2);
                }

                $http.get(BASE_URI + 'Audits/GetShiftAllowanceReportData', { params: params })
                    .then(function (response) {
                        _this.loading  = false;
                        _this.searched = true;
                        if (response.data.success) {
                            _this.reportData = response.data.data ? response.data.data.Rows : [];
                            _this.hasData    = _this.reportData && _this.reportData.length > 0;
                        } else {
                            alert('Error: ' + (response.data.message || 'Unknown error'));
                        }
                    }, function (err) {
                        _this.loading  = false;
                        _this.searched = true;
                        alert('HTTP Error: ' + (err.statusText || err.status));
                    });
            };

            // ── Popup ─────────────────────────────────────────────────────
            this.showPopup = function (type, row) {
                _this.modalEmployee = row.Name + ' (' + row.EmpCode + ')';
                switch (type) {
                    case 'leave':
                        _this.modalTitle = 'Leave Dates';
                        _this.modalDates = row.LeaveDates || [];
                        break;
                    case 'day':
                        _this.modalTitle = 'Day Shift Dates';
                        _this.modalDates = row.DayShiftDates || [];
                        break;
                    case 'night':
                        _this.modalTitle = 'Night Shift Dates';
                        _this.modalDates = row.NightShiftDates || [];
                        break;
                    case 'nsa':
                        _this.modalTitle = 'NSA (Night Shift Allowance) Dates';
                        _this.modalDates = row.NightShiftDates || [];
                        break;
                    default:
                        _this.modalTitle = 'Details';
                        _this.modalDates = [];
                }
                $('#sarDetailModal').modal('show');
            };

            // ── Export to Excel ───────────────────────────────────────────
            this.exportToExcel = function () {
                var table = document.getElementById('shiftAllowanceTable');
                if (!table) return;
                var html = table.outerHTML;
                var url  = 'data:application/vnd.ms-excel;base64,' + btoa(unescape(encodeURIComponent(html)));
                var link = document.createElement('a');
                link.href     = url;
                link.download = 'ShiftAllowanceReport.xls';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            };

            _this.init();
        }

        ShiftAllowanceReportController.$inject = [
            '$log',
            '$http',
            'BASE_URI',
            'ReportLookupService'
        ];

        return ShiftAllowanceReportController;
    }());

    angular
        .module('app')
        .controller('ShiftAllowanceReportController', ShiftAllowanceReportController);

})(App || (App = {}));
