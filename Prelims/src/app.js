var App;
(function (App) {
    angular
        .module('app', [
        'ui.router',
        'ui.bootstrap',
        'ngSanitize'
        ]).config(['$stateProvider',
            '$locationProvider',
            '$httpProvider',
            '$urlRouterProvider',
            '$compileProvider',
            '$controllerProvider',
            function (
                $stateProvider,
                $locationProvider,
                $httpProvider,
                $urlRouterProvider,
                $compileProvider,
                $controllerProvider
                ) {


            }]);

})(App || (App = {}));

var App;
(function (App) {

    var AuditController = (function () {
        function AuditController($log, $http, BASE_URI) {
            var _this = this;

            _this.CrnId = "";
            _this.Crns = [];
            _this.FilteredSenders = [];
            _this.fileURL = "http://webmail.fnfsocal.com";
            _this.emailItems=[];

            _this.fetchEmails = function () {
                
                var req = {
                    method: 'GET',
                    url: BASE_URI + "Emails/ReadMails"
                }

                $http(req).then(function (emailItems) {
                    _this.emailItems = emailItems.data.data;
                });
            }

            this.showEmailDetail = function (emailItem) {
                _this.emailDetail = undefined;
                var req = {
                    method: 'GET',
                    url: BASE_URI + "Emails/GetEmailDetails?id=" + emailItem[5]
                }

                $http(req).then(function (emailDetailResponse) {
                    _this.emailDetail = emailDetailResponse.data.data;
                });
            }

            this.init = function () {
                
                this.getCrns().then(function (response) {
                    _this.Crns = response.data;
                });

                this.getSenders().then(function (response) {
                    _this.senders = response.data;

                });

                _this.fetchEmails();
            };

            this.onSubmitClick = function (e) {
                var isOrderExist=false;
                
                $.ajax({
                    async: false,
                    url: BASE_URI + "Audits/CheckOrderStatus?orderNo=" + $('#OrderNo').val(),
                    success: function (response) {
                        isOrderExist = response;
                    }
                });

                if (isOrderExist) {
                    if (!confirm("Order you are trying to enter is already exist. Are you sure to enter the duplicate order?")) {
                        e.preventDefault();
                    }
                }
            }

            this.onCrnChange = function () {
                this.FilteredSenders = _.filter(_this.senders, function (itemToFilter) {
                    return itemToFilter.CrnId == _this.CrnId;
                })
            }

            this.getCrns = function () {
                var req = {
                    method: 'GET',
                    url: BASE_URI + "api/Crns"
                }

                return $http(req)
            };

            this.getSenders = function () {
                var req = {
                    method: 'GET',
                    url: BASE_URI + "api/AuditSendersApi"
                }

                return $http(req)
            }

            this.prepareFromAddress = function (emailItem) {
                return _.map(emailItem.from, function (fromAddress) {
                    return _this.getDisplayName(fromAddress[0]) + "(" + fromAddress[1] + ")";
                }).join();
            }

            this.getEmailContent = function (emailItem) {
                let responseContent = "";

                var emailHtmlContent = _.find(emailItem.attachments, function (itemToFind) {
                    return itemToFind["content_type"] === "text/html" || itemToFind["content_type"] === "text/plain";
                });

                if (emailHtmlContent) {
                    responseContent = emailHtmlContent.content;
                }

                return responseContent;
            }

            this.getDisplayName = function (nameString) {
                var displayName = nameString.replace("\"", "").replace("\"", "");

                var splitedString = nameString.split(",");

                if (splitedString.length > 1) {
                    return splitedString[1].replace("\"", "") + " " + splitedString[0].replace("\"", "");
                } else {
                    return nameString;
                }
                return "";
            }

            this.init();
        }
        return AuditController;
    }());
    AuditController.$inject = [
        '$log',
        '$http',
        'BASE_URI'
    ];


    angular
        .module('app')
        .controller('AuditController', AuditController);
})(App || (App = {}));

var App;
(function () {

    var ReportLookupService = (function () {
        function ReportLookupService($injector, BASE_URI, $http) {
            var _this = this;
            this.$injector = $injector;

            this.getHours = function () {

                return [
                    "00",
                    "01",
                    "02",
                    "03",
                    "04",
                    "05",
                    "06",
                    "07",
                    "08",
                    "09",
                    "10",
                    "11",
                    "12",
                    "13",
                     "14",
                     "15",
                     "16",
                     "17",
                     "18",
                     "19",
                     "20",
                     "21",
                     "22",
                     "23"
                ];
            };

            this.getMinutes = function () {
                return ["00",
                          "01",
                          "02",
                          "03",
                          "04",
                          "05",
                          "06",
                          "07",
                          "08",
                          "09",
                          "10",
                          "11",
                          "12",
                          "13",
                          "14",
                          "15",
                          "16",
                          "17",
                          "18",
                          "19",
                          "20",
                          "21",
                          "22",
                          "23",
                          "24",
                          "25",
                          "26",
                          "27",
                          "28",
                          "29",
                          "30",
                          "31",
                          "32",
                          "33",
                          "34",
                          "35",
                          "36",
                          "37",
                          "38",
                          "39",
                          "40",
                          "41",
                          "42",
                          "43",
                          "44",
                          "45",
                          "46",
                          "47",
                          "48",
                          "49",
                          "50",
                          "51",
                          "52",
                          "53",
                          "54",
                          "55",
                          "56",
                          "57",
                          "58",
                          "59"];
            };

            this.getPeriods = function () {
                return ["AM", "PM"];
            };

            this.getISODate = function (date) {
                var d = new Date(date),
                    month = '' + (d.getMonth() + 1),
                    day = '' + d.getDate(),
                    year = d.getFullYear();

                if (month.length < 2) month = '0' + month;
                if (day.length < 2) day = '0' + day;

                return [year, month, day].join('-');
            }
        }


        return ReportLookupService;

    }());
    ReportLookupService.$inject = [
        '$injector',
        'BASE_URI',
        '$http'
    ];
    angular
        .module('app')
        .service('ReportLookupService', ReportLookupService);
})(App || (App = {}));

