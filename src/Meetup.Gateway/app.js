'use strict';

var Hapi = require('hapi');

const server = Hapi.server({
  host: '0.0.0.0',
  port: 8000
});

// Add the route
server.route({
  method: 'GET',
  path: '/hello',
  options: {
    tags: ['api'],
    description: 'My route description',
    notes: 'My route notes'
  },
  handler: function (request, h) {
    return 'hello world';
  }
});

server.route({
  path: '/',
  method: 'GET',
  handler(request, h) {
    return h.redirect('/docs');
  }
});



// Start the server
async function start() {
  await server.register([
    require('inert'),
    require('vision'),
    {
      plugin: require('hapi-swaggered'),
      options: {
        tags: {
          'foobar/test': 'Example foobar description'
        },
        info: {
          title: 'Example API',
          description: 'Powered by node, hapi, joi, hapi-swaggered, hapi-swaggered-ui and swagger-ui',
          version: '1.0'
        }
      }
    },
    {
      plugin: require('hapi-swaggered-ui'),
      options: {
        title: 'Example API',
        path: '/docs',
        swaggerOptions: {} // see above
      }
    }
  ])

  try {
    await server.start();
  }
  catch (err) {
    console.log(err);
    process.exit(1);
  }

  console.log('Server running at:', server.info.uri);
};

start();
require('./messages-connection').StartRabbit();