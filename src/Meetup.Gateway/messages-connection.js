var polly = require("polly-js");

var startRabbit = function (cb) {
    var urlRabbit = process.env.messagebroker || "amqp://localhost"
    var context = require('rabbit.js').createContext(urlRabbit);

    context.on('ready', function () {
        var pub = context.socket('PUB'), sub = context.socket('SUB');
        sub.pipe(process.stdout);
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