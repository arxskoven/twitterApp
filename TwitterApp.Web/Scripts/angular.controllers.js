angular.module('twitterApp', ['ngRoute'])
    .controller('twitterSearch', function ($scope, $http) {
        $scope.status = {
            working: false,
            loadingTweets: false,
            loadingPlaceTweets: false,
            loadingUserTweets: false,
        };

        $scope.data = {
            tweets: [],
            placeTweets: [],
            userTweets: [],
            user: ''
        };

        $scope.init = function (userId) {
            $scope.data.user = userId;
            $scope.loadOwnTweets();
            $scope.searchPlaceTweets();
            $scope.searchUserTweets();
        };

        // Default input values, for testing in load purpose.
        $scope.input = {
            newTweet: '',
            placeId: 'FBI',
            user: 'arxskoven',
            hashTag: '#Mexico'
        };

        $scope.loadOwnTweets = function () {
            $scope.data.tweets = [];
            $scope.status.loadingTweets = true;
            $http.get('/api/twitter/getMessages?userName=' + $scope.data.user).
                success(function (data, status, headers, config) {
                    $scope.status.loadingTweets = false;
                    $scope.data.tweets = data;
                });
        };

        $scope.newTweet = function () {
            if ($scope.input.newTweet) {
                $scope.status.working = true;
                $scope.input.newTweetEncoded = $scope.input.newTweet.replace(new RegExp('#', 'g'), '%23');
                $http.post('/api/twitter/sendMessage?message=' + $scope.input.newTweetEncoded).
                    success(function (data, status, headers, config) {
                        $scope.data.tweets.unshift($scope.input.newTweet);
                        $scope.input.newTweet = '';
                        $scope.status.working = false;
                    });
            }
            else {
                alert('Write some value for the Tweet');
            }
        };

        $scope.searchPlaceTweets = function () {
            if ($scope.input.placeId) {
                $scope.data.placeTweets = [];
                $scope.status.loadingPlaceTweets = true;
                $http.get('/api/twitter/getMessages?userName=' + $scope.input.placeId + '&count=5').
                    success(function (data, status, headers, config) {
                        $scope.status.loadingPlaceTweets = false;
                        $scope.data.placeTweets = data;
                    });
            }
            else {
                alert('Write some value for the Place');
            }
        };

        $scope.searchUserTweets = function () {
            if ($scope.input.user) {
                $scope.data.userTweets = [];
                $scope.status.loadingUserTweets = true;
                $scope.input.hashTagEncoded = $scope.input.hashTag;
                if ($scope.input.hashTagEncoded) {
                    $scope.input.hashTagEncoded = $scope.input.hashTag.replace(new RegExp('#', 'g'), '%23');
                }

                $http.get('/api/twitter/getMessages?userName=' + $scope.input.user + '&count=5&hashTag=' + $scope.input.hashTagEncoded).
                    success(function (data, status, headers, config) {
                        $scope.status.loadingUserTweets = false;
                        $scope.data.userTweets = data;
                    });
            }
            else {
                alert('Write some value for the User');
            }
        };
    })
    .controller('users', function ($scope, $http) {
        $scope.status = {
            working: false,
            loadingUsers: false,
        };

        $scope.data = {
            users: [],
            currentEmail: ''
        };

        $scope.init = function (currentEmail) {
            $scope.loadUsers();
            $scope.data.currentEmail = currentEmail;
        };

        $scope.input = {
        };

        $scope.loadUsers = function () {
            $scope.data.users = [];
            $scope.status.loadingUsers = true;
            $http.get('/api/user/getUsers').
                success(function (data, status, headers, config) {
                    $scope.status.loadingUsers = false;
                    $scope.data.users = data;
                });
        };

        $scope.deleteUser = function (user) {
            if (user && user.Email) {
                if ($scope.data.currentEmail != user.Email) {
                    $scope.status.working = true;
                    $http.post('/api/user/removeUser?email=' + user.Email).
                        success(function (data, status, headers, config) {
                            for (var i = 0; i < $scope.data.users.length; i++) {
                                if ($scope.data.users[i].Email === user.Email) {
                                    $scope.data.users.splice(i, 1);
                                }
                            }

                            $scope.status.working = false;
                        });
                }
                else {
                    alert('You can not delete your own user.');
                }
            }
            else {
                alert('Select user to delete');
            }
        };
    })