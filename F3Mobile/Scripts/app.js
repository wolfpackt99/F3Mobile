var app = angular.module("app", ["firebase"]);
app.controller("Ctrl", function ($scopre, $firebaseAuth) {
    var ref = new Firebase("");
    $scope.authObj = $firebaseAuth(ref);

    $scope.authObj.$authWithPassword({

    }).then(function (authData) {

    }
    ).catch(function (error) {

    });
});