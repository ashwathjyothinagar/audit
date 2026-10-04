var App;
(function (App) {
    var AuditEditErrorsController = (function () {
        function AuditEditErrorsController($log, $http, BASE_URI, $window, $scope) {
            var _this = this;

            _this.auditId = (typeof window !== 'undefined' && window.currentAuditId) ? window.currentAuditId : 0;
            _this.orderNo = (typeof window !== 'undefined' && window.currentOrderNo) ? window.currentOrderNo : '';
            _this.taskEmployees = (typeof window !== 'undefined' && window.taskEmployeesData) ? window.taskEmployeesData : {};
            _this.errorCategories = (typeof window !== 'undefined' && window.errorCategoriesData) ? window.errorCategoriesData : [];
            _this.auditTasks = (typeof window !== 'undefined' && window.auditTasksData) ? window.auditTasksData : [];
            _this.existingErrors = (typeof window !== 'undefined' && window.currentErrorsData) ? window.currentErrorsData : [];

            _this.allEmployees = [];
            _this.taskErrorsMap = {}; // Keyed by TaskId
            _this.saving = false;
            _this.statusMessage = '';
            _this.isSuccess = false;
            _this.isError = false;

            // Build all employees list for auto-complete datalist
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
                // Initialize taskErrorsMap for each task
                _this.auditTasks.forEach(function (task) {
                    _this.taskErrorsMap[task.Id] = {
                        taskId: task.Id,
                        taskName: task.Name,
                        hasErrors: 'NO',
                        errors: []
                    };
                });

                // Populate with existing errors
                if (_this.existingErrors && _this.existingErrors.length > 0) {
                    _this.existingErrors.forEach(function (existing) {
                        var taskId = existing.TaskId;
                        if (!_this.taskErrorsMap[taskId]) {
                            _this.taskErrorsMap[taskId] = {
                                taskId: taskId,
                                taskName: 'Task ' + taskId,
                                hasErrors: 'NO',
                                errors: []
                            };
                        }

                        if (existing.OrderErrorJson) {
                            try {
                                var parsedList = JSON.parse(existing.OrderErrorJson);
                                if (Array.isArray(parsedList) && parsedList.length > 0) {
                                    _this.taskErrorsMap[taskId].hasErrors = 'YES';
                                    parsedList.forEach(function (err) {
                                        var newErr = {
                                            selectedCategory: err.SelectedCategory || err.selectedCategory || null,
                                            selectedType: err.SelectedType || err.selectedType || [],
                                            errorTypes: [],
                                            errorDoneBy: err.ErrorDoneBy || err.errorDoneBy || '',
                                            errorDoneById: err.ErrorDoneById || err.errorDoneById || null,
                                            errorDoneByEmail: err.ErrorDoneByEmail || err.errorDoneByEmail || '',
                                            comments: err.Comments || err.comments || '',
                                            isCritical: err.IsCritical || err.isCritical || false
                                        };

                                        // Load error types for the category
                                        if (newErr.selectedCategory) {
                                            _this.loadErrorTypesOnly(newErr.selectedCategory, newErr);
                                        }

                                        _this.taskErrorsMap[taskId].errors.push(newErr);
                                    });
                                }
                            } catch (e) {
                                console.error('Error parsing OrderErrorJson for taskId ' + taskId, e);
                            }
                        }
                    });
                }
            };

            this.loadErrorTypesOnly = function (categoryId, errorItem) {
                if (!categoryId) {
                    errorItem.errorTypes = [];
                    return;
                }
                var url = BASE_URI + "Audits/GetErrorTypes?categoryId=" + categoryId;
                if (_this.auditId) {
                    url += "&auditId=" + _this.auditId;
                }
                $http.get(url).then(function (response) {
                    if (response.data) {
                        if (Array.isArray(response.data)) {
                            errorItem.errorTypes = response.data;
                        } else {
                            errorItem.errorTypes = response.data.errorTypes || [];
                        }
                        _this.checkIfCritical(errorItem);
                    }
                });
            };

            this.loadErrorTypes = function (categoryId, errorItem) {
                if (categoryId) {
                    _this.autoPopulateEmployee(categoryId, errorItem);

                    var url = BASE_URI + "Audits/GetErrorTypes?categoryId=" + categoryId;
                    if (_this.auditId) {
                        url += "&auditId=" + _this.auditId;
                    }
                    $http.get(url).then(function (response) {
                        if (response.data) {
                            if (Array.isArray(response.data)) {
                                errorItem.errorTypes = response.data;
                            } else {
                                errorItem.errorTypes = response.data.errorTypes || [];
                                if (response.data.errorDoneBy && !errorItem.errorDoneBy) {
                                    errorItem.errorDoneBy = response.data.errorDoneBy;
                                    errorItem.errorDoneById = response.data.errorDoneById;
                                    errorItem.errorDoneByEmail = response.data.errorDoneByEmail;
                                }
                            }
                            errorItem.selectedType = [];
                            _this.checkIfCritical(errorItem);
                        }
                    });
                } else {
                    errorItem.errorTypes = [];
                    errorItem.selectedType = [];
                    errorItem.errorDoneBy = '';
                    errorItem.errorDoneById = null;
                    errorItem.errorDoneByEmail = '';
                    errorItem.isCritical = false;
                }
            };

            this.autoPopulateEmployee = function (categoryId, errorItem) {
                if (!categoryId || !errorItem) return;
                var category = null;
                if (_this.errorCategories && _this.errorCategories.length > 0) {
                    category = _this.errorCategories.find(function (c) { return c.Id == categoryId; });
                }
                var catName = category ? (category.Name || '').trim() : '';
                var emp = null;
                if (catName && _this.taskEmployees) {
                    for (var key in _this.taskEmployees) {
                        if (key.toLowerCase() === catName.toLowerCase()) {
                            emp = _this.taskEmployees[key];
                            break;
                        }
                    }
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
                    errorItem.errorDoneBy = emp.Name;
                    errorItem.errorDoneById = emp.UserId || null;
                    errorItem.errorDoneByEmail = emp.Email || '';
                }
            };

            this.onEmployeeChange = function (errorItem) {
                if (!errorItem || !errorItem.errorDoneBy) return;
                var trimmed = errorItem.errorDoneBy.trim().toLowerCase();
                if (_this.allEmployees && _this.allEmployees.length > 0) {
                    var matched = _this.allEmployees.find(function (emp) {
                        return (emp.Name && emp.Name.trim().toLowerCase() === trimmed);
                    });
                    if (matched) {
                        errorItem.errorDoneById = matched.UserId;
                        errorItem.errorDoneByEmail = matched.Email;
                    }
                }
            };

            this.addError = function (taskId) {
                if (!_this.taskErrorsMap[taskId]) return;
                _this.taskErrorsMap[taskId].hasErrors = 'YES';
                _this.taskErrorsMap[taskId].errors.push({
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

            this.removeError = function (taskId, index) {
                if (!_this.taskErrorsMap[taskId]) return;
                _this.taskErrorsMap[taskId].errors.splice(index, 1);
                if (_this.taskErrorsMap[taskId].errors.length === 0) {
                    _this.taskErrorsMap[taskId].hasErrors = 'NO';
                }
            };

            this.removeAllErrorsForTask = function (taskId) {
                if (!_this.taskErrorsMap[taskId]) return;
                if (confirm('Are you sure you want to remove all errors for this task?')) {
                    _this.taskErrorsMap[taskId].errors = [];
                    _this.taskErrorsMap[taskId].hasErrors = 'NO';
                }
            };

            this.onHasErrorsChange = function (taskId) {
                var taskEntry = _this.taskErrorsMap[taskId];
                if (!taskEntry) return;
                if (taskEntry.hasErrors === 'NO') {
                    taskEntry.errors = [];
                } else if (taskEntry.hasErrors === 'YES') {
                    if (taskEntry.errors.length === 0) {
                        _this.addError(taskId);
                    }
                }
            };

            this.checkIfCritical = function (errorItem) {
                var selectedTypes = errorItem.selectedType;
                var isCrit = false;
                if (selectedTypes && errorItem.errorTypes) {
                    if (Array.isArray(selectedTypes)) {
                        isCrit = errorItem.errorTypes.some(function (type) {
                            return selectedTypes.indexOf(type.Id) !== -1 && type.IsCritical;
                        });
                    } else {
                        var selectedType = errorItem.errorTypes.find(function (type) { return type.Id === selectedTypes; });
                        if (selectedType && selectedType.IsCritical) {
                            isCrit = true;
                        }
                    }
                }
                errorItem.isCritical = isCrit;
            };

            this.saveAll = function () {
                _this.saving = true;
                _this.statusMessage = '';
                _this.isSuccess = false;
                _this.isError = false;

                var tasksPayload = [];

                for (var taskIdKey in _this.taskErrorsMap) {
                    var taskEntry = _this.taskErrorsMap[taskIdKey];
                    var hasValid = false;
                    var validList = [];

                    if (taskEntry.hasErrors === 'YES' && taskEntry.errors && taskEntry.errors.length > 0) {
                        taskEntry.errors.forEach(function (err) {
                            if (err.selectedCategory) {
                                hasValid = true;
                                validList.push({
                                    selectedCategory: err.selectedCategory,
                                    selectedType: Array.isArray(err.selectedType) ? err.selectedType : (err.selectedType ? [err.selectedType] : []),
                                    errorDoneBy: err.errorDoneBy || '',
                                    errorDoneById: err.errorDoneById || null,
                                    errorDoneByEmail: err.errorDoneByEmail || '',
                                    comments: err.comments || '',
                                    isCritical: err.isCritical || false
                                });
                            }
                        });
                    }

                    tasksPayload.push({
                        TaskId: parseInt(taskIdKey, 10),
                        HasErrors: hasValid,
                        OrderErrorJson: JSON.stringify(validList)
                    });
                }

                var postData = {
                    AuditId: _this.auditId,
                    Tasks: tasksPayload
                };

                $http.post(BASE_URI + "Audits/SaveEditedErrors", postData).then(function (response) {
                    _this.saving = false;
                    if (response.data && response.data.success) {
                        _this.isSuccess = true;
                        _this.statusMessage = response.data.message || 'Errors saved successfully!';
                        setTimeout(function () {
                            $window.location.href = BASE_URI + "Audits/Details/" + _this.auditId;
                        }, 1200);
                    } else {
                        _this.isError = true;
                        _this.statusMessage = (response.data && response.data.message) ? response.data.message : 'Error saving changes.';
                    }
                }, function (err) {
                    _this.saving = false;
                    _this.isError = true;
                    _this.statusMessage = 'Server error occurred while saving.';
                });
            };

            this.init();
        }

        return AuditEditErrorsController;
    }());

    AuditEditErrorsController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        '$window',
        '$scope'
    ];

    angular
        .module('app')
        .controller('AuditEditErrorsController', AuditEditErrorsController);
})(App || (App = {}));
