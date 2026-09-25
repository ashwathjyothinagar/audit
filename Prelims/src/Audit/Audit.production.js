$('#submitButton').click(function (e) {
    if ($('#Audit_TaskId').val() == "3") {
        if (!confirm("Are you sure that you have sent mail to client?")) {
            e.preventDefault();
        }
    }
});


var App;
(function (App) {

    var AuditProductionController = (function () {
        function AuditProductionController($log, $http, BASE_URI, $window, $scope) {
            var _this = this;

            _this.checks = {};
            this.init = function () {
                _this.errorCategories = [];
                _this.orderErrors = [];

                $http.get(ROOTURL + "Audits/GetErrorCategories").then(function (response) {
                    _this.errorCategories = response.data;
                });

                _this.addOrderError();
            };

            this.loadErrorTypes = function (categoryId, index) {
                if (categoryId) {
                    // Fetch error types based on the selected category
                    $http.get(ROOTURL + "Audits/GetErrorTypes?categoryId=" + categoryId)
                        .then(function (response) {
                            // Assign the error types to the specific order error entry
                            _this.orderErrors[index].errorTypes = response.data;
                            _this.orderErrors[index].selectedType = null; // Clear selected error type
                        });
                } else {
                    // If no category is selected, clear the error types
                    _this.orderErrors[index].errorTypes = [];
                    _this.orderErrors[index].selectedType = null; // Clear selected error type
                }
            };

            this.addOrderError = function () {
                _this.orderErrors.push({
                    selectedCategory: null,
                    selectedType: null,
                    errorTypes: [], // Empty error types array for this row
                    comments: '',
                    isCritical: false
                });
            };

            this.onAnyErrorsChange = function () {
                if (_this.checks.anyErrors === '--' || _this.checks.anyErrors === 'NO') {
                    _this.orderErrors = [];
                    this.addOrderError();
                }
            }

            _this.removeOrderError = function (index) {
                _this.orderErrors.splice(index, 1);
            };

            _this.checkIfCritical = function (index) {
                var selectedTypeId = _this.orderErrors[index].selectedType;
                var selectedType = _this.orderErrors[index].errorTypes.find(type => type.Id === selectedTypeId);
                if (selectedType && selectedType.IsCritical) {
                    _this.orderErrors[index].isCritical = true;
                } else {
                    _this.orderErrors[index].isCritical = false;
                }
            };

            this.init();
        }
        return AuditProductionController;
    }());
    AuditProductionController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        '$window',
        '$scope'
    ];


    angular
        .module('app')
        .controller('AuditProductionController', AuditProductionController);
})(App || (App = {}));