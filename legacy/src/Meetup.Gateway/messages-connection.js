var polly = require("polly-js");
var rabbit = require('rabbit.js');
var context;
var pub;

var startRabbit = function (cb) {
    var urlRabbit = process.env.messagebroker || "amqp://localhost"
    context = rabbit.createContext(urlRabbit);

    context.on('ready', function () {
        var pub = context.socket('PUB'), sub = context.socket('SUB');
        sub.on('data', function (event) {
            var obj = JSON.parse(event);
            console.log(obj);
        });

        sub.connect('events', function () {
            pub.connect('events', function () {
                pub.write(JSON.stringify({ welcome: 'rabbit.js' }), 'utf8');
            });
        });
    });

    context.on('error', function (err) {
        cb(err);
    });
}

var createMeetup = function (name) {
    pub.write(JSON.stringify({ name: name }), 'utf8');
}

var startWithRetry = function () {
    polly()
        .handle(function (err) {
            console.log("retrying after error: " + err);
            return true;
        })
        .waitAndRetry([1000, 2000, 4000, 5000, 5000, 5000])
        .executeForNode(startRabbit);
}

exports.StartRabbit = startWithRetry;
