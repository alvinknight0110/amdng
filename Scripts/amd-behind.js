var amd = angular.module('amd', ['ngRoute', 'ngSanitize', 'blockUI']).config(['$routeProvider', '$locationProvider', 'blockUIConfig',
    function ($routeProvider, $locationProvider, blockUIConfig) {
        $routeProvider
            .when("/", { templateUrl: '/Control/Index', controller: 'main' });

        blockUIConfig.message = 'Loading ...';
    }
]);

amd.filter("jsonDate", function () {
    var re = /\/Date\(([0-9]*)\)\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});

amd.controller('main', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $scope.search = { 'Type': '1' };
        $scope.data = [];

        $scope.query = function () {
            $http.get('/Control/Query', { params: $scope.search }).then(function (response) {
                $scope.data = response.data;
            }, function (error) {
            });
        };

        $scope.check = function (item) {
            $scope.request = item;
        };

        $scope.verify = function () {
            $http.post('Control/Verify', $scope.request).then(function (result) {
                if (result.data === false) alert('資料更新失敗，請重新嘗試。');
                else {
                    alert('資料送出成功！');
                    $('#verify').modal('hide');
                }
            }, function (error) {
            });
        };

        $scope.export = function () {
            $http.get('/Control/Export', { params: $scope.search }).then(function (response) {
                if (response.data !== false) {
                    alert('檔案匯出成功！');
                    window.location = response.data;
                } else {
                    alert('檔案匯出失敗…');
                }
            }, function (error) {
            });
        };
    }
]);