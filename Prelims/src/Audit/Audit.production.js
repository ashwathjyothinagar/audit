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
            _this.taskEmployees = (typeof window !== 'undefined' && window.taskEmployeesData) ? window.taskEmployeesData : {};
            _this.errorCategories = (typeof window !== 'undefined' && window.errorCategoriesData) ? window.errorCategoriesData : [];
            _this.allEmployees = [];

            // Helper to build datalist suggestions
            function rebuildAllEmployees() {
                _this.allEmployees = [];
                for (var key in _this.taskEmployees) {
                    var emp = _this.taskEmployees[key];
                    if (emp && emp.Name && !_this.allEmployees.some(function (e) { return e.Name === emp.Name; })) {
                        _this.allEmployees.push(emp);
                    }
                }
            }
            rebuildAllEmployees();

            this.init = function () {
                if (!_this.errorCategories || _this.errorCategories.length === 0) {
                    $http.get(ROOTURL + "Audits/GetErrorCategories").then(function (response) {
                        _this.errorCategories = response.data;
                    });
                }

                var auditId = $('#AuditId').val() || (typeof window !== 'undefined' ? window.currentAuditId : null);
                if (auditId && Object.keys(_this.taskEmployees).length === 0) {
                    $http.get(ROOTURL + "Audits/GetTaskEmployeesByTitleOrderId?auditId=" + auditId).then(function (response) {
                        if (response.data && response.data.success && response.data.taskEmployees) {
                            _this.taskEmployees = response.data.taskEmployees;
                            rebuildAllEmployees();

                            // Auto populate any already selected category
                            if (_this.orderErrors && _this.orderErrors.length > 0) {
                                _this.orderErrors.forEach(function (err, idx) {
                                    if (err.selectedCategory && !err.errorDoneBy) {
                                        _this.autoPopulateEmployee(err.selectedCategory, idx);
                                    }
                                });
                            }
                        }
                    });
                }

                if (!_this.orderErrors || _this.orderErrors.length === 0) {
                    _this.orderErrors = [];
                    _this.addOrderError();
                } else {
                    // Check if existing errors need employee populated
                    var hasValid = false;
                    _this.orderErrors.forEach(function (err, idx) {
                        if (err.selectedCategory) {
                            hasValid = true;
                            if (!err.errorDoneBy) {
                                _this.autoPopulateEmployee(err.selectedCategory, idx);
                            }
                        }
                    });
                    if (hasValid) {
                        _this.checks.anyErrors = 'YES';
                    }
                }
            };

            this.loadErrorTypes = function (categoryId, index) {
                if (categoryId) {
                    var auditId = $('#AuditId').val() || (typeof window !== 'undefined' ? window.currentAuditId : null);

                    // 1. Immediately auto-populate Error Done By synchronously
                    _this.autoPopulateEmployee(categoryId, index);

                    // 2. Fetch error types and server-resolved employee from backend
                    var url = ROOTURL + "Audits/GetErrorTypes?categoryId=" + categoryId;
                    if (auditId) {
                        url += "&auditId=" + auditId;
                    }

                    $http.get(url).then(function (response) {
                        if (response.data) {
                            if (Array.isArray(response.data)) {
                                _this.orderErrors[index].errorTypes = response.data;
                            } else {
                                _this.orderErrors[index].errorTypes = response.data.errorTypes || [];
                                if (response.data.errorDoneBy && !_this.orderErrors[index].errorDoneBy) {
                                    _this.orderErrors[index].errorDoneBy = response.data.errorDoneBy;
                                    _this.orderErrors[index].errorDoneById = response.data.errorDoneById;
                                    _this.orderErrors[index].errorDoneByEmail = response.data.errorDoneByEmail;
                                }
                            }
                            _this.orderErrors[index].selectedType = [];
                        }
                    });
                } else {
                    _this.orderErrors[index].errorTypes = [];
                    _this.orderErrors[index].selectedType = [];
                    _this.orderErrors[index].errorDoneBy = '';
                    _this.orderErrors[index].errorDoneById = null;
                    _this.orderErrors[index].errorDoneByEmail = '';
                }
            };

            this.autoPopulateEmployee = function (categoryId, index) {
                if (!categoryId || !_this.orderErrors || !_this.orderErrors[index]) return;

                var category = null;
                if (_this.errorCategories && _this.errorCategories.length > 0) {
                    category = _this.errorCategories.find(function (c) { return c.Id == categoryId; });
                }

                var catName = category ? (category.Name || '').trim() : '';

                var emp = null;
                if (catName && _this.taskEmployees) {
                    // Direct key match (case-insensitive)
                    for (var key in _this.taskEmployees) {
                        if (key.toLowerCase() === catName.toLowerCase()) {
                            emp = _this.taskEmployees[key];
                            break;
                        }
                    }
                    // Partial / substring match
                    if (!emp) {
                        for (var key2 in _this.taskEmployees) {
                            if (key2.toLowerCase().indexOf(catName.toLowerCase()) !== -1 || catName.toLowerCase().indexOf(key2.toLowerCase()) !== -1) {
                                emp = _this.taskEmployees[key2];
                                break;
                            }
                        }
                    }
                }

                if (emp && emp.Name) {
                    _this.orderErrors[index].errorDoneBy = emp.Name;
                    _this.orderErrors[index].errorDoneById = emp.UserId || null;
                    _this.orderErrors[index].errorDoneByEmail = emp.Email || '';
                } else {
                    _this.orderErrors[index].errorDoneBy = '';
                    _this.orderErrors[index].errorDoneById = null;
                    _this.orderErrors[index].errorDoneByEmail = '';
                }
            };

            this.onEmployeeChange = function (index) {
                var error = _this.orderErrors[index];
                if (!error || !error.errorDoneBy) return;
                var trimmed = error.errorDoneBy.trim().toLowerCase();
                if (_this.allEmployees && _this.allEmployees.length > 0) {
                    var matched = _this.allEmployees.find(function (emp) {
                        return (emp.Name && emp.Name.trim().toLowerCase() === trimmed);
                    });
                    if (matched) {
                        error.errorDoneById = matched.UserId;
                        error.errorDoneByEmail = matched.Email;
                    }
                }
            };

            this.addOrderError = function () {
                _this.orderErrors.push({
                    selectedCategory: null,
                    selectedType: [],
                    errorTypes: [],
                    errorDoneBy: '',
                    errorDoneById: null,
                    errorDoneByEmail: '',
                    comments: '',
                    isCritical: false
                });
            };

            this.onAnyErrorsChange = function () {
                if (_this.checks.anyErrors === '--' || _this.checks.anyErrors === 'NO') {
                    _this.orderErrors = [];
                    this.addOrderError();
                }
            };

            _this.removeOrderError = function (index) {
                _this.orderErrors.splice(index, 1);
            };

            _this.checkIfCritical = function (index) {
                var selectedTypes = _this.orderErrors[index].selectedType;
                var isCrit = false;
                if (selectedTypes && _this.orderErrors[index].errorTypes) {
                    if (Array.isArray(selectedTypes)) {
                        isCrit = _this.orderErrors[index].errorTypes.some(function (type) {
                            return selectedTypes.indexOf(type.Id) !== -1 && type.IsCritical;
                        });
                    } else {
                        var selectedType = _this.orderErrors[index].errorTypes.find(function (type) { return type.Id === selectedTypes; });
                        if (selectedType && selectedType.IsCritical) {
                            isCrit = true;
                        }
                    }
                }
                _this.orderErrors[index].isCritical = isCrit;
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