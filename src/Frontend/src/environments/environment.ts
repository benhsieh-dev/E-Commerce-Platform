export const environment = {
  production: false,
  apiUrl: 'https://localhost:5000/graphql',
  apiWsUrl: 'wss://localhost:5000/graphql',
  appName: 'AirBnB Platform',
  version: '1.0.0',
  enableLogging: true,
  googleMapsApiKey: 'YOUR_GOOGLE_MAPS_API_KEY_HERE',
  stripePublishableKey: 'pk_test_YOUR_STRIPE_PUBLISHABLE_KEY_HERE',
  features: {
    enableNotifications: true,
    enableReviews: true,
    enablePayments: true,
    enableChat: false
  }
};