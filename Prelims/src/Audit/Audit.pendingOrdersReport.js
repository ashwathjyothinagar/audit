(function () {
    'use strict';

    angular
        .module('app')
        .controller('PendingOrdersReportController', ['$http', function ($http) {
            var $ctrl = this;

            $ctrl.selectedStatuses = [];
            $ctrl.loading = false;
            $ctrl.filteredOrders = [];

            $ctrl.getSelectedOffices = function () {
                return $('#SelectedOffices').val() || '';
            };

            $ctrl.getSelectedStatuses = function () {
                return ($ctrl.selectedStatuses || []).join(',');
            };

            $ctrl.loadReport = function () {
                $ctrl.loading = true;
                var url = '/Audits/GetPendingOrders';
                $http.get(url, {
                    params: {
                        selectedOffices: $ctrl.getSelectedOffices(),
                        selectedStatuses: $ctrl.getSelectedStatuses()
                    }
                }).then(function (response) {
                    $ctrl.filteredOrders = response.data;
                    $ctrl.loading = false;
                }, function () {
                    $ctrl.loading = false;
                });
            };

            $ctrl.getTotalCount = function () {
                return $ctrl.filteredOrders.reduce(function (sum, o) {
                    return sum + Number(o.totalItems || 0);
                }, 0);
            };

            $ctrl.exportReport = function () {
                var invisibleLink = document.createElement('a');
                var href = '/Audits/ExportPendingOrdersReport'
                    + '?selectedOffices=' + encodeURIComponent($ctrl.getSelectedOffices())
                    + '&selectedStatuses=' + encodeURIComponent($ctrl.getSelectedStatuses());
                $(invisibleLink).attr('href', href);
                document.body.appendChild(invisibleLink);
                invisibleLink.click();
                document.body.removeChild(invisibleLink);
            };

            $ctrl.countByStatus = function (status) {
                return $ctrl.filteredOrders.filter(function (o) { return o.status === status; }).length;
            };

            angular.element(document).ready(function () {
                $(document).off('change.pendingOrdersReport', '.group-checkbox, .office-checkbox');
                $(document).on('change.pendingOrdersReport', '.group-checkbox, .office-checkbox', function () {
                    $ctrl.loadReport();
                });
            });

            // Load on init
            $ctrl.loadReport();
        }]);
})();
