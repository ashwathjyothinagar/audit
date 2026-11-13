(function () {
    'use strict';

    angular
        .module('app')
        .controller('ProductionMetricsController', ProductionMetricsController);

    ProductionMetricsController.$inject = ['$http'];

    function ProductionMetricsController($http) {
        var $ctrl = this;

        $ctrl.reportType = 'Daily';
        $ctrl.startDate = '';
        $ctrl.endDate = '';
        $ctrl.startTime = '';
        $ctrl.endTime = '';
        $ctrl.selectedOffices = [];
        $ctrl.selectedLocation = '';
        
        $ctrl.hasData = false;
        $ctrl.searched = false;
        $ctrl.reportData = [];
        $ctrl.dynamicColumns = [];

        $ctrl.modalOrders = [];
        $ctrl.modalUser = '';
        $ctrl.modalTask = '';

        $ctrl.init = function () {
            $ctrl.setDatesForReportType();
        };

        $ctrl.setDatesForReportType = function () {
            var today = new Date();
            if ($ctrl.reportType === 'Monthly') {
                // First day of the current month
                $ctrl.startDate = new Date(today.getFullYear(), today.getMonth(), 1);
                // Last day of the current month
                $ctrl.endDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);
            } else {
                // Daily: default to today
                $ctrl.startDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
                $ctrl.endDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
            }
        };

        $ctrl.onReportTypeChange = function () {
            $ctrl.setDatesForReportType();
        };

        function formatDateForApi(dateObj) {
            if (!dateObj) return '';
            var dd = String(dateObj.getDate()).padStart(2, '0');
            var mm = String(dateObj.getMonth() + 1).padStart(2, '0');
            var yyyy = dateObj.getFullYear();
            return mm + '/' + dd + '/' + yyyy;
        }

        function formatTimeForApi(timeStr) {
            if (!timeStr) return '';
            // Handle both string "HH:mm" and Date object from ng-model
            if (timeStr instanceof Date) {
                var hh = String(timeStr.getHours()).padStart(2, '0');
                var mm = String(timeStr.getMinutes()).padStart(2, '0');
                return hh + ':' + mm;
            }
            return timeStr;
        }

        $ctrl.exportToExcel = function () {
            var table = document.getElementById("metricsTable");
            var html = table.outerHTML;
            var url = 'data:application/vnd.ms-excel;base64,' + btoa(unescape(encodeURIComponent(html)));
            var link = document.createElement("a");
            link.href = url;
            link.download = "ProductionMetrics.xls";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };

        $ctrl.generateReport = function () {
            var offices = $('#officeSelect').val();
            var selectedOfficesStr = offices ? offices.join(',') : '';
            
            var params = {
                startDate: formatDateForApi($ctrl.startDate),
                endDate: formatDateForApi($ctrl.endDate),
                reportType: $ctrl.reportType,
                selectedOffices: selectedOfficesStr,
                selectedLocation: $ctrl.selectedLocation || '',
                startTime: formatTimeForApi($ctrl.startTime),
                endTime: formatTimeForApi($ctrl.endTime)
            };

            $http.post('/Audits/GetProductionMetricsData', params)
                .then(function (response) {
                    $ctrl.searched = true;
                    if (response.data.success) {
                        $ctrl.reportData = response.data.data.UserRows;
                        $ctrl.dynamicColumns = response.data.data.DynamicTaskColumns;
                        $ctrl.hasData = $ctrl.reportData.length > 0;
                        
                        // Re-initialize DataTable if needed
                        setTimeout(function() {
                            if ($.fn.DataTable && $.fn.DataTable.isDataTable('#metricsTable')) {
                                $('#metricsTable').DataTable().destroy();
                            }
                            if ($.fn.DataTable) {
                                $('#metricsTable').DataTable();
                            }
                        }, 100);
                    } else {
                        alert("Error generating report: " + response.data.message);
                        $ctrl.hasData = false;
                    }
                }, function (error) {
                    alert("HTTP Error: " + (error.statusText || error.status));
                    $ctrl.hasData = false;
                    $ctrl.searched = true;
                });
        };

        $ctrl.showOrders = function (orderNumbers, userName, taskName) {
            $ctrl.modalOrders = orderNumbers;
            $ctrl.modalUser = userName;
            $ctrl.modalTask = taskName;
            $('#ordersModal').modal('show');
        };
    }
})();
