'use strict';

app.controller('exampleController', ['$scope', 'HubProxyFactory', function ($scope, HubProxyFactory) {
    console.log('trying to connect to service');
    var hub = HubProxyFactory('signalr', 'ExampleHub');
    console.log('connected to service');

    $scope.subscribe = function (fqn) {
        hub.invoke('subscribe', '');
    }

    $scope.unsubscribe = function (fqn) {
        hub.invoke('unsubscribe', '');
    }

    hub.on('read', function (value) {
        $scope.time = value;
    })
}
]);