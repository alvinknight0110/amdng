var amd = angular.module('amd', ['ngRoute', 'ngSanitize', 'blockUI']).config(['$routeProvider', '$locationProvider',
    function ($routeProvider, $locationProvider) {
        $routeProvider
            .when("/", { templateUrl: '/Act/Index' })
            .when("/xbox", { templateUrl: '/Act/XBox' })
            .when("/gb", { templateUrl: '/Act/GB' })
            .when("/progress", { templateUrl: '/Act/Progress' });
    }
]);