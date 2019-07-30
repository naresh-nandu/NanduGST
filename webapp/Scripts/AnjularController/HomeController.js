var app = angular.module('Homeapp', []);

app.controller('HomeController', function ($http, $scope) {

    //Get Pan Number record

    $http.get('/APIReport/Get_Pan').then(function (d) {

        $scope.Panlist = d.data;
       

    }, function () {

        alert('failed');

    });

    // Get GSTIN Number record

    $scope.Gstin = function () {
     
        $http.get('/APIReport/Get_GSTIN?PanNo=' + $scope.Panno).then(function (e) {
            $scope.Gstinlist = e.data;

        });

    };

    
});