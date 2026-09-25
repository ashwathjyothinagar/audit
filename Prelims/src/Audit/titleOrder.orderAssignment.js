var App;
(function (App) {

    var OrderAssignmentController = (function () {
        function OrderAssignmentController($log, $http, BASE_URI,  $location, $scope) {
            var _this = this;

            _this.isRowSelected = false;
            _this.selectedOrderId;

            _this.onAssignClick = function (e) {
                var items = [];

                $.each($('#orderAssignmentTable .selectRow'), function () {
                    items.push(parseInt($(this).attr("itemId")));
                });

                var userId = $("#UserInfos").val() === "" ? null : parseInt($("#UserInfos").val());

                var req = {
                    method: 'POST',
                    url: BASE_URI + "Audits/AssignUserToOrder",
                    data: { userId: userId, titleOrderIds: items }
                }

                $http(req).then(function (response) {
                    _this.isRowSelected = false;

                    $('#orderAssignmentTable .trrow').each(function (index) {
                        $(this).removeClass('selectRow');
                    });

                    alert("User is assigned successfully.");
                });
            }

            this.init = function () {
                $('#orderAssignmentTable .trrow').click(function (event) {
                    $(this).toggleClass('selectRow');
                    _this.isRowSelected = $('#orderAssignmentTable tr').hasClass("selectRow");
                    $scope.$digest();
                });
            };

            this.init();
        }
        return OrderAssignmentController;
    }());

    OrderAssignmentController.$inject = [
        '$log',
        '$http',
        'BASE_URI',
        '$location',
        '$scope'
    ];

    angular
        .module('app')
        .controller('OrderAssignmentController', OrderAssignmentController);
})(App || (App = {}));
