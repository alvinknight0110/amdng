var amd = angular.module('amd', ['ngRoute', 'ngSanitize', 'ngMessages', 'blockUI']).config(['$routeProvider', '$locationProvider', 'blockUIConfig',
    function ($routeProvider, $locationProvider, blockUIConfig) {
        $routeProvider
            .when("/", { templateUrl: '/Activity/Faith', controller: 'faith' })
            .when("/xbox", { templateUrl: '/Activity/XBox', controller: 'xbox' })
            .when("/gb", { templateUrl: '/Activity/GB', controller: 'gb' })
            .when("/faith", { templateUrl: '/Activity/Faith', controller: 'faith' })
            .when("/gifts", { templateUrl: '/Activity/Gifts', controller: 'gifts' })
            .when("/progress", { templateUrl: '/Activity/Progress', controller: 'progress' })
            .otherwise({ redirectTo: '/' });

        blockUIConfig.message = 'Loading ...';
    }
]);

amd.run(['$rootScope', '$http',
    function ($rootScope, $http) {
        $rootScope.request = {};
        $rootScope.apply = function () {
            if ($rootScope.request.InvoiceFile === undefined) {
                alert("請上傳發票照片");
                return false;
            }
            $http({
                method: 'POST',
                url: '/Activity/Apply',
                encType: 'multipart/form-data',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data, headersGetter) {
                    var formData = new FormData();
                    angular.forEach(data, function (value, key) {
                        formData.append(key, value);
                    });
                    var headers = headersGetter();
                    delete headers['Content-Type'];
                    return formData;
                },
                data: $rootScope.request
            }).then(function (result) {
                var strResult;
                if (result.data) {
                    strResult = "申請成功！";
                    $rootScope.request = { 'Type': $rootScope.request.Type, 'USeagate': '無購買', 'UTForce': '無購買' };
                }
                else strResult = "申請失敗，請稍候再試！";
                alert(strResult);
            }, function (error) {
                alert('網路錯誤，請稍候再試！');
            });
        };
    }
]);

amd.directive('input', function () {
    return {
        restrict: 'E',
        scope: {
            ngModel: '=',
            ngChange: '&',
            type: '@'
        },
        link: function (scope, element, attrs) {
            if (attrs.type.toLowerCase() !== 'file') return;
            element.bind('change', function (event) {
                var file = event.target.files[0];
                var ext = event.target.value.substring(event.target.value.lastIndexOf('.')).toUpperCase();
                if (ext !== ".BMP" &&
                    ext !== ".PNG" &&
                    ext !== ".JPG" &&
                    ext !== ".JPEG") {
                    alert('檔案限制 bmp, png, jpeg, jpg 格式！');
                    scope.ngModel = null;
                    return;
                } else if (file.size > 1024 * 1024 * 10) {
                    alert('檔案不能超過 10 MB！');
                    scope.ngModel = null;
                } else scope.ngModel = event.target.files[0];
                scope.$apply();
                scope.ngChange();
            });
        }
    };
});

amd.controller('main', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'main';
    }
]);

amd.controller('xbox', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'xbox';
        $rootScope.request = { 'Type': 1, 'USeagate': '無購買', 'UTForce': '無購買' };
    }
]);

amd.controller('gb', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'gb';
        $rootScope.request = { 'Type': 2, 'USeagate': '無購買', 'UTForce': '無購買' };
    }
]);

amd.controller('again', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'again';
        $rootScope.request = { 'Type': 3, 'USeagate': 'None', 'UTForce': 'None' };
        $('a[title="公告"]').click();
    }
]);

amd.controller('faith', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'faith';
        $rootScope.request = { 'Type': 4, 'USeagate': 'None', 'UTForce': 'None' };
    }
]);

amd.controller('gifts', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'gifts';
        $rootScope.request = { 'Type': 5, 'USeagate': 'None', 'UTForce': 'None' };
    }
]);

amd.controller('progress', ['$rootScope', '$scope', '$http',
    function ($rootScope, $scope, $http) {
        $rootScope.page = 'progress';
        $scope.search = { 'Type': '1' };
        $scope.data = null;
        $scope.query = function () {
            $http.get('/Activity/Query', { params: $scope.search }).then(function (result) {
                if (result.data === false) alert('查詢失敗，請稍候再試。');
                else {
                    $scope.data = result.data;
                }
            }, function (error) {
            });
        };
    }
]);
