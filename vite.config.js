// Allow specific external hosts (e.g. ngrok) to access the Vite dev server.
//
// The dev server will block requests coming from unknown hostnames for
// security. If you're exposing your local dev server through ngrok or a
// similar tunnel, add the tunnel host here (the host shown in the error
// screenshot).

module.exports = {
  server: {
    // List allowed hosts. Include the exact host from the error screenshot
    // and the parent domain so subdomains are allowed as well.
    allowedHosts: [
      '1f040638cadd.ngrok-free.app',
      '.ngrok-free.app',
      'localhost'
    ]
  }
};
